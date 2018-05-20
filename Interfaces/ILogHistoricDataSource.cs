using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Interfaces
{
    public interface ILogHistoricDataSource : IEnumerable<JObject>
    {
    }
}
