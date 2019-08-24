namespace DW.ELA.Controller
{
    using System.Threading.Tasks;

    public interface IApiKeyValidator
    {
        Task<bool> ValidateKeyAsync(string cmdrName, string apiKey);
    }
}
