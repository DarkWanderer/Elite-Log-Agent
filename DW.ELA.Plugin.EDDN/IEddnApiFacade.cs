using System.Threading.Tasks;
using DW.ELA.Plugin.EDDN.Model;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Plugin.EDDN
{
    public interface IEddnApiFacade
    {
        Task PostEventsAsync(params EddnEvent[] events);
    }
}