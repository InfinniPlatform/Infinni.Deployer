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
            var packageId = options.PackageId;
            var version = options.Version;

            CheckExistingInstallation(packageId, version);

            Log.Information("Installing application {PackageId}.{Version}.", packageId, version);
            await _nugetPackageInstaller.Install(packageId, version);
            Log.Information("Application {PackageId}.{Version} successfully installed.", packageId, version);

            var binPath = Apps.GetExecutablePath(_appSettings.InstallDirectoryPath, packageId, version);

            _systemServiceManager.Create(packageId, version, binPath);
        }

        private void CheckExistingInstallation(string packageId, string version)
        {
            var appDirectoryPath = Path.Combine(_appSettings.InstallDirectoryPath, Apps.GetAppFullName(packageId, version));

            if (Directory.Exists(appDirectoryPath))
            {
                Log.Error("Application {PackageId}.{Version} already installed.", packageId, version);
                throw new Exception();
            }
        }
    }
}