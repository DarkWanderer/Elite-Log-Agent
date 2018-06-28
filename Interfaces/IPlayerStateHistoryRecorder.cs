using DW.ELA.LogModel;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IPlayerStateHistoryRecorder : IObserver<LogEvent>
    {
        long? GetPlayerShipId(DateTime atTime);
        string GetPlayerShipType(DateTime atTime);
        string GetPlayerSystem(DateTime atTime);
    }
}