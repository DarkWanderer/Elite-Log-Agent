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
        public static IEnumerable<LogEvent> TypedLogEvents => CannedEvents.Where(e => e.GetType() != typeof(LogEvent));

        public static IEnumerable<LogEvent> CannedEvents
        {
            get
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

        public static IEnumerable<LogEvent> LocalBetaEvents
        {
            get
            {
                foreach (var file in Directory.EnumerateFiles(new SavedGamesDirectoryHelper().Directory, "JournalBeta.*.log"))
                    using (var fileReader = File.OpenRead(file))
                    using (var textReader = new StreamReader(fileReader))
                    {
                        var reader = new LogReader();
                        foreach (var @event in reader.ReadEventsFromStream(textReader))
                            yield return @event;
                    }
            }
        }

        public static IEnumerable<LogEvent> LocalEvents
        {
            get
            {
                foreach (var file in Directory.EnumerateFiles(new SavedGamesDirectoryHelper().Directory, "Journal.*.log"))
                    using (var fileReader = File.OpenRead(file))
                    using (var textReader = new StreamReader(fileReader))
                    {
                        var reader = new LogReader();
                        foreach (var @event in reader.ReadEventsFromStream(textReader))
                            yield return @event;
                    }
            }
        }
    }
}
