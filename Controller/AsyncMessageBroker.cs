using System;
using System.Linq;
using DW.ELA.Interfaces;
using DW.ELA.Utility;
using NLog;
using DW.ELA.Utility.Observable;

namespace DW.ELA.Controller
{
    /// <summary>
    /// Forwards events from one IObservables to multiple IObservers in parallel fashion
    /// </summary>
    public class AsyncMessageBroker : BasicObservable<JournalEvent>, IMessageBroker, IObserver<JournalEvent>, IObservable<JournalEvent>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public override void OnCompleted()
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnCompleted());
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        public override void OnError(Exception exception)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnError(exception));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        public override void OnNext(JournalEvent next)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnNext(next));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error caught in Async Broker");
            }
        }

        void IObserver<JournalEvent>.OnCompleted() => OnCompleted();

        void IObserver<JournalEvent>.OnError(Exception error) => OnError(error);

        void IObserver<JournalEvent>.OnNext(JournalEvent value) => OnNext(value);

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (Observers)
                        Observers.Clear();
                }

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
    }
}
