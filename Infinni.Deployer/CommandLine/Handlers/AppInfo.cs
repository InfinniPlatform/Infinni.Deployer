using System.IO;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class AppInfo
    {
        public AppInfo(string name, string version, string installDirectory)
        {
            Name = name;
            Version = version;
            InstallPath = Path.GetFullPath(installDirectory);
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string InstallPath { get; set; }
    }
}