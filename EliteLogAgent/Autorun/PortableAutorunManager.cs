namespace EliteLogAgent.Autorun
{
    using System.Reflection;

    public class PortableAutorunManager : ClickOnceAutorunManager
    {
        protected override string ExecutablePath => Assembly.GetExecutingAssembly().Location;
    }
}
