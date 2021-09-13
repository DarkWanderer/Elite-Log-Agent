using System;

namespace DW.ELA.Interfaces
{
    public class RateLimitException : Exception
    {
        public RateLimitException() : base("Server rate limit exceeded")
        {

        }
    }
}
