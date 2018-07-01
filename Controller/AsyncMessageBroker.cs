using DW.ELA.Interfaces;
using DW.ELA.LogModel;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using Utility;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// Forwards events from one IObservables to multiple IObservers in parallel fashion
    /// </summary>
    public class AsyncMessageBroker : AbstractObservable<LogEvent>, IMessageBroker, IObserver<LogEvent>, IObservable<LogEvent>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected override void OnCompleted()
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnCompleted());
            }
            catch (Exception e)
            {
                logger.Error(e, "Error caught in Async Broker");
            }
        }

        protected override void OnError(Exception exception)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnError(exception));
            }
            catch (Exception e)
            {
                logger.Error(e, "Error caught in Async Broker");
            }
        }

        protected override void OnNext(LogEvent next)
        {
            try
            {
                lock (Observers)
                    Observers.AsParallel().ExecuteManyWithAggregateException(i => i.OnNext(next));
            }
            catch (Exception e)
            {
                logger.Error(e, "Error caught in Async Broker");
            }
        }

        void IObserver<LogEvent>.OnCompleted() => OnCompleted();

        void IObserver<LogEvent>.OnError(Exception error) => OnError(error);

        void IObserver<LogEvent>.OnNext(LogEvent value) => OnNext(value);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    lock (Observers)
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