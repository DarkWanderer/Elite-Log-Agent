namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using Utility.Observable;

    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class JournalBurstPlayer : BasicObservable<JournalEvent>
    {
        private readonly string logDirectory;
        private readonly int filesNumber;

        public JournalBurstPlayer(string logDirectory, int filesNumber = 5)
        {
            if (string.IsNullOrEmpty(logDirectory))
                throw new ArgumentException("Must provide log directory", nameof(logDirectory));
            if (filesNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(logDirectory), filesNumber, "nubmer of files must be > 0");

            this.logDirectory = logDirectory;
            this.filesNumber = filesNumber;
        }

        public IEnumerable<JournalEvent> Events
        {
            get
            {
                var reader = new JournalFileReader();

                var files = JournalFileEnumerator.GetLogFiles(logDirectory)
                    .Take(filesNumber)
                    .OrderBy(x => x) // from oldest to freshest
                    .ToList();

                // Read journal events
                var events = files.SelectMany(f => reader.ReadEventsFromJournal(f));
                foreach (var @event in events)
                    yield return @event;

                /*
                // Read Market.json, Outfitting.json etc.
                // Disabled - it is unclear for now how to deal with beta data
                files = LogEnumerator.GetJsonEventFiles(LogDirectory).ToList();
                events = files.Select(f => reader.ReadFileEvent(f)).Where(e => e != null);
                foreach (var @event in events)
                    yield return @event;
                */
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