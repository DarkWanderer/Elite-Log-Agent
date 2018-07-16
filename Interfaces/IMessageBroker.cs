using DW.ELA.Interfaces;
using System;

namespace DW.ELA.Interfaces
{
    public interface IMessageBroker : IObservable<LogEvent>, IObserver<LogEvent>, IDisposable
    {
    }
}
