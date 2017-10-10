using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Infinni.Deployer.CommandHandlers
{
    public class AppsCommandHandler : ICommandHandler<AppsOptions>
    {
        private readonly AppSettings _appSettings;

        public AppsCommandHandler(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Task Handle(AppsOptions opt)
        {
            if (!Directory.Exists(_appSettings.InstallDirectoryPath))
            {
                return Task.CompletedTask;
            }

            var directories = Directory.GetDirectories(_appSettings.InstallDirectoryPath);

            Log.Information("Found {DirectoriesCount} installed apps in {InstallDirectoryPath}.", directories.Length, Path.GetFullPath(_appSettings.InstallDirectoryPath));

            if (directories.Length <= 0)
            {
                return Task.CompletedTask;
            }

            foreach (var directory in directories.Where(d => Directory.EnumerateFileSystemEntries(d).Any()))
            {
                Log.Information("{AppsInfo}", JsonConvert.SerializeObject(new AppInfo(directory), Formatting.Indented));
            }

            return Task.CompletedTask;
        }


        public class AppInfo
        {
            private static readonly Regex Regex = new Regex("(?<name>\\w+)\\.(?<version>\\d\\.\\d+\\.\\d+\\.\\d+)", RegexOptions.Compiled);

            public AppInfo(string appInstallDirectory)
            {
                var match = Regex.Match(Path.GetFileName(appInstallDirectory));

                Name = match.Groups["name"].Value;
                Version = match.Groups["version"].Value;
                InstallPath = Path.GetFullPath(appInstallDirectory);
            }

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
}