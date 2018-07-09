using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Interdicted : LogEvent
    {
        [JsonProperty("Submitted")]
        public bool Submitted { get; set; }

        [JsonProperty("Interdictor")]
        public string Interdictor { get; set; }

        [JsonProperty("Interdictor_Localised")]
        public string InterdictorLocalised { get; set; }

        [JsonProperty("IsPlayer")]
        public bool IsPlayer { get; set; }

        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("CombatRank")]
        public short? CombatRank { get; set; }
    }
}
