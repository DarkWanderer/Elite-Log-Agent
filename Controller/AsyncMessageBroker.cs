using Interfaces;
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
    public class AsyncMessageBroker : AbstractObservable<JObject>, IMessageBroker, IObserver<JObject>, IObservable<JObject>
    {
        private readonly ILogger Log;

        public AsyncMessageBroker(ILogger log)
        {
            Log = log;
        }

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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    lock(Observers)
                        Observers.Clear();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AsyncMessageBroker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}