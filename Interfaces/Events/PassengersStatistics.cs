using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class PassengersStatistics
    {
        [JsonProperty("Passengers_Missions_Accepted")]
        public long? PassengersMissionsAccepted { get; set; }

        [JsonProperty("Passengers_Missions_Disgruntled")]
        public long? PassengersMissionsDisgruntled { get; set; }

        [JsonProperty("Passengers_Missions_Bulk")]
        public long? PassengersMissionsBulk { get; set; }

        [JsonProperty("Passengers_Missions_VIP")]
        public long? PassengersMissionsVip { get; set; }

        [JsonProperty("Passengers_Missions_Delivered")]
        public long? PassengersMissionsDelivered { get; set; }

        [JsonProperty("Passengers_Missions_Ejected")]
        public long? PassengersMissionsEjected { get; set; }
    }
}
