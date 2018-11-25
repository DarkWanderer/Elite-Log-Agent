namespace DW.ELA.Interfaces.Events
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class CombatStatistics
    {
        [JsonProperty("Bounties_Claimed")]
        public long BountiesClaimed { get; set; }

        [JsonProperty("Bounty_Hunting_Profit")]
        public long BountyHuntingProfit { get; set; }

        [JsonProperty("Combat_Bonds")]
        public long CombatBonds { get; set; }

        [JsonProperty("Combat_Bond_Profits")]
        public long CombatBondProfits { get; set; }

        [JsonProperty("Assassinations")]
        public long Assassinations { get; set; }

        [JsonProperty("Assassination_Profits")]
        public long AssassinationProfits { get; set; }

        [JsonProperty("Highest_Single_Reward")]
        public long HighestSingleReward { get; set; }

        [JsonProperty("Skimmers_Killed")]
        public long SkimmersKilled { get; set; }
    }
}
