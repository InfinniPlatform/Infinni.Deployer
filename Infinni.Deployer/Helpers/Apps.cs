using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Infinni.Deployer.CommandLine.Handlers;

namespace Infinni.Deployer.Helpers
{
    public static class Apps
    {
        private static readonly Regex AppDirectoryRegex = new Regex("(?<packageId>\\w+)\\.(?<version>\\d\\.\\d+\\.\\d+\\.\\d+)", RegexOptions.Compiled);

        private static readonly Dictionary<string, string> Executables = new Dictionary<string, string>
        {
            {"Habinet", "Habinet.Core.dll"}
        };

        public static string GetExecutablePath(string installDirectoryPath, string packageId, string version)
        {
            return Path.Combine(Path.GetFullPath(installDirectoryPath),
                                GetAppFullName(packageId, version),
                                Executables[packageId]);
        }

        public static string GetAppFullName(string packageId, string version)
        {
            return $"{packageId}.{version}";
        }

        public static AppInfo GetInfoByPath(string path)
        {
            var match = AppDirectoryRegex.Match(path);

            var packageId = match.Groups["packageId"].Value;
            var version = match.Groups["version"].Value;

            var appInfo = new AppInfo(packageId, version, path);

            return appInfo;
        }
    }
}