using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    public class AppInfo
    {
        private static readonly Regex AppFullNameRegex = new Regex("(?!.*\\\\)(?<packageId>.+)\\.(?<version>\\d+\\.\\d+\\.\\d+\\.\\d+)(?<prerelease>\\-.+)?\\@(?<instance>.+)", RegexOptions.Compiled);

        public string PackageId { get; set; }

        public string Version { get; set; }

        public string Instance { get; set; }

        public static AppInfo FromPath(string path)
        {
            var match = AppFullNameRegex.Match(path);

            var packageId = match.Groups["packageId"].Value;
            var version = match.Groups["version"].Value;

            var isPrerelease = match.Groups["prerelease"].Success;
            var isInstanceSpecified = match.Groups["instance"].Success;

            if (isInstanceSpecified && match.Groups["instance"].Value.Any(char.IsUpper))
            {
                Log.Warning("Instance name will be converted to lower case.");
            }

            var appInfo = new AppInfo
            {
                PackageId = packageId,
                Version = isPrerelease ? $"{version}{match.Groups["prerelease"].Value}" : version,
                Instance = isInstanceSpecified ? match.Groups["instance"].Value.ToLowerInvariant() : null
            };

            return appInfo;
        }

        public bool HasInstance()
        {
            return !string.IsNullOrEmpty(Instance);
        }

        public override string ToString()
        {
            return HasInstance()
                       ? $"{PackageId}.{Version}@{Instance}"
                       : $"{PackageId}.{Version}";
        }
    }
}