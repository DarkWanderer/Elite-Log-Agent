using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public partial class FactionStateTrend
    {
        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Trend")]
        public long? Trend { get; set; }
    }
}
