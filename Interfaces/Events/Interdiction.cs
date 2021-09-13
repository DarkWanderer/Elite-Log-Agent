using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Interdiction : JournalEvent
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("IsPlayer")]
        public bool IsPlayer { get; set; }

        [JsonProperty("Interdicted")]
        public string Interdicted { get; set; }

        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("CombatRank")]
        public short? CombatRank { get; set; }

        [JsonProperty("Power")]
        public string Power { get; set; }
    }
}
