using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Controller
{
    public class AsyncMessageBroker : AbstractObservable<JObject>, IObserver<JObject>
    {
        protected override void OnCompleted()
        {
            lock (Observers)
                Observers.AsParallel().ForAll(o => o.OnCompleted());
        }

        protected override void OnError(Exception exception)
        {
            lock (Observers)
                Observers.AsParallel().ForAll(o => o.OnError(exception));
        }

        protected override void OnNext(JObject next)
        {
            lock (Observers)
                Observers.AsParallel().ForAll(o => o.OnNext(next));
        }

        void IObserver<JObject>.OnCompleted()
        {
            OnCompleted();
        }

        void IObserver<JObject>.OnError(Exception error)
        {
            OnError(error);
        }

        void IObserver<JObject>.OnNext(JObject value)
        {
            OnNext(value);
        }
    }
}
