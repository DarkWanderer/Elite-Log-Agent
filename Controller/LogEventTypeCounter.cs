using DW.ELA.Interfaces;
using DW.ELA.LogModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Controller
{
    public class LogEventTypeCounter : IObserver<LogEvent>
    {
        private ConcurrentDictionary<string, int> eventTypeCounters = new ConcurrentDictionary<string, int>();

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public IReadOnlyCollection<string> EventTypes =>
            eventTypeCounters.Keys.Distinct().OrderBy(x => x).ToList();

        public void OnNext(LogEvent value)
        {
            try
            {
                eventTypeCounters.AddOrUpdate(value.Event, key => 1, (key, count) => count++);
            }
            catch { }
        }
    }
}
