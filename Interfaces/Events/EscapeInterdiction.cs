namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class EscapeInterdiction : JournalEvent
    {
        [JsonProperty("Interdictor")]
        public string Interdictor { get; set; }

        [JsonProperty("Interdictor_Localised")]
        public string InterdictorLocalised { get; set; }

        [JsonProperty("IsPlayer")]
        public bool IsPlayer { get; set; }
    }
}
