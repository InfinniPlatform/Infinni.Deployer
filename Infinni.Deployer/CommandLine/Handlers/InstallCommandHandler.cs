using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class InstallCommandHandler : ICommandHandler<InstallOptions>
    {
        private const string PackagesFolderPath = "packages";
        private readonly AppSettings _appSettings;
        private readonly NugetSettings _nugetSettings;
        private readonly ISystemServiceManager _systemServiceManager;

        public InstallCommandHandler(AppSettings appSettings,
                                     NugetSettings nugetSettings,
                                     ISystemServiceManager systemServiceManager)
        {
            _appSettings = appSettings;
            _nugetSettings = nugetSettings;
            _systemServiceManager = systemServiceManager;
        }

        public async Task Handle(InstallOptions options)
        {
            Log.Information("Installing application {PackageId}.{Version}.", options.PackageId, options.Version);

            CheckExistingInstallation(options.PackageId, options.Version);

            var sourceRepository = new SourceRepository(_nugetSettings.PackageSource, _nugetSettings.ResourceProviders);
            var sourceRepositoryProvider = new SourceRepositoryProvider(_nugetSettings.Configuration, _nugetSettings.ResourceProviders);
            var project = new FolderNuGetProject(_appSettings.InstallDirectoryPath);
            var nuGetPackageManager = new NuGetPackageManager(sourceRepositoryProvider, _nugetSettings.Configuration, PackagesFolderPath)
            {
                PackagesFolderNuGetProject = project
            };
            var resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, false, false, VersionConstraints.None);
            var emptyNuGetProjectContext = new EmptyNuGetProjectContext();
            var sourceRepositories = Array.Empty<SourceRepository>();

            try
            {
                await nuGetPackageManager.InstallPackageAsync(project, new PackageIdentity(options.PackageId, NuGetVersion.Parse(options.Version)), resolutionContext, emptyNuGetProjectContext, sourceRepository, sourceRepositories, CancellationToken.None);
            }
            catch (Exception e)
            {
                Log.Error("Installation of {PackageId}.{Version} completed with error {Exception}.", options.PackageId, options.Version, e);
            }

            Log.Information("Application {PackageId}.{Version} successfully installed.", options.PackageId, options.Version);

            var binPath = Path.Combine(Path.GetFullPath(_appSettings.InstallDirectoryPath),
                                       AppsHelper.GetAppDirectoryName(options.PackageId, options.Version),
                                       "Habinet.Core.dll");

            _systemServiceManager.Create(options.PackageId, options.Version, binPath);
        }

        private void CheckExistingInstallation(string packageId, string version)
        {
            var appDirectoryPath = Path.Combine(_appSettings.InstallDirectoryPath, AppsHelper.GetAppDirectoryName(packageId, version));

            if (Directory.Exists(appDirectoryPath))
            {
                Log.Error("Application {PackageId}.{Version} already installed.", packageId, version);
            }
        }
    }
}