namespace DW.ELA.Interfaces
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    public interface ILogHistoricDataSource : IEnumerable<JObject>
    {
    }
}
