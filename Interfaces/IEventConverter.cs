using DW.ELA.LogModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Interfaces
{
    public interface IEventConverter<out TEvent> where TEvent : class
    {
        IEnumerable<TEvent> Convert(LogEvent @event);
    }
}
