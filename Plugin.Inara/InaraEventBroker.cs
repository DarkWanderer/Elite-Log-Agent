using InaraUpdater.Model;
using Interfaces;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace InaraUpdater
{
    public class InaraEventBroker : IObserver<JObject>
    {
        private readonly InaraApiFacade apiFacade;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly List<ApiEvent> eventQueue = new List<ApiEvent>();
        private readonly Timer logFlushTimer = new Timer();

        private void Queue(ApiEvent e)
        {
            lock (eventQueue)
            {
                if (e != null && e?.Timestamp > DateTime.UtcNow.AddDays(-30)) // INARA API only accepts events for last month 
                {
                    eventQueue.Add(e);
                    logger.Trace("Queued event {0}", e);
                }
            }
        }

        public async void FlushQueue()
        {
            try
            {
                ApiEvent[] apiEvents;
                lock (eventQueue)
                {
                    apiEvents = Compact(eventQueue)
                       //.Where(e => e.EventName == "addCommanderTravelDock" || e.EventName == "addCommanderTravelFSDJump") // DEBUG
                       .ToArray();
                    eventQueue.Clear();
                }
                if (apiEvents.Any())
                    await apiFacade.ApiCall(apiEvents);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while flushing event queue");
            }
        }

        private static readonly string[] compactableEvents = new[] {
            "setCommanderInventoryMaterials",
            "setCommanderGameStatistics"
        };

        public InaraEventBroker(InaraApiFacade apiFacade, IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.apiFacade = apiFacade ?? throw new ArgumentNullException(nameof(apiFacade));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = 5000; // send data every n seconds
            logFlushTimer.Elapsed += (o, e) => Task.Factory.StartNew(FlushQueue);
            logFlushTimer.Enabled = true;
        }

        public void OnCompleted() => FlushQueue();

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
                    case "PVPKill":
                        Queue(ToPvpKillEvent(@event)); break; 
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in OnNext");
            }
        }

        private ApiEvent ToPvpKillEvent(JObject @event)
        {
            var timestamp = DateTime.Parse(@event["timestamp"].ToString());
            return new ApiEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(timestamp) },
                    { "opponentName", @event["Victim"].ToString() },
                },
                Timestamp = timestamp
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

            if (@event["StarSystem"] == null)
                return null;

            return new ApiEvent(eventType)
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"]?.ToString() },
                    { "opponentName", (@event["Interdicted"] ?? @event["Interdictor"] ?? "Unknown").ToString() },
                    { "isPlayer", @event["IsPlayer"]?.ToObject<long>() },
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
            var timestamp = DateTime.Parse(@event["timestamp"].ToString());
            return new ApiEvent("addCommanderTravelDock")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "stationName", @event["StationName"].ToString()},
                    { "marketID", @event["MarketID"]?.ToObject<long>() },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private ApiEvent ToFsdJumpEvent(JObject @event)
        {
            var timestamp = DateTime.Parse(@event["timestamp"].ToString());
            return new ApiEvent("addCommanderTravelFSDJump")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"].ToString() },
                    { "jumpDistance", @event["JumpDist"].ToObject<double>() },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
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

        private ApiEvent ToCommanderCreditsEvent(JObject @event)
        {
            return new ApiEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object> {
                    { "commanderCredits", @event["Credits"]?.ToObject<long>() },
                    { "commanderLoan", @event["Loan"]?.ToObject<long>() }
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
