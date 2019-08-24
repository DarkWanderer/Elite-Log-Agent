namespace DW.ELA.Interfaces
{
    using System;

    public interface IMessageBroker : IObservable<JournalEvent>, IObserver<JournalEvent>, IDisposable
    {
    }
}
