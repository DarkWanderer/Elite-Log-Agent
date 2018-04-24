﻿using InaraUpdater.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;

namespace InaraUpdater
{
    public class EventBroker : IObserver<JObject>
    {
        private readonly ApiFacade apiFacade;
        private readonly List<ApiEvent> eventQueue = new List<ApiEvent>();
        private void Queue(ApiEvent e) => eventQueue.Add(e);

        private static readonly string[] compactableEvents = new[] {
            "setCommanderInventoryMaterials",
            "setCommanderGameStatistics"
        };

        public EventBroker(ApiFacade apiFacade)
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
                eventsByType[type] = new[] { eventsByType[type].WithMaximal(e => e.Timestamp) };

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
                    case "LoadGame": Queue(ToCommanderCreditsEvent(@event)); break;
                    case "Materials": Queue(ToMaterialsInventoryEvent(@event)); break;
                    case "Statistics": Queue(ToStatisticsEvent(@event)); break;
                    case "FSDJump": Queue(ToFsdJumpEvent(@event)); break;
                    case "Docked": Queue(ToDockedEvent(@event)); break;

                    case "EngineerProgress": Queue(ToEngineerProgressEvent(@event)); break;
                }
            }
            catch
            {
            }
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
