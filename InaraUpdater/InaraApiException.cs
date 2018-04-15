using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater
{
    internal class InaraApiException : Exception
    {
        public string InputJson { get; }

        public InaraApiException(string message) : base(message)
        {
        }

        public InaraApiException(string message, string inputJson) : base(message)
        {
            InputJson = inputJson;
        }

        public override string ToString()
        {
            return base.ToString() + "; input was:\n" + InputJson;
        }
    }
}
