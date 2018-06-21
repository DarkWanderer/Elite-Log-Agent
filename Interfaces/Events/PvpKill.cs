using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class PvpKill : LogEvent
    {

        [JsonProperty("Victim")]
        public string Victim { get; set; }

        [JsonProperty("CombatRank")]
        public long CombatRank { get; set; }
    }
}
