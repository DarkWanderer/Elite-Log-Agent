using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class CrewStatistics
    {
        [JsonProperty("NpcCrew_TotalWages")]
        public long NpcCrewTotalWages { get; set; }

        [JsonProperty("NpcCrew_Hired")]
        public long NpcCrewHired { get; set; }

        [JsonProperty("NpcCrew_Fired")]
        public long NpcCrewFired { get; set; }

        [JsonProperty("NpcCrew_Died")]
        public long? NpcCrewDied { get; set; }
    }
}
