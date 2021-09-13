using System;
using System.Collections.Generic;
using System.Linq;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Events;
using DW.ELA.Plugin.Inara.Model;
using DW.ELA.Utility.Extensions;
using DW.ELA.Utility.Json;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;

namespace DW.ELA.Plugin.Inara
{
    public class InaraEventConverter : IEventConverter<ApiInputEvent>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;

        public InaraEventConverter(IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
        }

        public IEnumerable<ApiInputEvent> Convert(JournalEvent @event)
        {
            try
            {
                string eventName = @event.Event;
                switch (@event)
                {
                    // Generic
                    case LoadGame e:
                        return ConvertEvent(e);
                    case Statistics e:
                        return ConvertEvent(e);
                    case Location e:
                        return ConvertMinorFactionReputation(e.Timestamp, e.Factions);
                    case Friends e:
                        return ConvertEvent(e);

                    // Inventory
                    case Materials e:
                        return ConvertEvent(e);
                    case MaterialCollected e:
                        return ConvertEvent(e);
                    case MaterialTrade e:
                        return ConvertEvent(e);
                    case StoredModules e:
                        return ConvertEvent(e);
                    case Cargo e:
                        return ConvertEvent(e);

                    // Shipyard
                    case ShipyardSell e:
                        return ConvertEvent(e);
                    case ShipyardTransfer e:
                        return ConvertEvent(e);
                    case StoredShips e:
                        return ConvertEvent(e);

                    // Travel
                    case FsdJump e:
                        return ConvertEvent(e);
                    case Docked e:
                        return ConvertEvent(e);

                    // Ranks/reputation
                    case EngineerProgress e:
                        return ConvertEvent(e);
                    case Rank e:
                        return ConvertEvent(e);
                    case Progress e:
                        return ConvertEvent(e);
                    case Reputation e:
                        return ConvertEvent(e);

                    // Powerplay pledge
                    case Powerplay e:
                        return ConvertEvent(e);
                    case PowerplayLeave e:
                        return ConvertEvent(e);
                    case PowerplayJoin e:
                        return ConvertEvent(e);
                    case PowerplayDefect e:
                        return ConvertEvent(e);

                    // Combat
                    case Interdicted e:
                        return ConvertEvent(e);
                    case Interdiction e:
                        return ConvertEvent(e);
                    case EscapeInterdiction e:
                        return ConvertEvent(e);
                    case PvpKill e:
                        return ConvertEvent(e);

                    // Ship events
                    case Loadout e:
                        return ConvertEvent(e);
                    case ShipyardSwap e:
                        return ConvertEvent(e);

                    // Missions
                    case MissionAccepted e:
                        return ConvertEvent(e);
                    case MissionCompleted e:
                        return ConvertEvent(e);
                    case MissionAbandoned e:
                        return ConvertEvent(e);
                    case MissionFailed e:
                        return ConvertEvent(e);

                    // Community goals
                    case CommunityGoal e:
                        return ConvertEvent(e);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in OnNext");
            }
            return Enumerable.Empty<ApiInputEvent>();
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(PowerplayDefect e) => GetPowerplayRankEvent(e.Timestamp, e.ToPower, 1);

        private IEnumerable<ApiInputEvent> ConvertEvent(PowerplayJoin e) => GetPowerplayRankEvent(e.Timestamp, e.Power, 1);

        private IEnumerable<ApiInputEvent> ConvertEvent(PowerplayLeave e) => GetPowerplayRankEvent(e.Timestamp, e.Power, 0);

        private IEnumerable<ApiInputEvent> ConvertEvent(Powerplay e) => GetPowerplayRankEvent(e.Timestamp, e.Power, Math.Max(e.Rank, 1));

        private IEnumerable<ApiInputEvent> GetPowerplayRankEvent(DateTime timestamp, string power, int? rank = null)
        {
            yield return new ApiInputEvent("setCommanderRankPower")
            {
                Timestamp = timestamp,
                EventData = new Dictionary<string, object>
                {
                    { "powerName", power },
                    { "rankValue", rank ?? 0 }
                }
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Cargo e)
        {
            yield return new ApiInputEvent("setCommanderInventoryCargo")
            {
                Timestamp = e.Timestamp,
                EventData = (e.Inventory ?? Enumerable.Empty<InventoryRecord>())
                    .Select(cr => new { itemName = cr.Name, itemCount = cr.Count, isStolen = cr.Stolen })
                    .ToArray()
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Friends e)
        {
            if (e.Status == "Lost")
            {
                yield return new ApiInputEvent("delCommanderFriend")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "commanderName", e.Name },
                        { "gamePlatform", "PC" }
                    }
                };
            }
            else if (e.Status == "Added" || e.Status == "Online")
            {
                yield return new ApiInputEvent("addCommanderFriend")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "commanderName", e.Name },
                        { "gamePlatform", "PC" }
                    }
                };
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(StoredShips e)
        {
            foreach (var ship in e.ShipsHere)
            {
                var @event = new ApiInputEvent("setCommanderShip")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "shipType", ship.ShipType },
                        { "shipGameID", ship.ShipId },
                        { "marketID", e.MarketId },
                        { "starsystemName", e.StarSystem },
                        { "stationName", e.StationName },
                        { "isCurrentShip", false },
                        { "isHot", ship.Hot }
                    }
                };
                yield return @event;
            }

            foreach (var ship in e.ShipsRemote)
            {
                var @event = new ApiInputEvent("setCommanderShip")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "shipType", ship.ShipType },
                        { "shipGameID", ship.ShipId },
                        { "marketID", ship.ShipMarketId },
                        { "starsystemName", ship.StarSystem },
                        { "isCurrentShip", false },
                        { "isHot", ship.Hot },
                    }
                };
                yield return @event;
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(ShipyardTransfer e)
        {
            yield return new ApiInputEvent("setCommanderShipTransfer")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType", e.ShipType },
                    { "shipGameID", e.ShipId },
                    { "starsystemName", e.System },
                    { "stationName", playerStateRecorder.GetPlayerStation(e.Timestamp) },
                    { "transferTime", e.TransferTime },
                    { "marketID", e.MarketId }
                }
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Progress e)
        {
            var ranks = new Dictionary<string, float>()
            {
                { nameof(e.Combat), System.Convert.ToSingle(e.Combat) / 100.0f },
                { nameof(e.Cqc), System.Convert.ToSingle(e.Cqc) / 100.0f },
                { nameof(e.Empire), System.Convert.ToSingle(e.Empire) / 100.0f },
                { nameof(e.Explore), System.Convert.ToSingle(e.Explore) / 100.0f },
                { nameof(e.Federation), System.Convert.ToSingle(e.Federation) / 100.0f },
                { nameof(e.Trade), System.Convert.ToSingle(e.Trade) / 100.0f }
            };

            foreach (var rank in ranks)
            {
                var @event = new ApiInputEvent("setCommanderRankPilot")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "rankName", rank.Key.ToLower() },
                        { "rankProgress", rank.Value }
                    }
                };
                yield return @event;
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Rank e)
        {
            var ranks = new Dictionary<string, int>();
            ranks.AddIfNotNull(nameof(e.Combat), e.Combat);
            ranks.AddIfNotNull(nameof(e.Cqc), e.Cqc);
            ranks.AddIfNotNull(nameof(e.Empire), e.Empire);
            ranks.AddIfNotNull(nameof(e.Explore), e.Explore);
            ranks.AddIfNotNull(nameof(e.Federation), e.Federation);
            ranks.AddIfNotNull(nameof(e.Trade), e.Trade);

            foreach (var rank in ranks)
            {
                var @event = new ApiInputEvent("setCommanderRankPilot")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "rankName", rank.Key.ToLower() },
                        { "rankValue", rank.Value }
                    }
                };
                yield return @event;
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(MaterialTrade e)
        {
            var @event = new ApiInputEvent("addCommanderInventoryMaterialsItem")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "itemName", e.Received.Material },
                    { "itemCount", e.Received.Quantity }
                }
            };
            yield return @event;

            @event = new ApiInputEvent("delCommanderInventoryMaterialsItem")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "itemName", e.Paid.Material },
                    { "itemCount", e.Paid.Quantity }
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(CommunityGoal e)
        {
            foreach (var goal in e.CurrentGoals)
            {
                yield return new ApiInputEvent("setCommunityGoal")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "communitygoalGameID", goal.Cgid },
                        { "communitygoalName", goal.Title },
                        { "starsystemName", goal.SystemName },
                        { "stationName", goal.MarketName },
                        { "goalExpiry", goal.Expiry },
                        { "tierReached", goal.TierReached },
                        { "tierMax", goal.TopTier?.Name },
                        { "topRankSize", goal.TopRankSize },
                        { "isCompleted", goal.IsComplete },
                        { "contributorsNum", goal.NumContributors },
                        { "contributionsTotal", goal.CurrentTotal },
                        { "completionBonus", goal.TopTier?.Bonus },
                    }
                };

                if (goal.PlayerContribution == 0)
                    continue; // INARA rejects setCommanderCommunityGoalProgress with 0 contribution

                yield return new ApiInputEvent("setCommanderCommunityGoalProgress")
                {
                    Timestamp = e.Timestamp,
                    EventData = new Dictionary<string, object>()
                    {
                        { "communitygoalGameID", goal.Cgid },
                        { "contribution", goal.PlayerContribution },
                        { "percentileBand", goal.PlayerPercentileBand },
                        { "percentileBandReward", goal.Bonus },
                        { "isTopRank", goal.PlayerInTopRank }
                    }
                };
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Reputation e)
        {
            yield return new ApiInputEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Empire) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Empire) / 100.0f },
                }
            };
            yield return new ApiInputEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Alliance) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Alliance) / 100.0f },
                }
            };
            yield return new ApiInputEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Federation) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Federation) / 100.0f },
                }
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(ShipyardSwap e)
        {
            // Send event for old ship indicating it's location
            var @event = new ApiInputEvent("setCommanderShip")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType", e.StoreOldShip },
                    { "shipGameID", e.StoreShipId },
                    { "marketID", e.MarketId },
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "stationName", playerStateRecorder.GetPlayerStation(e.Timestamp) },
                    { "isCurrentShip", false }
                }
            };
            yield return @event;

            // and for new, indicating that it's now current
            @event = new ApiInputEvent("setCommanderShip")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType", e.ShipType },
                    { "shipGameID", e.ShipId },
                    { "marketID", e.MarketId },
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "stationName", playerStateRecorder.GetPlayerStation(e.Timestamp) },
                    { "isCurrentShip", true }
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(ShipyardSell e)
        {
            var @event = new ApiInputEvent("delCommanderShip")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType", e.ShipType },
                    { "shipGameID", e.SellShipId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(StoredModules e)
        {
            var @event = new ApiInputEvent("setCommanderStorageModules")
            {
                Timestamp = e.Timestamp,
                EventData = e.Items.Select(ConvertStoredItem).ToArray()
            };
            yield return @event;
        }

        private Dictionary<string, object> ConvertStoredItem(StoredItem item)
        {
            var result = new Dictionary<string, object>()
            {
                { "itemName", item.Name },
                { "itemValue", item.BuyPrice },
                { "isHot", item.Hot },
                { "starsystemName", item.StarSystem },
                { "marketID", item.MarketId },
            };
            if (!string.IsNullOrEmpty(item.EngineerModifications))
            {
                var value = new Dictionary<string, object>()
                {
                    { "blueprintName", item.EngineerModifications },
                    { "blueprintLevel", item.Level },
                    { "blueprintQuality", item.Quality },
                };
                result.Add("engineering", value);
            }

            return result;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(MissionFailed e)
        {
            var @event = new ApiInputEvent("setCommanderMissionFailed")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(MissionAbandoned e)
        {
            var @event = new ApiInputEvent("setCommanderMissionAbandoned")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(MissionCompleted e)
        {
            var data = new Dictionary<string, object>()
            {
                    { "missionGameID", e.MissionId },
                    { "missionName", e.Name },
                    { "donationCredits", e.Donation },
                    { "rewardCredits", e.Reward },
            };

            data.AddIfNotNull("rewardCommodities", ConvertCommodityReward(e.CommodityReward));
            data.AddIfNotNull("rewardMaterials", ConvertMaterialReward(e.MaterialsReward));
            data.AddIfNotNull("minorfactionEffects", ConvertMissionFactionEffects(e.FactionEffects));

            var @event = new ApiInputEvent("setCommanderMissionCompleted")
            {
                Timestamp = e.Timestamp,
                EventData = data
            };
            yield return @event;

            if (e.MaterialsReward != null)
            {
                foreach (var cr in e.MaterialsReward)
                {
                    @event = new ApiInputEvent("addCommanderInventoryMaterialsItem")
                    {
                        Timestamp = e.Timestamp,
                        EventData = new Dictionary<string, object>()
                        {
                            { "itemName", cr.Name },
                            { "itemCount", cr.Count }
                        }
                    };
                    yield return @event;
                }
            }
        }

        private object[] ConvertMissionFactionEffects(FactionEffect[] factionEffects)
        {
            if (factionEffects == null)
                return null;

            var effects = from fe in factionEffects
                          from inf in fe.Influence
                          select new { minorfactionName = fe.Faction, influenceGain = inf.InfluenceValue, reputationGain = fe.Reputation };

            return effects.ToArray();
        }

        private object[] ConvertCommodityReward(CommodityReward[] commodityRewards) => commodityRewards?.Select(cr => new { itemName = cr.Name, itemCount = cr.Count })?.ToArray<object>();

        private object[] ConvertMaterialReward(MaterialsReward[] materialsRewards) => materialsRewards?.Select(cr => new { itemName = cr.Name, itemCount = cr.Count })?.ToArray<object>();

        private IEnumerable<ApiInputEvent> ConvertEvent(MissionAccepted e)
        {
            var data = new Dictionary<string, object>()
                {
                    { "missionName", e.Name },
                    { "missionGameID", e.MissionId },
                    { "missionExpiry", e.Expiry },
                    { "influenceGain", e.Influence },
                    { "reputationGain", e.Reputation },
                    { "starsystemNameOrigin", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "stationNameOrigin", playerStateRecorder.GetPlayerStation(e.Timestamp) },
                    { "minorfactionNameOrigin", e.Faction },
                    { "starsystemNameTarget", e.DestinationSystem },
                    { "stationNameTarget", e.DestinationStation },
                    { "minorfactionNameTarget", e.TargetFaction }
                };

            data.AddIfNotNull("commodityName", e.Commodity);
            data.AddIfNotNull("commodityCount", e.Count);
            data.AddIfNotNull("targetName", e.Target);
            data.AddIfNotNull("targetType", e.TargetType);
            data.AddIfNotNull("killCount", e.KillCount);

            data.AddIfNotNull("passengerType", e.PassengerType);
            data.AddIfNotNull("passengerCount", e.PassengerCount);
            data.AddIfNotNull("passengerIsVIP", e.PassengerVIPs);
            data.AddIfNotNull("passengerIsWanted", e.PassengerWanted);

            var @event = new ApiInputEvent("addCommanderMission")
            {
                Timestamp = e.Timestamp,
                EventData = data
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(MaterialCollected e)
        {
            var @event = new ApiInputEvent("addCommanderInventoryMaterialsItem")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "itemName", e.Name },
                    { "itemCount", e.Count }
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Loadout e)
        {
            if (e.Ship == null || e.Ship.Contains("Fighter"))
                yield break;

            var shipEvent = new ApiInputEvent("setCommanderShip")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType", e.Ship },
                    { "shipGameID", e.ShipId },
                    { "shipName", e.ShipName },
                    { "shipIdent", e.ShipIdent },
                    { "isCurrentShip", true },
                    { "isMainShip", false },
                    { "isHot", false }, // TODO
                    { "shipHullValue", e.HullValue },
                    { "shipModulesValue", e.ModulesValue },
                    { "shipRebuyCost", e.Rebuy },
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) }
                }
            };

            yield return shipEvent;

            var loadoutEvent = new ApiInputEvent("setCommanderShipLoadout")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>
                {
                    { "shipType", e.Ship },
                    { "shipGameID", e.ShipId },
                    { "shipLoadout", e.Modules.Select(ConvertModule).ToArray() }
                }
            };

            yield return loadoutEvent;
        }

        private JObject ConvertModule(Module module)
        {
            var item = new JObject
            {
                ["slotName"] = module.Slot,
                ["itemName"] = module.Item,
                ["itemHealth"] = module.Health,
                ["isOn"] = module.On,
                ["isHot"] = false, // TODO!
                ["itemPriority"] = module.Priority,
            };
            item.AddIfNotNull("itemAmmoClip", module.AmmoInClip);
            item.AddIfNotNull("itemAmmoHopper", module.AmmoInHopper);
            item.AddIfNotNull("itemValue", module.Value);
            if (module.Engineering != null)
                item["engineering"] = ConvertEngineering(module.Engineering);
            return item;
        }

        private JObject ConvertEngineering(ModuleEngineering eng)
        {
            var item = new JObject
            {
                ["blueprintName"] = eng.BlueprintName,
                ["blueprintLevel"] = eng.Level,
                ["blueprintQuality"] = eng.Quality,
                ["modifiers"] = new JArray(eng.Modifiers.Select(ConvertModifier).ToArray())
            };
            item.AddIfNotNull("experimentalEffect", eng.ExperimentalEffect);
            return item;
        }

        private JObject ConvertModifier(Modifier mod)
        {
            var item = new JObject()
            {
                ["name"] = mod.Label,
                ["value"] = mod.Value,
                ["originalValue"] = mod.OriginalValue,
                ["lessIsGood"] = mod.LessIsGood > 0
            };
            return item;
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(EscapeInterdiction e)
        {
            yield return new ApiInputEvent("addCommanderCombatInterdictionEscape")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Interdiction e)
        {
            yield return new ApiInputEvent("addCommanderCombatInterdiction")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdicted ?? e.Faction },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Interdicted e)
        {
            yield return new ApiInputEvent("addCommanderCombatInterdicted")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer },
                    { "isSubmit", e.Submitted }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(PvpKill @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiInputEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(timestamp) },
                    { "opponentName", @event.Victim },
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Statistics @event)
        {
            yield return new ApiInputEvent("setCommanderGameStatistics")
            {
                EventData = @event.Raw,
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Docked @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiInputEvent("addCommanderTravelDock")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", @event.StarSystem },
                    { "stationName", @event.StationName },
                    { "marketID", @event.MarketId },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(FsdJump @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiInputEvent("addCommanderTravelFSDJump")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", @event.StarSystem },
                    { "jumpDistance", @event.JumpDist },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };

            foreach (var reputationEvent in ConvertMinorFactionReputation(@event.Timestamp, @event.Factions))
                yield return reputationEvent;
        }

        private IEnumerable<ApiInputEvent> ConvertMinorFactionReputation(DateTime timestamp, Faction[] factions)
        {
            if (factions == null)
                yield break;

            foreach (var faction in factions.Where(f => f.MyReputation.HasValue))
            {
                yield return new ApiInputEvent("setCommanderReputationMinorFaction")
                {
                    EventData = new Dictionary<string, object>
                    {
                        { "minorfactionName", faction.Name },
                        { "minorfactionReputation", faction.MyReputation / 100.0 },
                    },
                    Timestamp = timestamp
                };
            }
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(Materials @event)
        {
            var materialCounts = @event.RawMats.Concat(@event.Manufactured).Concat(@event.Encoded)
                .ToDictionary(mat => mat.Name, mat => mat.Count);

            yield return new ApiInputEvent("setCommanderInventoryMaterials")
            {
                EventData = materialCounts
                    .Select(kvp => new { itemName = kvp.Key, itemCount = kvp.Value })
                    .OrderBy(x => x.itemName)
                    .ToArray(),
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(LoadGame @event)
        {
            yield return new ApiInputEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object>
                {
                    { "commanderCredits", @event.Credits },
                    { "commanderLoan", @event.Loan }
                },
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiInputEvent> ConvertEvent(EngineerProgress @event)
        {
            if (@event.Engineers != null)
            {
                foreach (var engineer in @event.Engineers)
                {
                    yield return new ApiInputEvent("setCommanderRankEngineer")
                    {
                        EventData = new Dictionary<string, object>
                        {
                            { "engineerName", engineer.EngineerName },
                            { "rankStage", engineer.Progress },
                            { "rankValue", engineer.Rank }
                        },
                        Timestamp = @event.Timestamp
                    };
                }
            }
        }
    }
}
