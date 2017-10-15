using System.Text.RegularExpressions;
using Infinni.Deployer.CommandHandlers;

namespace Infinni.Deployer.Helpers
{
    public static class AppsHelper
    {
        private static readonly Regex AppDirectoryRegex = new Regex("(?<packageId>\\w+)\\.(?<version>\\d\\.\\d+\\.\\d+\\.\\d+)", RegexOptions.Compiled);

        public static string GetAppDirectoryName(string packageId, string version)
        {
            return $"{packageId}.{version}";
        }

        public static AppInfo GetAppInfoFromPath(string path)
        {
            var match = AppDirectoryRegex.Match(path);

            var packageId = match.Groups["packageId"].Value;
            var version = match.Groups["version"].Value;

            var appInfo = new AppInfo(packageId, version, path);

            return appInfo;
        }
    }
}