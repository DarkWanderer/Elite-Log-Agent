using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class MessageBroker : AbstractObservable<JObject>, IObserver<JObject>
    {
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
