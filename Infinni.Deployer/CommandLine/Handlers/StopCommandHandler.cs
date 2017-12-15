using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class StopCommandHandler : ICommandHandler<StopOptions>
    {
        private readonly AppSettings _appSettings;
        private readonly AppsManager _appsManager;
        private readonly ISystemServiceManager _systemServiceManager;

        public StopCommandHandler(ISystemServiceManager systemServiceManager,
                                  AppSettings appSettings,
                                  AppsManager appsManager)
        {
            _systemServiceManager = systemServiceManager;
            _appSettings = appSettings;
            _appsManager = appsManager;
        }

        public Task Handle(StopOptions options)
        {
            var fromConfig = options.PackageFullNamesArray.Value;

            if (fromConfig.Length > 0)
            {
                foreach (var appInfo in fromConfig.Select(AppInfo.FromPath))
                {
                    StopApp(appInfo);
                }
            }
            else
            {
                var appsList = _appsManager.GetAppsList();

                foreach (var appInfo in appsList)
                {
                    StopApp(appInfo);
                }
            }

            return Task.CompletedTask;
        }

        private void StopApp(AppInfo appInfo)
        {
            var appPath = Path.Combine(_appSettings.InstallDirectoryPath, appInfo.ToString());

            if (Directory.Exists(appPath) && Directory.EnumerateFileSystemEntries(appPath).Any())
            {
                _systemServiceManager.Stop(appInfo);
            }
            else
            {
                Log.Information("Directory {AppPath} is empty.", appPath);
            }
        }
    }
}