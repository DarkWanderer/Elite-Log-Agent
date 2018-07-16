using DW.ELA.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace DW.ELA.Interfaces
{
    public interface ILogRealTimeDataSource : IObservable<LogEvent>, IDisposable
    {
    }
}
