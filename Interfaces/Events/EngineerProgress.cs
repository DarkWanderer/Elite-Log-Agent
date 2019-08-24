namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class EngineerProgress : JournalEvent
    {
        [JsonProperty("Engineers")]
        public EngineerProgressRecord[] Engineers { get; set; }
    }
}
