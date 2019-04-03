namespace DW.ELA.UnitTests.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.LogModel;
    using DW.ELA.Utility.Json;
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
                .Select(e =>
                {
                    try
                    {
                        return LogEventConverter.Convert(e);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(e => e != null && e.GetType() == typeof(LogEvent))
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
            var eventExamples = LoadJsonEvents()
                .Concat(ExtractSamples(TestEventSource.LocalBetaEvents))
                .Concat(ExtractSamples(TestEventSource.LocalEvents))
                .Concat(ExtractSamples(TestEventSource.LocalStaticEvents))
                .ToHashSet();

            string eventsString = string.Join("\n", eventExamples);
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

        private static IEnumerable<string> LoadJsonEvents() => LogEnumerator.GetJsonEventFiles(new SavedGamesDirectoryHelper().Directory)
            .Select(File.ReadAllText)
            .Select(Serialize.FromJson<JObject>)
            .Select(Serialize.ToJson);
    }
}
