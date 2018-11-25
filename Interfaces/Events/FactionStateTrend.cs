namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FactionStateTrend
    {
        [JsonProperty("State")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FactionState State { get; set; }

        [JsonProperty("Trend")]
        public long? Trend { get; set; }
    }
}
