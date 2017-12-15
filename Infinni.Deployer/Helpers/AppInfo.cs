using System.Text.RegularExpressions;

namespace Infinni.Deployer.Helpers
{
    public class AppInfo
    {
        private static readonly Regex AppFullNameRegex = new Regex("(?!.*\\\\)(?<packageId>.+)\\.(?<version>\\d+\\.\\d+\\.\\d+\\.\\d+)(?<prerelease>\\-.+)?", RegexOptions.Compiled);

        public string PackageId { get; set; }

        public string Version { get; set; }

        public static AppInfo FromPath(string path)
        {
            var match = AppFullNameRegex.Match(path);

            var packageId = match.Groups["packageId"];
            var version = match.Groups["version"].Value;

            var isPrerelease = match.Groups["prerelease"].Success;

            return isPrerelease
                       ? new AppInfo
                       {
                           PackageId = packageId.Value,
                           Version = $"{version}{match.Groups["prerelease"].Value}"
                       }
                       : new AppInfo
                       {
                           PackageId = packageId.Value,
                           Version = version
                       };
        }

        public override string ToString()
        {
            return $"{PackageId}.{Version}";
        }
    }
}