namespace DW.ELA.Controller
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.LogModel;
    using DW.ELA.Utility.Observable;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class JournalUdpMonitor : IJournalDataSource
    {
        private const string Handshake = "{ \"Subscribe\": true, \"All\": true }";
        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly IPEndPoint bindAddress;
        private readonly UdpClient udpClient;
        private readonly BasicObservable<LogEvent> observable = new BasicObservable<LogEvent>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly object processLock = new object();
        private readonly StringBuilder splitMessageContents = new StringBuilder();

        private UdpJournalContext context = null;

        private bool isDisposed = false; // To detect redundant calls

        public JournalUdpMonitor(ISettingsProvider settingsProvider)
        {
            ushort port = settingsProvider.Settings.Port;
            bindAddress = new IPEndPoint(IPAddress.Loopback, port);
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;

            udpClient.Client.Bind(bindAddress);
            StartReceive();
        }

        public IDisposable Subscribe(IObserver<LogEvent> observer) => observable.Subscribe(observer);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    cancellationTokenSource.Cancel();
                    udpClient.Dispose();
                }
                isDisposed = true;
            }
        }

        private void StartReceive() => udpClient.ReceiveAsync()
            .ContinueWith(task => ProcessReceivedPacket(task.Result), cancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

        private void ProcessReceivedPacket(UdpReceiveResult receiveResult)
        {
            try
            {
                lock (processLock)
                {
                    var remoteEndpoint = receiveResult.RemoteEndPoint;
                    string message = Encoding.UTF8.GetString(receiveResult.Buffer);

                    if (log.IsTraceEnabled)
                        log.Trace("Received message: {message}", message);

                    bool parseResult = TryParseJson(message, out var jObject);

                    if (parseResult && jObject["Publish"] != null && jObject["Publish"].Value<string>() == "EliteDangerous")
                    {
                        string cmdrName = jObject["CommanderName"]?.Value<string>();
                        string version = jObject["Version"]?.Value<string>();
                        context = new UdpJournalContext(cmdrName, version);
                        SendSubscribeHandshake(remoteEndpoint);
                        log.Info("Received handshake, CMDR {commanderName}, game version {gameVersion}", cmdrName, version);
                    }
                    else if (parseResult)
                    {
                        ProcessValidJson(jObject);
                    }
                    else
                    {
                        ProcessInvalidJson(message);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                StartReceive();
            }
        }

        private void ProcessInvalidJson(string message)
        {
            bool wasNotEmpty = splitMessageContents.Length > 0;
            splitMessageContents.Append(message);
            if (wasNotEmpty && TryParseJson(splitMessageContents.ToString(), out var jObject))
            {
                splitMessageContents.Clear();
                ProcessValidJson(jObject);
            }
        }

        private void ProcessValidJson(JObject jObject)
        {
            var @event = LogEventConverter.Convert(jObject);
            observable.OnNext(@event);
        }

        private void SendSubscribeHandshake(IPEndPoint endpoint)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Handshake);
            udpClient.Connect(endpoint);
            udpClient.Client.Send(bytes);
        }

        private bool TryParseJson(string json, out JObject result)
        {
            try
            {
                result = JObject.Parse(json);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        internal class UdpJournalContext : IJournalContext
        {
            public UdpJournalContext(string commanderName, string gameVersion)
            {
                CommanderName = commanderName;
                GameVersion = gameVersion;
            }

            public string CommanderName { get; }

            public string GameVersion { get; }
        }
    }
}
