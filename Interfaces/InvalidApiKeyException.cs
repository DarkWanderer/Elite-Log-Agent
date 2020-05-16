using System;

namespace DW.ELA.Interfaces
{
    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException() : base("Invalid API key")
        {

        }
    }
}
