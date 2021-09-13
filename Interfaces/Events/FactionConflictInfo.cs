using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class FactionConflictInfo
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Stake { get; set; }

        [JsonProperty]
        public int? WonDays { get; set; }
    }
}
