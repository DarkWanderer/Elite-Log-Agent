using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
            storageAccount = CloudStorageAccount.Parse(configuration["database-key-1"]);
            //storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("elitelogagentdb_AzureStorageConnectionString"));
        }

        // POST api/values
        [HttpPost]
        public async void Post([FromBody]string value)
        {
            logger.LogTrace("Received event: {0}", value);

            var serializer = new JsonSerializer();
            var records = JArray.Parse(value)
                .Children<JObject>()
                .Select(jo => jo.ToObject<ExceptionTableRecord>())
                .ToArray();

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Errors");
            await table.CreateIfNotExistsAsync();
            foreach (var rec in records)
            {
                var insertOperation = TableOperation.InsertOrReplace(rec);
                await table.ExecuteAsync(insertOperation);
            }

            //Execute the insert operation.
        }

        public class ExceptionRecord
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("exceptionType")]
            public string ExceptionType { get; set; }

            [JsonProperty("callStack")]
            public string CallStack { get; set; }

            [JsonProperty("version")]
            public string SoftwareVersion { get; set; }
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
            public string ETag { get; set; }

            [JsonIgnore]
            public DateTimeOffset Timestamp { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                if (properties.TryGetValue(nameof(Message), out var pe1))
                    Message = pe1.StringValue;
                if (properties.TryGetValue(nameof(ExceptionType), out var pe2))
                    ExceptionType = pe2.StringValue;
                if (properties.TryGetValue(nameof(CallStack), out var pe3))
                    CallStack = pe3.StringValue;
                if (properties.TryGetValue(nameof(SoftwareVersion), out var pe4))
                    SoftwareVersion = pe4.StringValue;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                return new Dictionary<string, EntityProperty>
                {
                    { nameof (Message), new EntityProperty(Message) },
                    { nameof (ExceptionType), new EntityProperty(ExceptionType) },
                    { nameof (CallStack), new EntityProperty(CallStack) },
                    { nameof (SoftwareVersion), new EntityProperty(SoftwareVersion) }
                };
            }
        }
    }
}

