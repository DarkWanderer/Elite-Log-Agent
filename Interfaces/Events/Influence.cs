namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Influence
    {
        [JsonProperty("SystemAddress")]
        public long SystemAddress { get; set; }

        [JsonProperty("Trend")]
        public string Trend { get; set; }

        [JsonProperty("Influence")]
        public string InfluenceValue { get; set; }
    }
}
