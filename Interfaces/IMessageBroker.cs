using System;

namespace DW.ELA.Interfaces
{
    public interface IMessageBroker : IObservable<JournalEvent>, IObserver<JournalEvent>, IDisposable
    {
    }
}
