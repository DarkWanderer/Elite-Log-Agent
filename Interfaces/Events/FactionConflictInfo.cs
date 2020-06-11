namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

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
