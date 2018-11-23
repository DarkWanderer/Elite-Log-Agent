namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class EngineerProgress : LogEvent
    {
        [JsonProperty("Engineers")]
        public EngineerProgressRecord[] Engineers { get; set; }
    }
}
