using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class StartCommandHandler : ICommandHandler<StartOptions>
    {
        public StartCommandHandler(AppSettings appSettings,
                                   ISystemServiceManager systemServiceManager,
                                   AppsManager appsManager)
        {
            _appSettings = appSettings;
            _systemServiceManager = systemServiceManager;
            _appsManager = appsManager;
        }

        private readonly AppSettings _appSettings;
        private readonly ISystemServiceManager _systemServiceManager;
        private readonly AppsManager _appsManager;

        public Task Handle(StartOptions options)
        {
            var fromConfig = options.PackageFullNamesArray.Value;

            if (fromConfig.Length > 0)
            {
                foreach (var appInfo in fromConfig.Select(AppInfo.FromPath))
                {
                    StartApp(appInfo);
                }
            }
            else
            {
                var appsList = _appsManager.GetAppsList();
                foreach (var appInfo in appsList)
                {
                    StartApp(appInfo);
                }
            }

            return Task.CompletedTask;
        }

        private void StartApp(AppInfo appInfo)
        {
            var appPath = Path.Combine(_appSettings.InstallDirectoryPath, appInfo.ToString());

            if (Directory.Exists(appPath) && Directory.EnumerateFileSystemEntries(appPath).Any())
            {
                _systemServiceManager.Start(appInfo);
            }

            Log.Information("Directory {AppPath} is empty.", appPath);
        }
    }
}