namespace DW.ELA.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.LogModel;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class TestEventSource
    {
        public static IEnumerable<LogEvent> TypedLogEvents => CannedEvents.Where(e => e.GetType() != typeof(LogEvent));

        public static IEnumerable<LogEvent> CannedEvents => CannedEventsRaw.Select(LogEventConverter.Convert);

        public static IEnumerable<JObject> CannedEventsRaw
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "DW.ELA.UnitTests.CannedEvents.json";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var textReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true, CloseInput = false })
                {
                    while (jsonReader.Read())
                    {
                        yield return Converter.Serializer.Deserialize<JObject>(jsonReader);
                    }
                }
            }
        }

        public static IEnumerable<JObject> LocalBetaEvents
        {
            get
            {
                foreach (string file in Directory.EnumerateFiles(new SavedGamesDirectoryHelper().Directory, "JournalBeta.*.log"))
                {
                    using (var fileReader = File.OpenRead(file))
                    using (var textReader = new StreamReader(fileReader))
                    using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true, CloseInput = false })
                    {
                        while (jsonReader.Read())
                        {
                            yield return Converter.Serializer.Deserialize<JObject>(jsonReader);
                        }
                    }
                }
            }
        }

        public static IEnumerable<JObject> LocalEvents
        {
            get
            {
                foreach (string file in Directory.EnumerateFiles(new SavedGamesDirectoryHelper().Directory, "Journal.*.log"))
                {
                    using (var fileReader = File.OpenRead(file))
                    using (var textReader = new StreamReader(fileReader))
                    using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true, CloseInput = false })
                    {
                        while (jsonReader.Read())
                        {
                            yield return Converter.Serializer.Deserialize<JObject>(jsonReader);
                        }
                    }
                }
            }
        }
    }
}
