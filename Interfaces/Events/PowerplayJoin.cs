using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class PowerplayJoin : JournalEvent
    {
        [JsonProperty]
        public string Power { get; set; }
    }
}
