using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class JoinACrew : LogEvent
    {
        [JsonProperty("Captain")]
        public string Captain { get; set; }
    }

    public class QuitACrew : LogEvent
    {
        [JsonProperty("Captain")]
        public string Captain { get; set; }
    }
}
