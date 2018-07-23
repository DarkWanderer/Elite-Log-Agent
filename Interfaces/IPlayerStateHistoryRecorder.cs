using DW.ELA.Interfaces;
using System;

namespace DW.ELA.Interfaces
{
    public interface IPlayerStateHistoryRecorder : IObserver<LogEvent>
    {
        long? GetPlayerShipId(DateTime atTime);
        string GetPlayerShipType(DateTime atTime);
        string GetPlayerSystem(DateTime atTime);
        string GetPlayerStation(DateTime timestamp);
        bool GetPlayerCrewStatus(DateTime atTime);
    }
}