using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class EscapeInterdiction : LogEvent
    {
        [JsonProperty("Interdictor")]
        public string Interdictor { get; set; }

        [JsonProperty("IsPlayer")]
        public bool IsPlayer { get; set; }
    }
}
