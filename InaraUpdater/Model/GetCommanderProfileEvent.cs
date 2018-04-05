using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public class GetCommanderProfileEvent : Event
    {
        private readonly object commanderName;

        public GetCommanderProfileEvent(string commanderName)
        {
            this.commanderName = commanderName;
        }

        public override string EventName => "getCommanderProfile";
        public override object EventData => new { searchName = commanderName };
    }
}
