using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class StopCommandHandler : ICommandHandler<StopOptions>
    {
        private readonly AppsManager _appsManager;

        private readonly ISystemServiceManager _systemServiceManager;

        public StopCommandHandler(ISystemServiceManager systemServiceManager,
                                  AppsManager appsManager)
        {
            _systemServiceManager = systemServiceManager;
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
            var appDirectoryName = appInfo.ToString();

            Log.Information("Stopping application {AppDirectoryName}", appDirectoryName);

            _systemServiceManager.Stop(appInfo);
        }
    }
}