using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class PowerplayDefect : JournalEvent
    {
        [JsonProperty]
        public string FromPower { get; set; }

        [JsonProperty]
        public string ToPower { get; set; }
    }
}
