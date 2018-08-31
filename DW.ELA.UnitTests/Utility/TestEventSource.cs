using Controller;
using DW.ELA.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

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
            {
                var reader = new LogReader();
                foreach (var @event in reader.ReadEventsFromStream(textReader))
                    yield return @event;
            }
        }
    }
}
