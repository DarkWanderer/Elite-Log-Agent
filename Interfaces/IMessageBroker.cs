using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IMessageBroker : IObservable<JObject>, IObserver<JObject>, IDisposable
    {
    }
}
