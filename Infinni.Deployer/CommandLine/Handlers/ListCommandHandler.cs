using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Nuget;
using Infinni.Deployer.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class ListCommandHandler : ICommandHandler<ListOptions>
    {
        private readonly AppSettings _appSettings;

        private readonly AppsManager _appsManager;
        private readonly NugetPackageSearcher _nugetPackageSearcher;

        public ListCommandHandler(NugetPackageSearcher nugetPackageSearcher,
                                  AppsManager appsManager,
                                  AppSettings appSettings)
        {
            _nugetPackageSearcher = nugetPackageSearcher;
            _appsManager = appsManager;
            _appSettings = appSettings;
        }

        public async Task Handle(ListOptions options)
        {
            if (options.ShowInstalled)
            {
                ShowInstalled();

                return;
            }

            if (options.ShowAvailable)
            {
                await ShowAvailable(options);

                return;
            }

            ShowInstalled();
        }

        private void ShowInstalled()
        {
            _appsManager.EnsureInstallDirectory();

            var apps = _appsManager.GetAppsList().ToArray();

            Log.Information("Found {DirectoriesCount} installed apps in {InstallDirectoryPath}.", apps.Length,
                            Path.GetFullPath(_appSettings.InstallDirectoryPath));

            if (apps.Length > 0)
            {
                foreach (var appInfo in apps.Where(app => Directory.EnumerateFileSystemEntries(_appsManager.GetAppPath(app)).Any())
                                            .OrderBy(app => app.PackageId))
                {
                    Log.Information("{AppsInfo}", JsonConvert.SerializeObject(appInfo, Formatting.Indented));
                }
            }
        }

        private async Task ShowAvailable(ListOptions options)
        {
            await _nugetPackageSearcher.Search(options.PackageId, options.Take, options.IncludePrerelease);
        }
    }
}