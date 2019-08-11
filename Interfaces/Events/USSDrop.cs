namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class USSDrop : JournalEvent
    {
        [JsonProperty("USSType")]
        public string UssType { get; set; }

        [JsonProperty("USSType_Localised")]
        public string UssTypeLocalised { get; set; }

        [JsonProperty("USSThreat")]
        public int UssThreat { get; set; }
    }
}
