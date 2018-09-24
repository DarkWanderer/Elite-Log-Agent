using Controller;
using DW.ELA.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System;

namespace DW.ELA.UnitTests
{
    public static class TestEventSource
    {
        public static IEnumerable<LogEvent> LogEvents => lazyEvents.Value;
        public static IEnumerable<LogEvent> TypedLogEvents => lazyEvents.Value.Where(e => e.GetType() != typeof(LogEvent));

        private static readonly Lazy<IReadOnlyCollection<LogEvent>> lazyEvents = new Lazy<IReadOnlyCollection<LogEvent>>(LoadEvents);

        private static IReadOnlyCollection<LogEvent> LoadEvents()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DW.ELA.UnitTests.CannedEvents.json";
            var events = new List<LogEvent>();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var textReader = new StreamReader(stream))
            {
                var reader = new LogReader();
                events.AddRange(reader.ReadEventsFromStream(textReader));
            }
            return events;
        }
    }
}
