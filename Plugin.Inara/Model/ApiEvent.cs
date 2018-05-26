using Newtonsoft.Json;
using System;

namespace InaraUpdater.Model
{
    public sealed class ApiEvent
    {
        public ApiEvent(string eventName)
        {
            EventName = eventName;
            CustomID = 0;
        }

        [JsonProperty("eventName")]
        public string EventName { get; }

        [JsonProperty("eventTimestamp")]
        public DateTime Timestamp { get; internal set; }

        [JsonProperty("eventCustomID")]
        public int CustomID { get; }

        [JsonProperty("eventData")]
        public object EventData { get; internal set; }

        public override string ToString() => JsonConvert.SerializeObject(this);

        /// <summary>
        /// Denotes status of event processing as returned by API
        /// </summary>
        [JsonProperty("eventStatus")]
        public int? EventStatus { get; internal set; }

        /// <summary>
        /// Denotes error codes as returned by API
        /// </summary>
        [JsonProperty("eventStatusText")]
        public string EventStatusText { get; internal set; }
    }
}
