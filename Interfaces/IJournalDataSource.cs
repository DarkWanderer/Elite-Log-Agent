namespace DW.ELA.Interfaces
{
    using System;

    public interface IJournalDataSource : IObservable<LogEvent>, IDisposable
    {
    }
}
