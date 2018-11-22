namespace DW.ELA.Interfaces
{
    using System;

    public interface ILogRealTimeDataSource : IObservable<LogEvent>, IDisposable
    {
    }
}
