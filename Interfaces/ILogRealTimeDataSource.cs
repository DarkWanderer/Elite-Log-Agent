using DW.ELA.LogModel;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface ILogRealTimeDataSource : IObservable<LogEvent>, IDisposable
    {
    }
}
