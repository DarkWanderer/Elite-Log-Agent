using DW.ELA.Interfaces;
using DW.ELA.LogModel;
using DW.ELA.Utility.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class LogBurstPlayer : BasicObservable<LogEvent>
    {
        private readonly string LogDirectory;
        private readonly int filesNumber;

        public LogBurstPlayer(string logDirectory, int filesNumber = 5)
        {
            if (String.IsNullOrEmpty(logDirectory))
                throw new ArgumentException("Must provide log directory", nameof(logDirectory));
            if (filesNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(logDirectory), filesNumber, "nubmer of files must be > 0");

            LogDirectory = logDirectory;
            this.filesNumber = filesNumber;
        }

        public IEnumerable<LogEvent> Events
        {
            get
            {
                var reader = new LogReader();

                var files = LogEnumerator.GetLogFiles(LogDirectory)
                    .Take(filesNumber)
                    .OrderBy(x => x) // from oldest to freshest
                    .ToList();

                // Read journal events
                var events = files.SelectMany(f => reader.ReadEventsFromJournal(f));
                foreach (var @event in events)
                    yield return @event;

                // Read Market.json, Outfitting.json etc.
                // Disabled - it is unclear for now how to deal with beta data
                //files = LogEnumerator.GetJsonEventFiles(LogDirectory).ToList();
                //events = files.Select(f => reader.ReadFileEvent(f)).Where(e => e != null);
                //foreach (var @event in events)
                //    yield return @event;
            }
        }

        public void Play()
        {
            foreach (var @event in Events)
                OnNext(@event);
            OnCompleted();
        }
    }
}