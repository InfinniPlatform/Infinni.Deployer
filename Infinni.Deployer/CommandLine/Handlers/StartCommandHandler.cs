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
                                   ISystemServiceManager systemServiceManager)
        {
            _appSettings = appSettings;
            _systemServiceManager = systemServiceManager;
        }

        private readonly AppSettings _appSettings;
        private readonly ISystemServiceManager _systemServiceManager;

        public Task Handle(StartOptions options)
        {
            var appPath = Path.Combine(_appSettings.InstallDirectoryPath, $"{options.PackageId}.{options.Version}");

            if (Directory.Exists(appPath) && Directory.EnumerateFileSystemEntries(appPath).Any())
            {
                _systemServiceManager.Start(options.PackageId, options.Version);

                return Task.CompletedTask;
            }

            Log.Information("Directory {AppPath} is empty.", appPath);

            return Task.CompletedTask;
        }
    }
}