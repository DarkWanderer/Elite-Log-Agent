using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Friends : JournalEvent
    {
        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}
