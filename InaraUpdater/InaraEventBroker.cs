using InaraUpdater.Model;
using MoreLinq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InaraUpdater
{
    public class InaraEventBroker : IObserver<JObject>
    {
        private readonly ApiFacade apiFacade;
        private readonly List<ApiEvent> eventQueue = new List<ApiEvent>();
        private void Queue(ApiEvent e) => eventQueue.Add(e);

        private static readonly string[] compactableEvents = new[] {
            "setCommanderInventoryMaterials",
            "setCommanderGameStatistics"
        };

        public InaraEventBroker(ApiFacade apiFacade)
        {
            this.apiFacade = apiFacade ?? throw new ArgumentNullException(nameof(apiFacade));
        }

        public void OnCompleted()
        {
            var apiEvents = Compact(eventQueue)
                .Where(e => e.Timestamp > DateTime.UtcNow.AddDays(-30)) // INARA API only accepts events for last month
                .ToList();
            apiFacade.ApiCall(apiEvents).Wait();
        }

        private static IEnumerable<ApiEvent> Compact(IEnumerable<ApiEvent> events)
        {
            var eventsByType = events
                .GroupBy(e => e.EventName, e => e)
                .ToDictionary(g => g.Key, g => g.ToArray());
            foreach (var type in compactableEvents.Intersect(eventsByType.Keys))
                eventsByType[type] = new[] { eventsByType[type].MaxBy(e => e.Timestamp) };

            return eventsByType.Values.SelectMany(ev => ev).OrderBy(e => e.Timestamp);
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(JObject @event)
        {
            try
            {
                var eventName = @event["event"].ToString();
                switch (eventName)
                {
                    // Generic
                    case "LoadGame": Queue(ToCommanderCreditsEvent(@event)); break;
                    case "Materials": Queue(ToMaterialsInventoryEvent(@event)); break;
                    case "Statistics": Queue(ToStatisticsEvent(@event)); break;

                    // Travel
                    case "FSDJump": Queue(ToFsdJumpEvent(@event)); break;
                    case "Docked": Queue(ToDockedEvent(@event)); break;

                    // Engineers
                    case "EngineerProgress": Queue(ToEngineerProgressEvent(@event)); break;

                    // Combat
                    case "Interdicted":
                    case "Interdiction":
                    case "EscapeInterdiction":
                        Queue(ToInterdictionEvent(@event)); break;
                    //case "PVPKill":
                        //Queue(ToPvpKillEvent(@event)); break; 
                }
            }
            catch
            {
            }
        }

        private ApiEvent ToPvpKillEvent(JObject @event)
        {
            // TODO: where do I take star system from?
            return new ApiEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "opponentName", @event["Victim"].ToString() },
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToInterdictionEvent(JObject @event)
        {
            string eventType;
            switch (@event["event"].ToString())
            {
                case "Interdicted": eventType = "addCommanderCombatInterdicted"; break;
                case "Interdiction": eventType = "addCommanderCombatInterdiction"; break;
                case "EscapeInterdiction": eventType = "addCommanderCombatInterdictionEscape"; break;
                default: throw new ArgumentOutOfRangeException(nameof(@eventType));
            }
            return new ApiEvent(eventType)
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "opponentName", (@event["Interdicted"] ?? @event["Interdictor"]).ToString() },
                    { "isPlayer", @event["IsPlayer"]?.ToObject<Int64>() },
                    { "isSuccess", @event["Success"]?.ToObject<bool>() }
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToStatisticsEvent(JObject @event)
        {
            return new ApiEvent("setCommanderGameStatistics")
            {
                EventData = @event.ToObject<Dictionary<string, object>>(),
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToDockedEvent(JObject @event)
        {
            return new ApiEvent("addCommanderTravelDock")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "stationName", @event["StationName"].ToString()},
                    { "marketID", @event["MarketID"]?.ToObject<Int64>() }
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToMaterialsInventoryEvent(JObject @event)
        {
            var materialCounts = @event["Raw"]
                .ToDictionary(
                    arrayItem => arrayItem["Name"].ToString(),
                    arrayItem => (object)Int32.Parse(arrayItem["Count"].ToString())
                );

            return new ApiEvent("setCommanderInventoryMaterials")
            {
                EventData = materialCounts.Select(kvp => new { itemName = kvp.Key, itemCount = kvp.Value }).ToArray(),
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToFsdJumpEvent(JObject @event)
        {
            return new ApiEvent("addCommanderTravelFSDJump")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "jumpDistance", @event["JumpDist"].ToObject<double>() }
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToCommanderCreditsEvent(JObject @event)
        {
            return new ApiEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object> {
                    { "commanderCredits", @event["Credits"]?.ToObject<Int64>() },
                    { "commanderLoan", @event["Loan"]?.ToObject<Int64>() }
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }

        private ApiEvent ToEngineerProgressEvent(JObject @event)
        {
            return new ApiEvent("setCommanderRankEngineer")
            {
                EventData = new Dictionary<string, object> {
                    { "engineerName", @event["Engineer"].ToString() },
                    { "rankStage", @event["Progress"]?.ToString() },
                    { "rankValue", @event["Rank"]?.ToObject<int>() }
                },
                Timestamp = DateTime.Parse(@event["timestamp"].ToString())
            };
        }
    }
}
