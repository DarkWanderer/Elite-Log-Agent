using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Interfaces
{
    public interface ILogHistoricDataSource : IEnumerable<JObject>
    {
    }
}
