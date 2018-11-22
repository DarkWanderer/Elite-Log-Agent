namespace DW.ELA.Interfaces
{
    using System;
    using DW.ELA.Interfaces.Settings;

    public interface ISettingsProvider
    {
        GlobalSettings Settings { get; set; }

        event EventHandler SettingsChanged;
    }
}