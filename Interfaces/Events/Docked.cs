namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public partial class Docked : LocationEventBase
    {
        [JsonProperty("DistFromStarLS")]
        public double DistFromStarLs { get; set; }

        [JsonProperty]
        public bool? ActiveFine { get; set; }
    }
}
