using System.IO;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class AppInfo
    {
        public AppInfo(string packageId, string version, string installDirectory)
        {
            PackageId = packageId;
            Version = version;
            InstallPath = Path.GetFullPath(installDirectory);
        }

        public string PackageId { get; set; }

        public string Version { get; set; }

        public string InstallPath { get; set; }
    }
}