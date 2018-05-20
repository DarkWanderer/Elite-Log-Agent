using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IPlayerStateHistoryRecorder : IObserver<JObject>
    {
        long? GetPlayerShipId(DateTime atTime);
        string GetPlayerLocation(DateTime atTime);
    }
}