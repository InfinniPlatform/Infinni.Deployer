using System;
using System.IO;
using System.Threading.Tasks;

using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Nuget;
using Infinni.Deployer.Settings;

using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class InstallCommandHandler : ICommandHandler<InstallOptions>
    {
        public InstallCommandHandler(AppSettings appSettings,
                                     NugetPackageInstaller nugetPackageInstaller,
                                     ISystemServiceManager systemServiceManager)
        {
            _appSettings = appSettings;
            _nugetPackageInstaller = nugetPackageInstaller;
            _systemServiceManager = systemServiceManager;
        }

        private readonly AppSettings _appSettings;
        private readonly NugetPackageInstaller _nugetPackageInstaller;
        private readonly ISystemServiceManager _systemServiceManager;

        public async Task Handle(InstallOptions options)
        {
            foreach (var fullName in options.PackageFullNames)
            {
                var appInfo = AppInfo.FromPath(fullName);

                CheckExistingInstallation(appInfo);

                Log.Information("Installing application {PackageId}.{Version}.", appInfo.PackageId, appInfo.Version);
                await _nugetPackageInstaller.Install(appInfo);
                Log.Information("Application {PackageId}.{Version} successfully installed.", appInfo.PackageId, appInfo.Version);

                var binPath = Apps.GetExecutablePath(_appSettings.InstallDirectoryPath, appInfo);

                _systemServiceManager.Create(appInfo, binPath);
            }
        }

        private void CheckExistingInstallation(AppInfo appInfo)
        {
            var appDirectoryPath = Path.Combine(_appSettings.InstallDirectoryPath, appInfo.ToString());

            if (Directory.Exists(appDirectoryPath))
            {
                Log.Error("Application {PackageId}.{Version} already installed.", appInfo.PackageId, appInfo.Version);
                throw new Exception();
            }
        }
    }
}