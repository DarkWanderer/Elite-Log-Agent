namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Friends : JournalEvent
    {
        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}
