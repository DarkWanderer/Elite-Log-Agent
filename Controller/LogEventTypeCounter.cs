using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Controller
{
    public class LogEventTypeCounter : IObserver<string>
    {
        private static IDictionary<string, int> eventTypeCounters = new Dictionary<string,int>();

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(string value)
        {
            var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(value);
            var name = data["event"] as string;

            if (!eventTypeCounters.ContainsKey(name))
                eventTypeCounters.Add(name, 1);
            else
                eventTypeCounters[name]++;
        }
    }
}
