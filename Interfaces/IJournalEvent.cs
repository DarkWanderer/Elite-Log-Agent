using System;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Interfaces
{
    public interface IJournalEvent
    {
        string Event { get; }
        JObject Raw { get; }
        DateTime Timestamp { get; }
    }
}