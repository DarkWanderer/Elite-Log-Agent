using System;
using System.Collections.Generic;

namespace DW.ELA.Utility.Observable
{
    public class BasicObservable<T> : IObservable<T>
    {
        protected List<IObserver<T>> Observers { get; } = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            lock (Observers)
            {
                if (!Observers.Contains(observer))
                    Observers.Add(observer);
            }
            return new Unsubscriber(Observers, observer);
        }

        public virtual void OnError(Exception exception)
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnError(exception));
        }

        public virtual void OnNext(T next)
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnNext(next));
        }

        public virtual void OnCompleted()
        {
            lock (Observers)
                Functional.ExecuteManyWithAggregateException(Observers, i => i.OnCompleted());
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<T>> observers;
            private readonly IObserver<T> observer;

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
    }
}
