using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Rank : LogEvent
    {
        [JsonProperty("Combat")]
        public int Combat { get; set; }

        [JsonProperty("Trade")]
        public int Trade { get; set; }

        [JsonProperty("Explore")]
        public int Explore { get; set; }

        [JsonProperty("Empire")]
        public int Empire { get; set; }

        [JsonProperty("Federation")]
        public int Federation { get; set; }

        [JsonProperty("CQC")]
        public int Cqc { get; set; }
    }
}
