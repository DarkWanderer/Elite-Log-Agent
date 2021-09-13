using System.Threading.Tasks;

namespace DW.ELA.Controller
{
    public interface IApiKeyValidator
    {
        Task<bool> ValidateKeyAsync(string cmdrName, string apiKey);
    }
}
