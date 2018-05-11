using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// Forwards events from one IObservables to multiple IObservers in parallel fashion
    /// </summary>
    public class AsyncMessageBroker : AbstractObservable<JObject>, IObserver<JObject>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected override void OnCompleted()
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ForAll(o => o.OnCompleted());
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        protected override void OnError(Exception exception)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ForAll(o => o.OnError(exception));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        protected override void OnNext(JObject next)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ForAll(o => o.OnNext(next));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        void IObserver<JObject>.OnCompleted()
        {
            OnCompleted();
        }

        void IObserver<JObject>.OnError(Exception error) => OnError(error);

        void IObserver<JObject>.OnNext(JObject value) => OnNext(value);
    }
}