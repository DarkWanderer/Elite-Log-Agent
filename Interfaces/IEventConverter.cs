using System.Collections.Generic;

namespace DW.ELA.Interfaces
{
    public interface IEventConverter<out TEvent> where TEvent : class
    {
        IEnumerable<TEvent> Convert(LogEvent @event);
    }
}
