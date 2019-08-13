namespace DW.ELA.Plugin.Inara.Model
{
    using System;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;

    public sealed class ApiInputEvent
    {
        public ApiInputEvent(string eventName)
        {
            EventName = eventName;
        }

        [JsonProperty("eventName")]
        public string EventName { get; }

        [JsonProperty("eventTimestamp")]
        public DateTime Timestamp { get; internal set; }

        [JsonProperty("eventData")]
        public object EventData { get; internal set; }


        public override string ToString() => Serialize.ToJson(this);
    }
}
