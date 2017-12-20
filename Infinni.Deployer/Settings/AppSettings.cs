using System.Collections.Generic;

namespace Infinni.Deployer.Settings
{
    public class AppSettings
    {
        public const string FileName = "appsettings.json";

        public string PackageSource { get; set; }

        public string InstallDirectoryPath { get; set; }

        public Dictionary<string, string> PackageExecutables { get; set; }
    }
}