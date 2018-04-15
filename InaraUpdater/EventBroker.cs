using InaraUpdater.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater
{
    public class EventBroker : IObserver<JObject>
    {
        private readonly ApiFacade apiFacade;
        private readonly List<JObject> creditEvents = new List<JObject>();

        public EventBroker(ApiFacade apiFacade)
        {
            this.apiFacade = apiFacade ?? throw new ArgumentNullException(nameof(apiFacade));
        }

        public void OnCompleted()
        {
            var apiEvents = creditEvents
                .AsEnumerable()
                .Reverse()
                .Take(100)
                .Select(@event => new ApiEvent("setCommanderCredits")
                {
                    EventData = new Dictionary<string, object> { { "commanderCredits", Int64.Parse(@event["Credits"].ToString()) } },
                    Timestamp = DateTime.Parse(@event["timestamp"].ToString())
                })
                .ToList();
            
            apiFacade.ApiCall(apiEvents).Wait();
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(JObject @event)
        {
            try
            {
                var eventName = @event["event"].ToString();
                switch (eventName)
                {
                    case "LoadGame":
                        creditEvents.Add(@event);
                        break;
                }
            }
            catch
            {
            }
        }
    }
}
