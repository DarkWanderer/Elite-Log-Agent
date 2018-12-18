namespace DW.ELA.Interfaces
{
    public interface IRestClientFactory
    {
        IRestClient CreateThrottlingRestClient(string url);
    }
}