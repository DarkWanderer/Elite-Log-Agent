using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class CombatStatistics
    {
        [JsonProperty("Bounties_Claimed")]
        public double BountiesClaimed { get; set; }

        [JsonProperty("Bounty_Hunting_Profit")]
        public double BountyHuntingProfit { get; set; }

        [JsonProperty("Combat_Bonds")]
        public double CombatBonds { get; set; }

        [JsonProperty("Combat_Bond_Profits")]
        public double CombatBondProfits { get; set; }

        [JsonProperty("Assassinations")]
        public double Assassinations { get; set; }

        [JsonProperty("Assassination_Profits")]
        public double AssassinationProfits { get; set; }

        [JsonProperty("Highest_Single_Reward")]
        public double HighestSingleReward { get; set; }

        [JsonProperty("Skimmers_Killed")]
        public double SkimmersKilled { get; set; }
    }
}
