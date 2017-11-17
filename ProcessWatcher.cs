using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DW.Inara.LogUploader
{
    public class ProcessWatcher : IDisposable
    {
        private string[] watchedProcessNames;

        public ProcessWatcher(params string[] watchedProcessNames)
        {
            if (watchedProcessNames == null)
                throw new ArgumentNullException(nameof(watchedProcessNames));
            if (watchedProcessNames.Length == 0)
                throw new ArgumentException(nameof(watchedProcessNames) + " must not be empty");

            this.watchedProcessNames = watchedProcessNames;
        }

        private void Run(CancellationToken token)
        {
            var managementScope = new ManagementScope(@"\\mysever\root\onguard");
            managementScope.Connect();
            while (!token.IsCancellationRequested)
            {
                var query = new EventQuery("SELECT TargetInstance FROM __InstanceDeletionEvent WHERE TargetInstance ISA \"Win32_Process\"");
                var eventWatcher = new ManagementEventWatcher(managementScope, query);
                var wmiEvent = eventWatcher.WaitForNextEvent();
                Console.Out.WriteLine(wmiEvent.GetPropertyValue("Description"));
            }
        }

        public class ProcessEventArgs : EventArgs
        {
            public string ProcessName;
            public int ProcessId;
        }

        public event EventHandler<ProcessEventArgs> ProcessStarted;
        public event EventHandler<ProcessEventArgs> ProcessFinished;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ProcessWatcher() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
