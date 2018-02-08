using System;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Nuget;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class InstallCommandHandler : ICommandHandler<InstallOptions>
    {
        private readonly AppsManager _appsManager;

        private readonly NugetPackageInstaller _nugetPackageInstaller;
        private readonly ISystemServiceManager _systemServiceManager;

        public InstallCommandHandler(NugetPackageInstaller nugetPackageInstaller,
                                     ISystemServiceManager systemServiceManager,
                                     AppsManager appsManager)
        {
            _nugetPackageInstaller = nugetPackageInstaller;
            _systemServiceManager = systemServiceManager;
            _appsManager = appsManager;
        }

        public async Task Handle(InstallOptions options)
        {
            foreach (var fullName in options.PackageFullNames)
            {
                var path = fullName;
                var appInfo = AppInfo.FromPath(path);

                CheckExistingInstallation(appInfo);

                Log.Information("Installing application {FullAppName}.", appInfo.ToString());
                await _nugetPackageInstaller.Install(appInfo);
                Log.Information("Application {FullAppName} successfully installed.", appInfo.ToString());

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