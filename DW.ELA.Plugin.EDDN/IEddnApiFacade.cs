using System.Threading.Tasks;
using DW.ELA.Plugin.EDDN.Model;

namespace DW.ELA.Plugin.EDDN
{
    public interface IEddnApiFacade
    {
        Task PostEventAsync(EddnEvent events);
    }
}