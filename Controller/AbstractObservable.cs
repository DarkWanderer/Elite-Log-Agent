using System;
using System.Collections.Generic;

namespace Controller
{
    public abstract class AbstractObservable<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (observers)
            {
                if (!observers.Contains(observer))
                    observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> observers;
            private IObserver<T> observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                lock (observers)
                {
                    if (observer != null && observers.Contains(observer))
                        observers.Remove(observer);
                }
            }
        }

        protected void OnError(Exception exception)
        {
            lock (observers)
                foreach (var observer in observers)
                    observer.OnError(exception);
        }

        protected void OnNext(T next)
        {
            lock (observers)
                foreach (var observer in observers)
                    observer.OnNext(next);
        }

        protected void OnCompleted()
        {
            lock (observers)
                foreach (var observer in observers)
                    observer.OnCompleted();
        }
    }
}