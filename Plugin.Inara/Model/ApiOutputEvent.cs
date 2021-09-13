using DW.ELA.Utility.Json;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.Inara.Model
{
    public sealed class ApiOutputEvent
    {
        [JsonProperty("eventData")]
        public object EventData { get; internal set; }

        /// <summary>
        /// Gets status of event processing as returned by API
        /// </summary>
        [JsonProperty("eventStatus")]
        public int? EventStatus { get; internal set; }

        /// <summary>
        /// Gets error codes as returned by API
        /// </summary>
        [JsonProperty("eventStatusText")]
        public string EventStatusText { get; internal set; }

        public override string ToString() => Serialize.ToJson(this);

    }
}
