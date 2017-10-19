using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
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
                Log.Information("{AppsInfo}", JsonConvert.SerializeObject(Apps.GetInfoByPath(directory), Formatting.Indented));
            }

            return Task.CompletedTask;
        }
    }
}