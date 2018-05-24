using Interfaces;
using NLog;

namespace Controller
{
    public class NLogSettingsManager : ILogSettingsBootstrapper
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;

        public NLogSettingsManager(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new System.ArgumentNullException(nameof(settingsProvider));
        }

        public void Setup()
        {
            LogLevel logLevel = LogLevel.Trace;
            try
            {
                if (!string.IsNullOrEmpty(settingsProvider.Settings.LogLevel))
                    logLevel = LogLevel.FromString(settingsProvider.Settings.LogLevel);
            }
            catch { /* Do nothing, use default*/ }

            var config = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Clear();
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, new NLog.Targets.DebuggerTarget()));
            LogManager.Configuration = config;
            logger.Info("Enabled logging with level {0}", logLevel);
        }
    }
}