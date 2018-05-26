using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ELA.Plugin.EDSM
{
    public interface IEdsmApiFacade
    {
        Task PostLogEvents(JObject[] events);
    }
}