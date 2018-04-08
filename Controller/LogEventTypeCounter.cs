using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controller
{
    public class LogEventTypeCounter : IObserver<JObject>
    {
        private IDictionary<string, int> eventTypeCounters = new Dictionary<string,int>();

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public IReadOnlyCollection<string> EventTypes => 
            eventTypeCounters.Keys.Distinct().OrderBy(x => x).ToList();

        public void OnNext(JObject value)
        {
            try
            {
                var name = value["event"].ToString();

                if (!eventTypeCounters.ContainsKey(name))
                    eventTypeCounters.Add(name, 1);
                else
                    eventTypeCounters[name]++;
            }
            catch
            {
            }
        }
    }
}
