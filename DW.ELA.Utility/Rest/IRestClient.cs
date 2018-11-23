namespace DW.ELA.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRestClient
    {
        Task<string> PostAsync(string input);

        Task<string> GetAsync(string input);

        Task<string> PostAsync(IDictionary<string, string> values);
    }
}