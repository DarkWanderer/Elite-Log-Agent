namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class PowerplayJoin : JournalEvent
    {
        [JsonProperty]
        public string Power { get; set; }
    }
}
