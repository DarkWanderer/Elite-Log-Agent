using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Controller
{
    public abstract class AbstractObservable<T> : IObservable<T>
    {
        protected readonly List<IObserver<T>> Observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (Observers)
            {
                if (!Observers.Contains(observer))
                    Observers.Add(observer);
            }
            return new Unsubscriber(Observers, observer);
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

        protected virtual void OnError(Exception exception)
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnError(exception));
        }

        protected virtual void OnNext(T next)
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnNext(next));
        }

        protected virtual void OnCompleted()
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnCompleted());
        }
    }
}