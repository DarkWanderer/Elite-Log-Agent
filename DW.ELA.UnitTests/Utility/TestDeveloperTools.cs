using Controller;
using DW.ELA.Interfaces;
using DW.ELA.LogModel;
using DW.ELA.Utility.Json;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DW.ELA.UnitTests.Utility
{
    public static class TestDeveloperTools
    {
        [Test]
        [Explicit]
        public static void ToolGetUnmappedEvents()
        {
            var events = TestEventSource.LocalEvents
                .Concat(TestEventSource.LocalBetaEvents)
                .Where(e => e.GetType() == typeof(LogEvent))
                .Select(e => e.Event)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            Assert.Pass(string.Join(", ", events));
        }

        private static JObject ReplaceTimestamp(JObject input)
        {
            var value = (JObject)input.DeepClone();
            value["timestamp"] = new DateTime(2018, 08, 28);
            return value;
        }

        private static IEnumerable<string> ExtractSamples(IEnumerable<LogEvent> events)
        {
            return events
                .GroupBy(e => e.Event)
                .SelectMany(g => g.OrderByDescending(e => e.Timestamp).Take(5))
                .OrderBy(e => e.Event).ThenBy(e => e.Timestamp)
                .Select(e => e.Raw)
                .Select(ReplaceTimestamp)
                .Select(Serialize.ToJson);
        }

        /// <summary>
        /// Utility method for developer to get canned events from own logs
        /// </summary>
        /// <returns></returns>
        [Test]
        [Explicit]
        public static void ToolPrepareCannedEvents()
        {
            var eventExamples = ExtractSamples(TestEventSource.LocalBetaEvents)
                .Concat(ExtractSamples(TestEventSource.LocalEvents))
                .ToHashSet();

            var eventsString = string.Join("\n", eventExamples.ToArray());
            Assert.IsNotEmpty(eventsString);
        }

        //[Test]
        //[Explicit]
        //public static void GenerateCodeForUnknownEvents()
        //{
        //    var eventExamples = GetLocalLogEvents()
        //        .Where(e => e.GetType() == typeof(LogEvent))
        //        .GroupBy(e => e.Event)
        //        .Select(g => GenerateCsharpDescription(g.ToList()))
        //        .ToList();
        //}

        //private static string GenerateCsharpDescription(IEnumerable<LogEvent> events)
        //{
        //    var mergeSettings = new JsonMergeSettings
        //    {
        //        // union array values together to avoid duplicates
        //        MergeArrayHandling = MergeArrayHandling.Union
        //    };

        //    var eventTypeName = events
        //        .Select(e => e.Event)
        //        .Distinct()
        //        .Single();
        //    var rawEvents = events.Select(e => e.Raw);
        //    var mergedJsonObject = rawEvents.First();
        //    foreach (var e in rawEvents.Skip(1))
        //        mergedJsonObject.Merge(e);

        //    var schema = JsonSchema4.FromSampleJson(Serialize.ToJson(mergedJsonObject));
        //    var generator = new CSharpGenerator(schema);
        //    var settings = generator.Settings;
        //    settings.Namespace = typeof(Commander).Namespace;
        //    settings.ClassStyle = CSharpClassStyle.Record;
        //    settings.GenerateDataAnnotations = false;
        //    settings.JsonConverters = new[] { typeof(StringEnumConverter).FullName };
        //    settings.ArrayType = "Array";
        //    settings.GenerateJsonMethods = false;
        //    settings.RequiredPropertiesMustBeDefined = false;

        //    var code = generator.GenerateFile(eventTypeName);
        //    return code;
        //}
    }
}
