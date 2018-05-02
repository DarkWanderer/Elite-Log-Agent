using Interfaces;
using System;

namespace Controller
{
    public static class LogSettingsManager
    {
        public static void Setup(ISettingsProvider settingsProvider)
        {
            var config = NLog.LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Clear();
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", new NLog.Targets.DebuggerTarget()));
            NLog.LogManager.Configuration = config;
        }
    }
}