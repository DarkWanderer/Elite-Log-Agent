using System;
using System.IO;
using System.Text;
using DW.ELA.Interfaces;
using DW.ELA.Utility;
using NLog;
using NLog.Fluent;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DW.ELA.Controller
{
    public class NLogSettingsManager : ILogSettingsBootstrapper
    {
        private const string DefaultLayout = "${longdate}|${level}|${logger}|${message} ${exception:format=ToString,StackTrace:innerFormat=ToString,StackTrace:maxInnerExceptionLevel=10}";
        private const string CloudErrorReportingUrl = "https://app-telemetry.azurewebsites.net/api/post-errors";

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;
        private readonly string logDirectory;

        static NLogSettingsManager()
        {
            AppDomain.CurrentDomain.DomainUnload += (o, e) => LogManager.Flush();
        }

        public NLogSettingsManager(ISettingsProvider settingsProvider, IPathManager pathManager)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            logDirectory = pathManager.LogDirectory;
        }

        public void Setup()
        {
            var logLevel = LogLevel.Info;
            try
            {
                if (!string.IsNullOrEmpty(settingsProvider.Settings?.LogLevel))
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
            Log.Info().Message("Logging enabled").Property("level", logLevel).Write();
        }

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
            layout.Attributes.Add(new JsonAttribute("@timestamp", "${date:format=o}"));
            return layout;
        }

        private Target CreateFileTarget()
        {
            return new FileTarget
            {
                FileName = Path.Combine(logDirectory, "EliteLogAgent.json"),
                ArchiveFileName = Path.Combine(logDirectory, "EliteLogAgent.{###}.json"),
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
