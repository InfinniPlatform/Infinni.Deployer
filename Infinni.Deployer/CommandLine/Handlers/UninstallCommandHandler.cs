using System;
using System.IO;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class UninstallCommandHandler : ICommandHandler<UninstallOptions>
    {
        private readonly AppSettings _appSettings;
        private readonly ISystemServiceManager _systemServiceManager;

        public UninstallCommandHandler(AppSettings appSettings,
                                       ISystemServiceManager systemServiceManager)
        {
            _appSettings = appSettings;
            _systemServiceManager = systemServiceManager;
        }

        public Task Handle(UninstallOptions options)
        {
            var appDirectoryName = AppsHelper.GetAppDirectoryName(options.PackageId, options.Version);

            var installDirectoryPath = Path.GetFullPath(_appSettings.InstallDirectoryPath);

            var appDirectoryPath = Path.Combine(installDirectoryPath, appDirectoryName);

            try
            {
                Log.Information("Deleting application {PackageId} {Version}", options.PackageId, options.Version);

                Directory.Delete(appDirectoryPath, true);

                _systemServiceManager.Delete(options.PackageId, options.Version);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Uninstall command executed with error.");
            }

            return Task.CompletedTask;
        }

        
    }
}