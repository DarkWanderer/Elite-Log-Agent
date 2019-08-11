namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class CommunityGoal : JournalEvent
    {
        [JsonProperty("CurrentGoals")]
        public CommunityGoalRecord[] CurrentGoals { get; set; }
    }
}
