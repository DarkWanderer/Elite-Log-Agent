using System;

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

        public override string ToString() => base.ToString() + "; input was:\n" + InputJson;
    }
}
