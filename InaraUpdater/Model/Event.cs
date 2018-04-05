using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public sealed class Event
    {
        private readonly DateTime _timestamp;

        public Event(string eventName, IDictionary<string, object> eventData)
        {
            EventName = eventName;
            _timestamp = DateTime.UtcNow;
            CustomID = 0;
            EventData = eventData;
        }

        [JsonProperty("eventName")]
        public string EventName { get; }

        [JsonProperty("eventTimestamp")]
        public DateTime Timestamp => _timestamp;

        [JsonProperty("eventCustomID")]
        public int CustomID { get; }

        [JsonProperty("eventData")]
        public IDictionary<string, object> EventData { get; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
