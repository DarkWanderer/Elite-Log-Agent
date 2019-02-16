namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;

    public class LogEventTypeCounter : IObserver<LogEvent>
    {
        private ConcurrentDictionary<string, int> eventTypeCounters = new ConcurrentDictionary<string, int>();


        public IReadOnlyCollection<string> EventTypes =>
            eventTypeCounters.Keys.Distinct().OrderBy(x => x).ToList();

        public IReadOnlyDictionary<string, int> EventCounts => eventTypeCounters;

        public void OnNext(LogEvent value)
        {
            try
            {
                eventTypeCounters.AddOrUpdate(value.Event, key => 1, (key, count) => count + 1);
            }
            catch
            { /* do nothing */
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void FromEnumerable(IEnumerable<LogEvent> events)
        {
            foreach (var e in events)
                OnNext(e);
        }
    }
}
