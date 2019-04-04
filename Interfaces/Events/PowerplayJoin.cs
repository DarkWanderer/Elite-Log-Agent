namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class PowerplayJoin : LogEvent
    {
        [JsonProperty]
        public string Power { get; set; }
    }
}
