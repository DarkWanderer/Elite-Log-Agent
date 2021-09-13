using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class ConflictInfo
    {
        [JsonProperty]
        public string WarType { get; set; }

        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public FactionConflictInfo Faction1 { get; set; }
        
        [JsonProperty]
        public FactionConflictInfo Faction2 { get; set; }
    }
}
