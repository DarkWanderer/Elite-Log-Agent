namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Friends : LogEvent
    {
        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}
