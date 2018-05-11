using System;
using Newtonsoft.Json.Linq;

namespace PowerplayGoogleSheetReporter
{
    internal class DummyObserver : IObserver<JObject>
    {
        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(JObject value)
        {
            
        }
    }
}