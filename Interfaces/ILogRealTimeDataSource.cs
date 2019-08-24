namespace DW.ELA.Interfaces
{
    using System;

    public interface ILogRealTimeDataSource : IObservable<JournalEvent>, IDisposable
    {
    }
}
