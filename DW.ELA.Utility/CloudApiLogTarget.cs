﻿using DW.ELA.Utility.Json;
using DW.ELA.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Utility;

namespace DW.ELA.Utility
{
    public class CloudApiLogTarget : NLog.Targets.Target
    {
        private readonly IRestClient restClient;
        private readonly ConcurrentQueue<ExceptionRecord> recordQueue = new ConcurrentQueue<ExceptionRecord>();
        private readonly Timer flushTimer = new Timer();

        public CloudApiLogTarget(IRestClient restClient)
        {
            this.restClient = restClient;

            flushTimer.AutoReset = true;
            flushTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            flushTimer.Start();
            flushTimer.Elapsed += (o, e) => FlushAsync((ex) => { });
        }

        private void EnqueueEvents(params AsyncLogEventInfo[] events) => EnqueueEvents(events.AsEnumerable());

        private void EnqueueEvents(IEnumerable<AsyncLogEventInfo> events)
        {
            foreach (var @event in events)
            {
                try
                {
                    if (@event.LogEvent.Level < LogLevel.Error)
                        continue;
                    foreach (var rec in Convert(@event.LogEvent))
                        recordQueue.Enqueue(rec);
                    @event.Continuation(null);

                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break(); // Can't log inside of logger
                    @event.Continuation(ex);
                }
            }
        }

        protected override void Write(IList<AsyncLogEventInfo> logEvents) => EnqueueEvents(logEvents);
        protected override void Write(AsyncLogEventInfo logEvent) => EnqueueEvents(logEvent);

        protected override void WriteAsyncThreadSafe(IList<AsyncLogEventInfo> logEvents) => EnqueueEvents(logEvents);
        protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent) => EnqueueEvents(logEvent);

        [Obsolete]
        protected override void Write(AsyncLogEventInfo[] logEvents) => EnqueueEvents(logEvents);
        [Obsolete]
        protected override void WriteAsyncThreadSafe(AsyncLogEventInfo[] logEvents) => EnqueueEvents(logEvents);

        protected override async void FlushAsync(AsyncContinuation asyncContinuation)
        {
            try
            {
                var records = new List<ExceptionRecord>();
                while (recordQueue.TryDequeue(out var rec))
                    records.Add(rec);

                if (records.Count > 0)
                    await PostEventAsync(records.Take(10).ToArray()); // Don't need thousands of exceptions

                asyncContinuation(null);
            }
            catch (Exception ex)
            {
                asyncContinuation(ex);
            }
        }

        private Task PostEventAsync(params ExceptionRecord[] records)
        {
            var json = Serialize.ToJson(new JArray(records.Select(JObject.FromObject)));
            return restClient.PostAsync(json);
        }

        private IEnumerable<ExceptionRecord> Convert(LogEventInfo logEvent)
        {
            string message;
            if (logEvent.Parameters != null)
                message = String.Format(logEvent.Message, logEvent.Parameters);
            else
                message = logEvent.Message;

            var exception = logEvent.Exception;
            if (exception is AggregateException ae)
                exception = ae.Flatten();

            if (exception != null)
                yield return new ExceptionRecord(AppInfo.Version, "", Serialize.ToJson(exception));
        }

        [JsonObject("exceptionRecord")]
        public class ExceptionRecord
        {
            [JsonConstructor]
            public ExceptionRecord(string softwareVersion, string cmdrNameHash, string exceptionString)
            {
                SoftwareVersion = softwareVersion;
                CmdrNameHash = cmdrNameHash;
                ExceptionString = exceptionString;
            }

            [JsonProperty("version")]
            public string SoftwareVersion { get; }

            [JsonProperty("cmdrNameHash")]
            public string CmdrNameHash { get;  }

            [JsonProperty("exceptionString")]
            public string ExceptionString { get; }
        }
    }
}
