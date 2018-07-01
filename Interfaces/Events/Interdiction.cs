using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class Interdiction : LogEvent
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("IsPlayer")]
        public bool IsPlayer { get; set; }

        [JsonProperty("Interdicted")]
        public string Interdicted { get; set; }

        [JsonProperty("Faction")]
        public string Faction { get; set; }
    }
}
