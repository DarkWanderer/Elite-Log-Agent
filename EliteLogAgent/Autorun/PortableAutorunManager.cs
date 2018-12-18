namespace EliteLogAgent.Autorun
{
    using System;
    using System.IO;
    using System.Reflection;
    using DW.ELA.Interfaces;
    using Microsoft.Win32;

    public class PortableAutorunManager : ClickOnceAutorunManager
    {
        protected override string ExecutablePath => Assembly.GetExecutingAssembly().Location;
    }
}
