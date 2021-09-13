using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Events;
using DW.ELA.Utility.Extensions;
using MoreLinq;
using NLog;
using NLog.Fluent;

namespace DW.ELA.Controller
{
    public class PlayerStateRecorder : IPlayerStateHistoryRecorder
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly StateRecorder<ShipRecord> shipRecorder = new();
        private readonly StateRecorder<string> starSystemRecorder = new();
        private readonly StateRecorder<string> stationRecorder = new();
        private readonly StateRecorder<bool> crewRecorder = new();
        private readonly ConcurrentDictionary<string, double[]> systemCoordinates = new();
        private readonly ConcurrentDictionary<string, ulong> systemAddresses = new();

        public string GetPlayerSystem(DateTime atTime) => starSystemRecorder.GetStateAt(atTime);

        public string GetPlayerStation(DateTime atTime) => stationRecorder.GetStateAt(atTime);

        public string GetPlayerShipType(DateTime atTime) => shipRecorder.GetStateAt(atTime)?.ShipType;

        public long? GetPlayerShipId(DateTime atTime) => shipRecorder.GetStateAt(atTime)?.ShipID;

        public bool GetPlayerIsInCrew(DateTime atTime) => crewRecorder.GetStateAt(atTime);

        public double[] GetStarPos(string systemName) => systemCoordinates.GetValueOrDefault(systemName);

        public ulong? GetSystemAddress(string systemName) => systemAddresses.ContainsKey(systemName) ? systemAddresses[systemName] : (ulong?)null;

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(JournalEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            try
            {
                dynamic e = @event.Raw;

                if (e.SystemAddress != null && e.StarSystem != null)
                {
                    if (systemAddresses.TryAdd((string)e.StarSystem, (ulong)e.SystemAddress))
                    {
                        Log.Info()
                            .Message("Got SystemAddress")
                            .Property("StarSystem", e.StarSystem)
                            .Property("SystemAddress", e.SystemAddress)
                            .Write();
                    }
                }

                if (e.StarSystem != null)
                {
                    starSystemRecorder.RecordState((string)e.StarSystem, @event.Timestamp);
                    Log.Info()
                        .Message("Recorded new location")
                        .Property("StarSystem", e.StarSystem)
                        .Write();
                }

                if (e.StationName != null)
                    stationRecorder.RecordState((string)e.StationName, @event.Timestamp);

                if (e.Ship != null && e.ShipID != null)
                {
                    ProcessShipIDEvent((long?)e.ShipID, (string)e.Ship, @event.Timestamp);
                    Log.Info()
                        .Message("Ship switch event")
                        .Property("ShipID", e.ShipID)
                        .Property("Ship", e.Ship)
                        .Write();
                }

                // Special cases
                switch (@event)
                {
                    case Undocked ud:
                        stationRecorder.RecordState(null, @event.Timestamp);
                        break;
                    // Crew status change events
                    case JoinACrew jc:
                        crewRecorder.RecordState(true, @event.Timestamp);
                        break;
                    case QuitACrew lc:
                        crewRecorder.RecordState(false, @event.Timestamp);
                        break;
                    case ShipyardSwap ss:
                        ProcessShipIDEvent(ss.ShipId, ss.ShipType, @event.Timestamp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void ProcessShipIDEvent(long? shipId, string shipType, DateTime timestamp)
        {
            try
            {
                if (shipId != null && shipType != null && shipType.ToLower() != "testbuggy" && !shipType.Contains("Fighter"))
                {
                    shipRecorder.RecordState(new ShipRecord(shipId.Value, shipType), timestamp);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error decoding used ship reference");
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

            public override bool Equals(object obj) => obj is ShipRecord record && ShipID == record.ShipID && ShipType == record.ShipType;

            public override int GetHashCode() => HashCode.Combine(ShipID, ShipType);

            public override string ToString() => $"{ShipType}-{ShipID}";
        }

        private class StateRecorder<T>
        {
            private readonly SortedList<DateTime, T> stateRecording = new();

            public T GetStateAt(DateTime atTime)
            {
                try
                {
                    lock (stateRecording)
                    {
                        return stateRecording
                            .Where(l => l.Key <= atTime)
                            .DefaultIfEmpty()
                            .MaxBy(l => l.Key)
                            .FirstOrDefault()
                            .Value;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return default;
                }
            }

            public void RecordState(T state, DateTime at)
            {
                try
                {
                    lock (stateRecording)
                    {
                        var current = GetStateAt(at);
                        if (!Equals(current, state))
                            stateRecording.Add(at, state);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
