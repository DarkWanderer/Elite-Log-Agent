using System;

namespace DW.Inara.LogUploader
{
    public class LogUploadEventArgs : EventArgs
    {
        public LogUploadEventArgs(string fileName)
        {
            FileName = fileName;
        }

        public LogUploadEventArgs(string fileName, string error)
        {
            FileName = fileName;
            Error = error;
        }

        public string FileName { get; }
        public string Error { get; }
    }
}