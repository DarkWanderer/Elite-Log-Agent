namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class JoinACrew : LogEvent
    {
        [JsonProperty("Captain")]
        public string Captain { get; set; }
    }
}
