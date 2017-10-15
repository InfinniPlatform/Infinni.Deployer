using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Serilog;

namespace Infinni.Deployer.CommandHandlers
{
    public class UninstallCommandHandler : ICommandHandler<UninstallOptions>
    {
        private readonly AppSettings _appSettings;

        public UninstallCommandHandler(AppSettings appSettings)
        {
            _appSettings = appSettings;
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

                UninstallService(options.PackageId, options.Version);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Uninstall command executed with error.");
            }

            return Task.CompletedTask;
        }

        private static void UninstallService(string packageId, string version)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ServiceControlWrapper.Delete(packageId, version);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SystemCtlWrapper.Delete(packageId, version);
            }
        }
    }
}