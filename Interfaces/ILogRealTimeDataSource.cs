using DW.ELA.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface ILogRealTimeDataSource : IObservable<LogEvent>, IDisposable
    {
    }
}
