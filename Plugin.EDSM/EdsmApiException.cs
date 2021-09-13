using System;
using System.Runtime.Serialization;

namespace DW.ELA.Plugin.EDSM
{
    [Serializable]
    internal class EdsmApiException : Exception
    {
        public int ReturnCode { get; private set; }

        public EdsmApiException()
        {
        }

        public EdsmApiException(string message)
            : base(message)
        {
        }

        public EdsmApiException(string message, int returnCode)
            : base(message)
        {
            ReturnCode = returnCode;
        }

        public EdsmApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EdsmApiException(string message, int returnCode, Exception innerException)
            : base(message, innerException)
        {
            ReturnCode = returnCode;
        }

        protected EdsmApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}