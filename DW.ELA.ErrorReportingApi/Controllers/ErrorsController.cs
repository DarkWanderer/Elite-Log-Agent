using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace DW.ELA.ErrorReportingApi.Controllers
{
    [Route("api/[controller]")]
    public partial class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> logger;
        private readonly IConfiguration configuration;
        private readonly CloudStorageAccount storageAccount;

        public ErrorsController(ILogger<ErrorsController> logger, IConfiguration configuration, IHostingEnvironment env)
        {
            this.logger = logger;
            this.configuration = configuration;

            //if (env.IsDevelopment())
            //    storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            //else
                using (var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(TokenUtils.GetToken)))
                {
                    var secret = kv.GetSecretAsync("https://elitelogagent-key-vault.vault.azure.net/secrets/database-key-1/4295c3798e9540ea9b74b0f9078f19d9").Result;
                    storageAccount = CloudStorageAccount.Parse(secret.Value);
                }
        }

        // POST api/values
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                logger.LogDebug("GET query");

                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("Errors");
                await table.CreateIfNotExistsAsync();

                var records = await GetAllRecords<ExceptionTableRecord>(table);
                return JsonConvert.SerializeObject(records, Formatting.Indented);
            }
            catch (StorageException excep)
            {
                var extendedInformation = excep.RequestInformation.ExtendedErrorInformation;
                var errorCode = extendedInformation.ErrorCode;
                if (errorCode == "TableBeingDeleted")//
                {
                    //Table is being deleted ... wait out.
                }
                return "Error: " + excep.Message;
            }
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]ExceptionTableRecord record) => await Post(new[] { record });

        [HttpPost]
        public async Task Post([FromBody]ExceptionTableRecord[] records)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Errors");
            await table.CreateIfNotExistsAsync();
            foreach (var rec in records)
            {
                rec.SetKeyHash();
                var insertOperation = TableOperation.InsertOrReplace(rec);
                await table.ExecuteAsync(insertOperation);
            }
        }

        private async Task<IReadOnlyCollection<T>> GetAllRecords<T>(CloudTable table) where T : ITableEntity, new()
        {
            var results = new List<T>();

            // Initialize a default TableQuery to retrieve all the entities in the table.
            var tableQuery = new TableQuery<T>();

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            do
            {
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                // Print the number of rows retrieved.
                logger.LogDebug("Retrieved {0} rows from {1}", tableQueryResult.Results.Count, table.Name);
                results.AddRange(tableQueryResult.Results);
            } while (continuationToken != null);

            return results;
        }
    }
}