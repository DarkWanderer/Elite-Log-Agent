namespace DW.ELA.Utility.Log
{
    using System;
    using System.Diagnostics;
    using NLog;
    using NLog.Fluent;

    public class LoggingTimer : IDisposable
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly string context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public LoggingTimer(string context)
        {
            this.context = context;
            stopwatch.Start();
        }

        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public TimeSpan Elapsed => stopwatch.Elapsed;

        public void Dispose()
        {
            stopwatch.Stop();
            logger.Log(LogLevel)
                .Message("{0}", context)
                .Property("duration", stopwatch.ElapsedMilliseconds)
                .Write();
        }
    }
}
