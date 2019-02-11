namespace DW.ELA.Interfaces
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(string url);
    }
}