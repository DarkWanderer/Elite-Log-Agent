using DW.ELA.LogModel;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IMessageBroker : IObservable<LogEvent>, IObserver<LogEvent>, IDisposable
    {
    }
}
