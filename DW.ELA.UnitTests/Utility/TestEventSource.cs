using Controller;
using DW.ELA.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MoreLinq;
using DW.ELA.Utility.Json;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using DW.ELA.LogModel;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DW.ELA.UnitTests
{
    public static class TestEventSource
    {
        public static IEnumerable<LogEvent> LogEvents => GetLogEvents();
        public static IEnumerable<LogEvent> TypedLogEvents => GetLogEvents().Where(e => e.GetType() != typeof(LogEvent));

        private static IEnumerable<LogEvent> GetLogEvents()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DW.ELA.UnitTests.CannedEvents.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true })
            {
                var serializer = new JsonSerializer();
                while (jsonReader.Read())
                {
                    var @object = (JObject)serializer.Deserialize(jsonReader);
                    yield return LogEventConverter.Convert(@object);
                }
            }
        }

        /// <summary>
        /// Utility method for developer to get canned events from own logs
        /// </summary>
        /// <returns></returns>
        [Test]
        [Explicit]
        public static void PrepareCannedEvents()
        {
            var logEventPlayer = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 10000);
            var events = new ConcurrentStack<LogEvent>();
            using (logEventPlayer.Subscribe(e => events.Push(e)))
                logEventPlayer.Play();

            var eventExamples = events
                .GroupBy(e => e.Event)
                .SelectMany(g => g.OrderByDescending(e => e.Timestamp).Take(5))
                .OrderBy(e => e.Event).ThenBy(e => e.Timestamp)
                .Select(e => e.Raw)
                .Select(Serialize.ToJson)
                .ToList();

            events.Clear();

            var eventsString = string.Join("\n", eventExamples.ToArray());
            Assert.IsNotEmpty(eventsString);
        }
    }
}
