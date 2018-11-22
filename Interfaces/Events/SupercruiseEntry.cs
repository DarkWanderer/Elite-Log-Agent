namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class SupercruiseEntry : LogEvent
    {
        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public long? SystemAddress { get; set; }
    }
}
