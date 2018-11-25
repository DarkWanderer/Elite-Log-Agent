namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Utility.Extensions;
    using MoreLinq;
    using NLog;

    public class PlayerStateRecorder : IPlayerStateHistoryRecorder
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly StateRecorder<ShipRecord> shipRecorder = new StateRecorder<ShipRecord>();
        private readonly StateRecorder<string> starSystemRecorder = new StateRecorder<string>();
        private readonly StateRecorder<string> stationRecorder = new StateRecorder<string>();
        private readonly StateRecorder<bool> crewRecorder = new StateRecorder<bool>();
        private readonly ConcurrentDictionary<string, double[]> systemCoordinates = new ConcurrentDictionary<string, double[]>();

        public string GetPlayerSystem(DateTime atTime) => starSystemRecorder.GetStateAt(atTime);

        public string GetPlayerStation(DateTime atTime) => stationRecorder.GetStateAt(atTime);

        public string GetPlayerShipType(DateTime atTime) => shipRecorder.GetStateAt(atTime)?.ShipType;

        public long? GetPlayerShipId(DateTime atTime) => shipRecorder.GetStateAt(atTime)?.ShipID;

        public bool GetPlayerIsInCrew(DateTime atTime) => crewRecorder.GetStateAt(atTime);

        public double[] GetStarPos(string systemName) => systemCoordinates.GetValueOrDefault(systemName);

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(LogEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            try
            {
                switch (@event)
                {
                    // Ship change events
                    case ShipyardSwap e: Process(e); break;
                    case LoadGame e: Process(e); break;
                    case Loadout e: Process(e); break;

                    // Location change events
                    case Location e: Process(e); break;
                    case FsdJump e: Process(e); break;
                    case Docked e: Process(e); break;
                    case SupercruiseEntry e: Process(e); break;
                    case Undocked e: Process(e); break;

                    // Crew status change events
                    case JoinACrew e: Process(e); break;
                    case QuitACrew e: Process(e); break;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in OnNext");
            }
        }

        private void Process(Undocked e) => stationRecorder.RecordState(null, e.Timestamp);

        private void Process(Loadout e) => ProcessShipIDEvent(e.ShipId, e.Ship, e.Timestamp);

        private void Process(LoadGame e) => ProcessShipIDEvent(e.ShipId, e.Ship, e.Timestamp);

        private void Process(ShipyardSwap e) => ProcessShipIDEvent(e.ShipId, e.ShipType, e.Timestamp);

        private void Process(Location e) => ProcessLocation(e.StarSystem, e.StarPos, e.Timestamp);

        private void Process(FsdJump e) => ProcessLocation(e.StarSystem, e.StarPos, e.Timestamp);

        private void Process(Docked e)
        {
            starSystemRecorder.RecordState(e.StarSystem, e.Timestamp);
            stationRecorder.RecordState(e.StationName, e.Timestamp);
        }

        private void Process(SupercruiseEntry e) => starSystemRecorder.RecordState(e.StarSystem, e.Timestamp);

        private void Process(QuitACrew e) => crewRecorder.RecordState(false, e.Timestamp);

        private void Process(JoinACrew e) => crewRecorder.RecordState(true, e.Timestamp);

        private void ProcessLocation(string starSystem, double[] starPos, DateTime timestamp)
        {
            if (systemCoordinates.TryAdd(starSystem, starPos))
                logger.Trace("Recorded location for {0}", starSystem);
            else
                logger.Trace("Location for {0} already recorded", starSystem);
            starSystemRecorder.RecordState(starSystem, timestamp);
        }

        private void ProcessShipIDEvent(long? shipId, string shipType, DateTime timestamp)
        {
            try
            {
                if (shipId == null ||
                    shipType == null ||
                    shipType.ToLower() == "testbuggy" ||
                    shipType.Contains("Fighter"))
                    return;

                shipRecorder.RecordState(new ShipRecord(shipId.Value, shipType), timestamp);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error decoding used ship reference");
            }
        }

        private class ShipRecord
        {
            public ShipRecord(long shipID, string shipType)
            {
                ShipID = shipID;
                ShipType = shipType;
            }

            public long ShipID { get; }

            public string ShipType { get; }

            public override bool Equals(object obj)
            {
                var record = obj as ShipRecord;
                return record != null &&
                       ShipID == record.ShipID &&
                       ShipType == record.ShipType;
            }

            public override int GetHashCode()
            {
                var hashCode = -1167275223;
                hashCode = hashCode * -1521134295 + ShipID.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShipType);
                return hashCode;
            }

            public override string ToString() => $"{ShipType}-{ShipID}";
        }

        private class StateRecorder<T>
        {
            private readonly SortedList<DateTime, T> stateRecording = new SortedList<DateTime, T>();

            public T GetStateAt(DateTime atTime)
            {
                try
                {
                    lock (stateRecording)
                        return stateRecording
                            .Where(l => l.Key <= atTime)
                            .DefaultIfEmpty()
                            .MaxBy(l => l.Key)
                            .FirstOrDefault()
                            .Value;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    return default(T);
                }
            }

            public void RecordState(T state, DateTime at)
            {
                try
                {
                    lock (stateRecording)
                    {
                        var current = GetStateAt(at);
                        if (Equals(current, state))
                            return;

                        stateRecording.Add(at, state);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }
    }
}
