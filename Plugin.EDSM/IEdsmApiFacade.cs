namespace DW.ELA.Plugin.EDSM
{
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public interface IEdsmApiFacade
    {
        Task PostLogEvents(JObject[] events);
    }
}