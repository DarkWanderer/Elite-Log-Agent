namespace DW.ELA.Interfaces
{
    using System;

    public interface ITrayIconController : IDisposable
    {
        void ShowErrorNotification(string error);
    }
}