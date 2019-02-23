namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class PowerplayDefect : LogEvent
    {
        [JsonProperty]
        public string FromPower { get; set; }

        [JsonProperty]
        public string ToPower { get; set; }
    }
}
