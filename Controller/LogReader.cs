namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.LogModel;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class LogReader
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly HashSet<string> SkippedEvents = new HashSet<string>(new[] { "Outfitting", "Shipyard", "Market" });

        /// <summary>
        /// Reads the given Journal file from specified position and generates the events
        /// </summary>
        /// <param name="textReader">Stream reader for input data</param>
        /// <returns>Sequence of events read from input</returns>
        public IEnumerable<LogEvent> ReadEventsFromStream(TextReader textReader)
        {
            if (textReader == null)
                throw new ArgumentNullException(nameof(textReader));
            using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true, CloseInput = false })
            {
                while (jsonReader.Read())
                {
                    var @object = Converter.Serializer.Deserialize<JObject>(jsonReader);
                    LogEvent? @event = null;
                    try
                    {
                        @event = LogEventConverter.Convert(@object);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deserializing event from journal");
                    }
                    if (@event != null)
                        yield return @event;
                }
            }
        }

        public LogEvent ReadFileEvent(string file)
        {
            using (var fileReader = OpenForSharedRead(file))
            using (var textReader = new StreamReader(fileReader))
                return ReadEventsFromStream(textReader).SingleOrDefault();
        }

        public IEnumerable<LogEvent> ReadEventsFromJournal(string journalFile)
        {
            using (var fileReader = OpenForSharedRead(journalFile))
            using (var textReader = new StreamReader(fileReader))
            {
                foreach (var @event in ReadEventsFromStream(textReader))
                    yield return @event;
            }
        }

        private Stream OpenForSharedRead(string file) => new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
}
