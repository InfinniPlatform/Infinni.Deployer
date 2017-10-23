using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class AppsCommandHandler : ICommandHandler<AppsOptions>
    {
        private readonly AppSettings _appSettings;
        private readonly AppsManager _appsManager;

        public AppsCommandHandler(AppSettings appSettings,
                                  AppsManager appsManager)
        {
            _appSettings = appSettings;
            _appsManager = appsManager;
        }

        public Task Handle(AppsOptions opt)
        {
            _appsManager.EnsureInstallDirectory();

            var apps = _appsManager.GetAppsList().ToArray();

            Log.Information("Found {DirectoriesCount} installed apps in {InstallDirectoryPath}.", apps.Length, Path.GetFullPath(_appSettings.InstallDirectoryPath));

            if (apps.Length > 0)
            {
                foreach (var appInfo in apps.Where(app => Directory.EnumerateFileSystemEntries(app.InstallPath)
                                                                   .Any())
                                            .OrderBy(app => app.Name))
                {
                    Log.Information("{AppsInfo}", JsonConvert.SerializeObject(appInfo, Formatting.Indented));
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}