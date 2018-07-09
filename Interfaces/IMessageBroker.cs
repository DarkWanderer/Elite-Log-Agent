using DW.ELA.Interfaces;
using System;

namespace Interfaces
{
    public interface IMessageBroker : IObservable<LogEvent>, IObserver<LogEvent>, IDisposable
    {
    }
}
