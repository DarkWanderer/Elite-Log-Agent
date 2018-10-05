using DW.ELA.Utility;
using DW.ELA.Interfaces;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Utility;

namespace Controller
{
    public class NLogSettingsManager : ILogSettingsBootstrapper
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;

        static NLogSettingsManager() => AppDomain.CurrentDomain.DomainUnload += (o, e) => LogManager.Flush();

        public NLogSettingsManager(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }
        private const string DefaultLayout = "${longdate}|${level}|${logger}|${message} ${exception:format=ToString,StackTrace:innerFormat=ToString,StackTrace:maxInnerExceptionLevel=10}";

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

            var fileTarget = CreateFileTarget();

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, fileTarget));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, new DebuggerTarget() { Layout = DefaultLayout }));

            if (settingsProvider.Settings.ReportErrorsToCloud)
                config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Error, new CloudApiLogTarget(new ThrottlingRestClient(CloudErrorReportingUrl))));

            LogManager.Configuration = config;
            logger.Info("Enabled logging with level {0}", logLevel);
        }

        private Target CreateFileTarget()
        {
            return new FileTarget
            {
                FileName = Path.Combine(LogDirectory, "EliteLogAgent.log"),
                ArchiveFileName = Path.Combine(LogDirectory, "EliteLogAgent.{#####}.log"),
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 10,
                ConcurrentWrites = true,
                ReplaceFileContentsOnEachWrite = false,
                Encoding = Encoding.UTF8,
                Layout = DefaultLayout
            };
        }

        private static string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"EliteLogAgent\Log");

        public static string CloudErrorReportingUrl => "https://elitelogagent-api.azurewebsites.net/api/errors";
    }
}