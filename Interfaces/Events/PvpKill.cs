namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class PvpKill : JournalEvent
    {
        [JsonProperty("Victim")]
        public string Victim { get; set; }

        [JsonProperty("CombatRank")]
        public long CombatRank { get; set; }
    }
}
