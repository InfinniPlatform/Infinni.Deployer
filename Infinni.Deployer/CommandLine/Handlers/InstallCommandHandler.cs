using System;
using System.IO;
using System.Threading.Tasks;

using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Nuget;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class InstallCommandHandler : ICommandHandler<InstallOptions>
    {
        public InstallCommandHandler(NugetPackageInstaller nugetPackageInstaller,
                                     ISystemServiceManager systemServiceManager,
                                     AppsManager appsManager)
        {
            _nugetPackageInstaller = nugetPackageInstaller;
            _systemServiceManager = systemServiceManager;
            _appsManager = appsManager;
        }

        private readonly NugetPackageInstaller _nugetPackageInstaller;
        private readonly ISystemServiceManager _systemServiceManager;
        private readonly AppsManager _appsManager;

        public async Task Handle(InstallOptions options)
        {
            foreach (var fullName in options.PackageFullNames)
            {
                var appInfo = AppInfo.FromPath(fullName);

                CheckExistingInstallation(appInfo);

                Log.Information("Installing application {PackageId}.{Version}.", appInfo.PackageId, appInfo.Version);
                await _nugetPackageInstaller.Install(appInfo);
                Log.Information("Application {PackageId}.{Version} successfully installed.", appInfo.PackageId, appInfo.Version);

                var binPath = _appsManager.GetExecutablePath(appInfo);

                _systemServiceManager.Create(appInfo, binPath);
            }
        }

        private void CheckExistingInstallation(AppInfo appInfo)
        {
            if (_appsManager.IsInstalled(appInfo))
            {
                Log.Error("Application {PackageId}.{Version} already installed.", appInfo.PackageId, appInfo.Version);
                throw new Exception();
            }
        }
    }
}