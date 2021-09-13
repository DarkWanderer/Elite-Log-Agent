using System;

namespace DW.ELA.Interfaces
{
    public interface IPlayerStateHistoryRecorder : IObserver<JournalEvent>
    {
        long? GetPlayerShipId(DateTime atTime);

        string GetPlayerShipType(DateTime atTime);

        string GetPlayerSystem(DateTime atTime);

        string GetPlayerStation(DateTime timestamp);

        bool GetPlayerIsInCrew(DateTime atTime);

        double[] GetStarPos(string systemName);

        ulong? GetSystemAddress(string systemName);
    }
}