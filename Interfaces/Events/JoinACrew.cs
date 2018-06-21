using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
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
