using Interfaces;
using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Text;

namespace Controller
{
    public class NLogSettingsManager : ILogSettingsBootstrapper
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;

        public NLogSettingsManager(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        public void Setup()
        {
            var logLevel = LogLevel.Info;
            try
            {
                if (!String.IsNullOrEmpty(settingsProvider.Settings.LogLevel))
                    logLevel = LogLevel.FromString(settingsProvider.Settings.LogLevel);
            }
            catch { /* Do nothing, use default*/ }

            var config = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Clear();

            var fileTarget = new FileTarget
            {
                FileName = Path.Combine(LogDirectory, "EliteLogAgent.log"),
                ArchiveFileName = Path.Combine(LogDirectory, "EliteLogAgent.{#####}.log"),
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30,
                ConcurrentWrites = true,
                ReplaceFileContentsOnEachWrite = false,
                Encoding = Encoding.UTF8,
                Layout = "${longdate}|${level}|${logger}|${message} ${exception:format=ShortType,Message,StackTrace:innerFormat=ShortType,Message,StackTrace:maxInnerExceptionLevel=10}"
            };

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, fileTarget));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Trace, new DebuggerTarget()));
            LogManager.Configuration = config;
            //TestExceptionLogging();
            logger.Info("Enabled logging with level {0}", logLevel);
        }

        private void TestExceptionLogging()
        {
            try
            {
                try {
                    throw new Exception("Test inner exception");
                }
                catch (Exception e1)
                {
                    throw new ApplicationException("Test outer exception", e1);
                }
            }
            catch (Exception e2)
            {
                logger.Debug(e2, "Exception format test");
            }
        }

        private static string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"EliteLogAgent\Log");
    }
}