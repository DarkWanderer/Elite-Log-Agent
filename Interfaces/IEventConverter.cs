namespace DW.ELA.Interfaces
{
    using System.Collections.Generic;

    public interface IEventConverter<out TEvent>
        where TEvent : class
    {
        IEnumerable<TEvent> Convert(JournalEvent @event);
    }
}
