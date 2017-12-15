using System.Collections.Generic;
using System.IO;

namespace Infinni.Deployer.Helpers
{
    public static class Apps
    {
        private static readonly Dictionary<string, string> Executables = new Dictionary<string, string>
        {
            {"Habinet", "Habinet.Core.dll"},
            {"Habinet.Notifications", "Habinet.Notifications.exe"}
        };

        public static string GetExecutablePath(string installDirectoryPath, AppInfo appInfo)
        {
            return Path.Combine(GetAppPath(installDirectoryPath, appInfo), Executables[appInfo.PackageId]);
        }

        public static string GetAppPath(string installDirectoryPath, AppInfo appInfo)
        {
            return Path.Combine(Path.GetFullPath(installDirectoryPath), appInfo.ToString());
        }
    }
}