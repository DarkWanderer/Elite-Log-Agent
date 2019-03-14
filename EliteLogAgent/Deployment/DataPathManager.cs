namespace EliteLogAgent.Deployment
{
    using System;
    using System.Deployment.Application;
    using System.IO;
    using DW.ELA.Interfaces;

    public class DataPathManager : IPathManager
    {
        public string SettingsDirectory => ApplicationDeployment.IsNetworkDeployed ? AppDataDirectory : LocalDirectory;

        public string LogDirectory => ApplicationDeployment.IsNetworkDeployed ? Path.Combine(AppDataDirectory, "Log") : Path.Combine(LocalDirectory, "Log");

        private string AppDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EliteLogAgent");

        private string LocalDirectory => Path.GetDirectoryName(typeof(Program).Assembly.CodeBase);
    }
}
