namespace DW.ELA.UnitTests.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.LogModel;
    using DW.ELA.Utility.Json;
    using MoreLinq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public static class TestDeveloperTools
    {
        [Test]
        [Explicit]
        public static void ToolGetUnmappedEvents()
        {
            var events = TestEventSource.LocalEvents
                .Concat(TestEventSource.LocalBetaEvents)
                .Select(LogEventConverter.Convert)
                .Where(e => e.GetType() == typeof(LogEvent))
                .Select(e => e.Event)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            Assert.Pass(string.Join(", ", events));
        }

        /// <summary>
        /// Utility method for developer to get canned events from own logs
        /// </summary>
        [Test]
        [Explicit]
        public static void ToolPrepareCannedEvents()
        {
            var eventExamples = ExtractSamples(TestEventSource.LocalBetaEvents)
                .Concat(ExtractSamples(TestEventSource.LocalEvents))
                .ToHashSet();

            var eventsString = string.Join("\n", eventExamples);
            Assert.IsNotEmpty(eventsString);
        }

        private static JObject ReplaceTimestamp(JObject input)
        {
            var value = (JObject)input.DeepClone();
            if (value["timestamp"] != null)
                value["timestamp"] = new DateTime(2018, 08, 28);

            if (value["Cost"] != null)
                value["Cost"] = 100;

            if (value["Amount"] != null)
                value["Amount"] = 200;

            return value;
        }

        private static IEnumerable<string> ExtractSamples(IEnumerable<JObject> events)
        {
            //return events
            //    .GroupBy(jo => jo.Property("event").Value.ToString())
            //    .SelectMany(g => g.OrderByDescending(jo => jo.Property("timestamp").Value.ToObject<DateTime>()).Take(5))
            //    .OrderBy(e => e.Event).ThenBy(e => e.Timestamp)
            //    .Select(e => e.Raw)
            //    .Select(ReplaceTimestamp)
            //    .Select(Serialize.ToJson);

            var eventGroups = from @event in events
                              let eventName = @event.Property("event").Value.ToString()
                              let timestamp = @event.Property("timestamp").Value.ToObject<DateTime>()
                              orderby eventName ascending, timestamp descending
                              group @event by eventName into eventGroup
                              select eventGroup;

            var processedEvents = eventGroups
                .SelectMany(g => g.Take(5))
                .Select(ReplaceTimestamp)
                .Select(Serialize.ToJson)
                .ToList();

            return processedEvents;
        }
    }
}
