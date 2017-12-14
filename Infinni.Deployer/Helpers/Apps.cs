using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Infinni.Deployer.Helpers
{
    public static class Apps
    {
        private static readonly Dictionary<string, string> Executables = new Dictionary<string, string>
                                                                         {
                                                                             { "Habinet", "Habinet.Core.dll" },
                                                                             { "Habinet.Notifications", "Habinet.Notifications.exe" }
                                                                         };

        public static string GetExecutablePath(string installDirectoryPath, string packageId, string version)
        {
            return Path.Combine(Path.GetFullPath(installDirectoryPath),
                                GetAppFullName(packageId, version),
                                Executables[packageId]);
        }

        public static string GetExecutablePath(string installDirectoryPath, AppInfo appInfo)
        {
            return Path.Combine(Path.GetFullPath(installDirectoryPath),
                                appInfo.ToString(),
                                Executables[appInfo.PackageId]);
        }

        public static string GetAppFullName(string packageId, string version)
        {
            return $"{packageId}.{version}";
        }
    }
}