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
        public UninstallCommandHandler(AppSettings appSettings,
                                       ISystemServiceManager systemServiceManager)
        {
            _appSettings = appSettings;
            _systemServiceManager = systemServiceManager;
        }

        private readonly AppSettings _appSettings;
        private readonly ISystemServiceManager _systemServiceManager;

        public Task Handle(UninstallOptions options)
        {
            foreach (var fullName in options.PackageFullNames)
            {
                var appInfo = AppInfo.FromPath(fullName);

                var installDirectoryPath = Path.GetFullPath(_appSettings.InstallDirectoryPath);

                var appDirectoryPath = Path.Combine(installDirectoryPath, fullName);

                try
                {
                    Log.Information("Deleting application {PackageId} {Version}", appInfo.PackageId, appInfo.Version);

                    try
                    {
                        _systemServiceManager.Stop(appInfo);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    

                    if (Directory.Exists(appDirectoryPath))
                    {
                        Directory.Delete(appDirectoryPath, true);
                    }

                    _systemServiceManager.Delete(appInfo);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Uninstall command executed with error.");
                }
            }

            return Task.CompletedTask;
        }
    }
}