using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface ILogRealTimeDataSource : IObservable<JObject>, IDisposable
    {
    }
}
