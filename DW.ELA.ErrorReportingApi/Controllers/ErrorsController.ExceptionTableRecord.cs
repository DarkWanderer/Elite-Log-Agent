using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace DW.ELA.ErrorReportingApi.Controllers
{
    public partial class ErrorsController
    {
        public class ExceptionTableRecord : ExceptionRecord, ITableEntity
        {
            [JsonIgnore]
            public string PartitionKey
            {
                get => SoftwareVersion;
                set => SoftwareVersion = value;
            }

            [JsonIgnore]
            public string RowKey { get; set; }

            [JsonIgnore]
            public string Hash
            {
                get
                {
                    var digestFeed = new StringBuilder()
                        .Append(Message)
                        .Append('|')
                        .Append(ExceptionType)
                        .Append('|')
                        .Append(CallStack)
                        .Append('|')
                        .Append(SoftwareVersion)
                        .ToString();

                    using (var sha1 = new SHA1Managed())
                        return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(digestFeed)));
                }
            }

            public void SetKeyHash() => RowKey = Hash;

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