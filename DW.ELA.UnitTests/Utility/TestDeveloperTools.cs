using Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Events;
using DW.ELA.Utility.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.Utility
{
    public static class TestDeveloperTools
    {
        private static IEnumerable<LogEvent> GetLocalLogEvents()
        {
            var logEventPlayer = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 30);
            return logEventPlayer.Events.ToList();
        }

        /// <summary>
        /// Utility method for developer to get canned events from own logs
        /// </summary>
        /// <returns></returns>
        [Test]
        [Explicit]
        public static void PrepareCannedEvents()
        {
            var eventExamples = GetLocalLogEvents()
                .GroupBy(e => e.Event)
                .SelectMany(g => g.OrderByDescending(e => e.Timestamp).Take(5))
                .OrderBy(e => e.Event).ThenBy(e => e.Timestamp)
                .Select(e => e.Raw)
                .Select(Serialize.ToJson)
                .ToList();

            var eventsString = string.Join("\n", eventExamples.ToArray());
            Assert.IsNotEmpty(eventsString);
        }

        [Test]
        [Explicit]
        public static void GenerateCodeForUnknownEvents()
        {
            var eventExamples = GetLocalLogEvents()
                .Where(e => e.GetType() == typeof(LogEvent))
                .GroupBy(e => e.Event)
                .Select(g => GenerateCsharpDescription(g.ToList()))
                .ToList();
        }

        private static string GenerateCsharpDescription(IEnumerable<LogEvent> events)
        {
            var mergeSettings = new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            };

            var eventTypeName = events
                .Select(e => e.Event)
                .Distinct()
                .Single();
            var rawEvents = events.Select(e => e.Raw);
            var mergedJsonObject = rawEvents.First();
            foreach (var e in rawEvents.Skip(1))
                mergedJsonObject.Merge(e);

            var schema = JsonSchema4.FromSampleJson(Serialize.ToJson(mergedJsonObject));
            var generator = new CSharpGenerator(schema);
            var settings = generator.Settings;
            settings.Namespace = typeof(Commander).Namespace;
            settings.ClassStyle = CSharpClassStyle.Record;
            settings.GenerateDataAnnotations = false;
            settings.JsonConverters = new[] { typeof(StringEnumConverter).FullName };
            settings.ArrayType = "Array";
            settings.GenerateJsonMethods = false;
            settings.RequiredPropertiesMustBeDefined = false;

            var code = generator.GenerateFile(eventTypeName);
            return code;
        }
    }
}
