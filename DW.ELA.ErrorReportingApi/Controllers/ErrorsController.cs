using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DW.ELA.ErrorReportingApi.Controllers
{
    [Route("api/[controller]")]
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> logger;
        private readonly IConfiguration configuration;
        private readonly CloudStorageAccount storageAccount;

        public ErrorsController(ILogger<ErrorsController> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;

            //storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("elitelogagentdb_AzureStorageConnectionString"));
        }

        // GET api/values
        [HttpGet]
        public async Task<int> Get()
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Errors");
            await table.CreateIfNotExistsAsync();
            TableContinuationToken token = null;

            var entities = new List<ExceptionTableRecord>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<ExceptionTableRecord>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities.Count;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            logger.LogDebug("Received get: {0}", id);
            return "value" + id;
        }

        // POST api/values
        [HttpPost]
        public async void Post([FromBody]string value)
        {
            logger.LogDebug("Received event: {0}", value);


            var serializer = new JsonSerializer();
            var e = JObject.Parse(value).ToObject<ExceptionTableRecord>();

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Errors");
            await table.CreateIfNotExistsAsync();
            var insertOperation = TableOperation.Insert(e);

            // Execute the insert operation.
            await table.ExecuteAsync(insertOperation);

            //return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private class ExceptionRecord
        {
            [JsonProperty("message")]
            public string Message { get; set; }
            [JsonProperty("exceptionType")]
            public string ExceptionType { get; set; }
            [JsonProperty("callStack")]
            public string CallStack { get; set; }
            [JsonProperty("version")]
            public string SoftwareVersion { get; set; }
            [JsonProperty("timestamp")]
            public DateTime ExceptionTimestamp { get; set; }
        }

        private class ExceptionTableRecord : ExceptionRecord, ITableEntity
        {
            [JsonIgnore]
            public string PartitionKey
            {
                get => SoftwareVersion;
                set => SoftwareVersion = value;
            }

            [JsonIgnore]
            public string RowKey
            {
                get => Message;
                set => Message = value;
            }

            [JsonIgnore]
            public DateTimeOffset Timestamp
            {
                get => ExceptionTimestamp;
                set => ExceptionTimestamp = value.UtcDateTime;
            }

            [JsonIgnore]
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext) => throw new NotImplementedException();
            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) => throw new NotImplementedException();
        }
    }
}

