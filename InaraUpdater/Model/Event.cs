using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public abstract class Event
    {
        [JsonProperty("eventName")]
        public abstract string EventName { get; }

        [JsonProperty("eventTimestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("eventCustomID")]
        public int CustomID { get; }

        [JsonProperty("eventData")]
        public object EventData { get; }
    }
}
