namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class PowerplayLeave : LogEvent
    {
        [JsonProperty]
        public string Power { get; set; }
    }
}
