namespace DW.ELA.Controller
{
    using System;
    using System.IO;
    using System.Text;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility;
    using NLog;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    public class NLogSettingsManager : ILogSettingsBootstrapper
    {
        private const string DefaultLayout = "${longdate}|${level}|${logger}|${message} ${exception:format=ToString,StackTrace:innerFormat=ToString,StackTrace:maxInnerExceptionLevel=10}";
        private const string CloudErrorReportingUrl = "https://app-telemetry.azurewebsites.net/api/post-errors";

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;

        static NLogSettingsManager()
        {
            AppDomain.CurrentDomain.DomainUnload += (o, e) => LogManager.Flush();
        }

        public NLogSettingsManager(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        private static string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"EliteLogAgent\Log");

        public void Setup()
        {
            var logLevel = LogLevel.Info;
            try
            {
                if (!string.IsNullOrEmpty(settingsProvider.Settings.LogLevel))
                    logLevel = LogLevel.FromString(settingsProvider.Settings.LogLevel);
            }
            catch
            { /* Do nothing, use default*/
            }

            var config = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Clear();

            var fileTarget = CreateFileTarget();

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, fileTarget));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, new DebuggerTarget() { Layout = DefaultLayout }));

            if (settingsProvider?.Settings?.ReportErrorsToCloud ?? false)
            {
                var webCollector = new WebServiceTarget() { Protocol = WebServiceProtocol.JsonPost, Url = new Uri(CloudErrorReportingUrl) };
                webCollector.Parameters.Add(new MethodCallParameter(string.Empty, GetCloudErrorLayout()));
                var asyncWrapper = new AsyncTargetWrapper()
                {
                    OverflowAction = AsyncTargetWrapperOverflowAction.Discard,
                    WrappedTarget = webCollector,
                    BatchSize = 1,
                    QueueLimit = 10,
                    FullBatchSizeWriteLimit = 10,
                    TimeToSleepBetweenBatches = 0,
                    Name = "CloudErrorTargetAsync"
                };

                config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Error, asyncWrapper));
                config.AddTarget(asyncWrapper);
            }

            LogManager.Configuration = config;
            Log.Info("Enabled logging with level {0}", logLevel);
        }

#pragma warning disable SA1118 // Parameter must not span multiple lines
        private JsonLayout GetDefaultJsonLayout()
        {
            return new JsonLayout()
            {
                Attributes =
                    {
                        new JsonAttribute("level", "${level}"),
                        new JsonAttribute("time", "${longdate}"),
                        new JsonAttribute("message", "${message}"),
                        new JsonAttribute("logger", "${logger}"),
                        new JsonAttribute("exception", 
                            new JsonLayout()
                            {
                                Attributes =
                                {
                                    new JsonAttribute("type", "${exception:format=ShortType}"),
                                    new JsonAttribute("message", "${exception:format=Message}"),
                                    new JsonAttribute("data", "${exception:format=Data}"),
                                    new JsonAttribute("stackTrace", "${exception:format=StackTrace}"),
                                    new JsonAttribute("innerException", 
                                        new JsonLayout()
                                        {
                                            Attributes =
                                            {
                                                new JsonAttribute("type", "${exception:format=:innerFormat=ShortType:MaxInnerExceptionLevel=1:InnerExceptionSeparator="),
                                                new JsonAttribute("message", "${exception:format=:innerFormat=Message:MaxInnerExceptionLevel=1:InnerExceptionSeparator="),
                                                new JsonAttribute("data", "${exception:format=:innerFormat=Data:MaxInnerExceptionLevel=1:InnerExceptionSeparator="),
                                                new JsonAttribute("stackTrace", "${exception:format=:innerFormat=StackTrace:MaxInnerExceptionLevel=1:InnerExceptionSeparator="),
                                            },
                                            RenderEmptyObject = false
                                        },
                                        false)
                                },
                                RenderEmptyObject = false
                            },
                            false)
                    },
                RenderEmptyObject = false,
                IncludeAllProperties = true,
                ExcludeProperties = { "CallerFilePath", "CallerLineNumber", "CallerMemberName" }
            };
        }

        private JsonLayout GetCloudErrorLayout()
        {
            var layout = GetDefaultJsonLayout();
            layout.Attributes.Add(new JsonAttribute("application", AppInfo.Name));
            layout.Attributes.Add(new JsonAttribute("version", AppInfo.Version));
            return layout;
        }
#pragma warning restore SA1118 // Parameter must not span multiple lines

        private Target CreateFileTarget()
        {
            return new FileTarget
            {
                FileName = Path.Combine(LogDirectory, "EliteLogAgent.json"),
                ArchiveFileName = Path.Combine(LogDirectory, "EliteLogAgent.{###}.json"),
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 10,
                ConcurrentWrites = true,
                ReplaceFileContentsOnEachWrite = false,
                Encoding = Encoding.UTF8,
                Layout = GetDefaultJsonLayout()
            };
        }
    }
}
