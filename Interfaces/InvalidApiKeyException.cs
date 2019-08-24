using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Interfaces
{
    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException() : base("Invalid API key")
        {

        }
    }
}
