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
            LogLevel logLevel = LogLevel.Trace;
            try
            {
                if (!String.IsNullOrEmpty(settingsProvider.Settings.LogLevel))
                    logLevel = LogLevel.FromString(settingsProvider.Settings.LogLevel);
            }
            catch { /* Do nothing, use default*/ }

            var config = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Clear();

            var fileTarget = new FileTarget();
            fileTarget.FileName = Path.Combine(LogDirectory, "EliteLogAgent.log");
            fileTarget.ArchiveFileName = Path.Combine(LogDirectory, "EliteLogAgent.{#####}.log");
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
            fileTarget.ArchiveEvery = FileArchivePeriod.Day;
            fileTarget.MaxArchiveFiles = 30;
            fileTarget.ConcurrentWrites = true;
            fileTarget.ReplaceFileContentsOnEachWrite = false;
            fileTarget.Encoding = Encoding.UTF8;

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, fileTarget));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, new DebuggerTarget()));
            LogManager.Configuration = config;
            logger.Info("Enabled logging with level {0}", logLevel);
        }

        private static string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"EliteLogAgent\Log");
    }
}