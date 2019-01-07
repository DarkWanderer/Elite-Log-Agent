namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public partial class FactionStateTrend
    {
        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Trend")]
        public long? Trend { get; set; }
    }
}
