using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Powerplay : JournalEvent
    {
        [JsonProperty]
        public string Power { get; set; }

        [JsonProperty]
        public int Rank { get; set; }

        [JsonProperty]
        public int? Merits { get; set; }

        [JsonProperty]
        public int? Votes { get; set; }

        [JsonProperty]
        public int? TimePledged { get; set; }
    }
}
