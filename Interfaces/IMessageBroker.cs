namespace DW.ELA.Interfaces
{
    using System;

    public interface IMessageBroker : IObservable<LogEvent>, IObserver<LogEvent>, IDisposable
    {
    }
}
