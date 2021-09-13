using System;

namespace DW.ELA.Interfaces
{
    public interface ILogRealTimeDataSource : IObservable<JournalEvent>, IDisposable
    {
    }
}
