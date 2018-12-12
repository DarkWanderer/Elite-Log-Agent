namespace DW.ELA.Plugin.Inara
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Plugin.Inara.Model;
    using DW.ELA.Utility.Json;
    using MoreLinq;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class InaraEventConverter : IEventConverter<ApiEvent>
    {
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public InaraEventConverter(IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
        }

        public IEnumerable<ApiEvent> Convert(LogEvent @event)
        {
            try
            {
                string eventName = @event.Event;
                switch (@event)
                {
                    // Generic
                    case LoadGame e: return ConvertEvent(e);
                    case Statistics e: return ConvertEvent(e);

                    // Inventory
                    case Materials e: return ConvertEvent(e);
                    case MaterialCollected e: return ConvertEvent(e);
                    case MaterialTrade e: return ConvertEvent(e);
                    case StoredModules e: return ConvertEvent(e);

                    // Shipyard
                    case ShipyardSell e: return ConvertEvent(e);
                    case ShipyardTransfer e: return ConvertEvent(e);
                    case StoredShips e: return ConvertEvent(e);

                    // Travel
                    case FsdJump e: return ConvertEvent(e);
                    case Docked e: return ConvertEvent(e);

                    // Ranks/reputation
                    case EngineerProgress e: return ConvertEvent(e);
                    case Rank e: return ConvertEvent(e);
                    case Progress e: return ConvertEvent(e);
                    case Reputation e: return ConvertEvent(e);

                    // Combat
                    case Interdicted e: return ConvertEvent(e);
                    case Interdiction e: return ConvertEvent(e);
                    case EscapeInterdiction e: return ConvertEvent(e);
                    case PvpKill e: return ConvertEvent(e);

                    // Ship events
                    case Loadout e: return ConvertEvent(e);
                    case ShipyardSwap e: return ConvertEvent(e);

                    // Missions
                    case MissionAccepted e: return ConvertEvent(e);
                    case MissionCompleted e: return ConvertEvent(e);
                    case MissionAbandoned e: return ConvertEvent(e);
                    case MissionFailed e: return ConvertEvent(e);

                    // Community goals
                    case CommunityGoal e: return ConvertEvent(e);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in OnNext");
            }
            return Enumerable.Empty<ApiEvent>();
        }

        private IEnumerable<ApiEvent> ConvertEvent(StoredShips e)
        {
            foreach (var ship in e.ShipsHere)
            {
                var @event = new ApiEvent("setCommanderShip")
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
                var @event = new ApiEvent("setCommanderShip")
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

        private IEnumerable<ApiEvent> ConvertEvent(ShipyardTransfer e)
        {
            yield return new ApiEvent("setCommanderShipTransfer")
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

        private IEnumerable<ApiEvent> ConvertEvent(Progress e)
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
                var @event = new ApiEvent("setCommanderRankPilot")
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

        private IEnumerable<ApiEvent> ConvertEvent(Rank e)
        {
            var ranks = new Dictionary<string, int>()
            {
                { nameof(e.Combat), e.Combat },
                { nameof(e.Cqc), e.Cqc },
                { nameof(e.Empire), e.Empire },
                { nameof(e.Explore), e.Explore },
                { nameof(e.Federation), e.Federation },
                { nameof(e.Trade), e.Trade }
            };

            foreach (var rank in ranks)
            {
                var @event = new ApiEvent("setCommanderRankPilot")
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

        private IEnumerable<ApiEvent> ConvertEvent(MaterialTrade e)
        {
            var @event = new ApiEvent("addCommanderInventoryMaterialsItem")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "itemName", e.Received.Material },
                    { "itemCount", e.Received.Quantity }
                }
            };
            yield return @event;

            @event = new ApiEvent("delCommanderInventoryMaterialsItem")
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

        private IEnumerable<ApiEvent> ConvertEvent(CommunityGoal e)
        {
            foreach (var goal in e.CurrentGoals)
            {
                yield return new ApiEvent("setCommunityGoal")
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

                yield return new ApiEvent("setCommanderCommunityGoalProgress")
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

        private IEnumerable<ApiEvent> ConvertEvent(Reputation e)
        {
            yield return new ApiEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Empire) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Empire) / 100.0f },
                }
            };
            yield return new ApiEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Alliance) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Alliance) / 100.0f },
                }
            };
            yield return new ApiEvent("setCommanderReputationMajorFaction")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "majorfactionName", nameof(e.Federation) },
                    { "majorfactionReputation", System.Convert.ToSingle(e.Federation) / 100.0f },
                }
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(ShipyardSwap e)
        {
            // Send event for old ship indicating it's location
            var @event = new ApiEvent("setCommanderShip")
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
            @event = new ApiEvent("setCommanderShip")
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

        private IEnumerable<ApiEvent> ConvertEvent(ShipyardSell e)
        {
            var @event = new ApiEvent("delCommanderShip")
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

        private IEnumerable<ApiEvent> ConvertEvent(StoredModules e)
        {
            var @event = new ApiEvent("setCommanderStorageModules")
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

        private IEnumerable<ApiEvent> ConvertEvent(MissionFailed e)
        {
            var @event = new ApiEvent("setCommanderMissionFailed")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionAbandoned e)
        {
            var @event = new ApiEvent("setCommanderMissionAbandoned")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionCompleted e)
        {
            var @event = new ApiEvent("setCommanderMissionCompleted")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "missionGameID", e.MissionId },
                    { "missionName", e.Name },
                    { "donationCredits", e.Donation },
                    { "rewardCredits", e.Reward },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionAccepted e)
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

            if (!string.IsNullOrWhiteSpace(e.Commodity))
            {
                data.Add("commodityName", e.Commodity);
                data.Add("commodityCount", e.Count);
            }

            if (!string.IsNullOrWhiteSpace(e.Target))
                data.Add("targetName", e.Target);

            if (!string.IsNullOrWhiteSpace(e.TargetType))
                data.Add("targetType", e.TargetType);

            if (e.KillCount.HasValue)
                data.Add("killCount", e.KillCount);

            var @event = new ApiEvent("addCommanderMission")
            {
                Timestamp = e.Timestamp,
                EventData = data
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MaterialCollected e)
        {
            var @event = new ApiEvent("addCommanderInventoryMaterialsItem")
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

        private IEnumerable<ApiEvent> ConvertEvent(Loadout e)
        {
            if (e.Ship == null || e.Ship.Contains("Fighter"))
                yield break;

            var shipEvent = new ApiEvent("setCommanderShip")
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

            var loadoutEvent = new ApiEvent("setCommanderShipLoadout")
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

        private IEnumerable<ApiEvent> ConvertEvent(EscapeInterdiction e)
        {
            yield return new ApiEvent("addCommanderCombatInterdictionEscape")
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

        private IEnumerable<ApiEvent> ConvertEvent(Interdiction e)
        {
            yield return new ApiEvent("addCommanderCombatInterdiction")
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

        private IEnumerable<ApiEvent> ConvertEvent(Interdicted e)
        {
            yield return new ApiEvent("addCommanderCombatInterdicted")
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

        private IEnumerable<ApiEvent> ConvertEvent(PvpKill @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object>
                {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(timestamp) },
                    { "opponentName", @event.Victim },
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Statistics @event)
        {
            yield return new ApiEvent("setCommanderGameStatistics")
            {
                EventData = @event.Raw,
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Docked @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderTravelDock")
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

        private IEnumerable<ApiEvent> ConvertEvent(FsdJump @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderTravelFSDJump")
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
        }

        private IEnumerable<ApiEvent> ConvertEvent(Materials @event)
        {
            var materialCounts = @event.RawMats.Concat(@event.Manufactured).Concat(@event.Encoded)
                .ToDictionary(mat => mat.Name, mat => mat.Count);

            yield return new ApiEvent("setCommanderInventoryMaterials")
            {
                EventData = materialCounts
                    .Select(kvp => new { itemName = kvp.Key, itemCount = kvp.Value })
                    .OrderBy(x => x.itemName)
                    .ToArray(),
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(LoadGame @event)
        {
            yield return new ApiEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object>
                {
                    { "commanderCredits", @event.Credits },
                    { "commanderLoan", @event.Loan }
                },
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(EngineerProgress @event)
        {
            if (@event.Engineers != null)
            {
                foreach (var engineer in @event.Engineers)
                {
                    yield return new ApiEvent("setCommanderRankEngineer")
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
