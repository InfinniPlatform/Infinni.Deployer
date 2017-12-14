using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Nuget;
using Infinni.Deployer.Settings;

using Newtonsoft.Json;

using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class ListCommandHandler : ICommandHandler<ListOptions>
    {
        public ListCommandHandler(NugetPackageSearcher nugetPackageSearcher,
                                  AppsManager appsManager,
                                  AppSettings appSettings)
        {
            _nugetPackageSearcher = nugetPackageSearcher;
            _appsManager = appsManager;
            _appSettings = appSettings;
        }

        private readonly AppsManager _appsManager;
        private readonly AppSettings _appSettings;
        private readonly NugetPackageSearcher _nugetPackageSearcher;

        public async Task Handle(ListOptions options)
        {
            if (options.ShowInstalled)
            {
                _appsManager.EnsureInstallDirectory();

                var apps = _appsManager.GetAppsList().ToArray();

                Log.Information("Found {DirectoriesCount} installed apps in {InstallDirectoryPath}.", apps.Length,
                                Path.GetFullPath(_appSettings.InstallDirectoryPath));

                if (apps.Length > 0)
                {
                    foreach (var appInfo in apps.Where(app => Directory.EnumerateFileSystemEntries(app.InstallPath)
                                                                       .Any())
                                                .OrderBy(app => app.Name))
                    {
                        Log.Information("{AppsInfo}", JsonConvert.SerializeObject(appInfo, Formatting.Indented));
                    }
                }

                return;
            }

            if (options.ShowAvailable)
            {
                await _nugetPackageSearcher.Search(options.PackageId, options.Take, options.IncludePrerelease);

                return;
            }

            Log.Error("Filter should be specified (--available or --installed).");
        }
    }
}