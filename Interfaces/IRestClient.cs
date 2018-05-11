using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public interface IRestClient
    {
        Task<string> PostAsync(string input);
    }
}