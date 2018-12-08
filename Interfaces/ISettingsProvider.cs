namespace DW.ELA.Interfaces
{
    using System;
    using DW.ELA.Interfaces.Settings;

    public interface ISettingsProvider
    {
        event EventHandler SettingsChanged;

        GlobalSettings Settings { get; set; }
    }
}