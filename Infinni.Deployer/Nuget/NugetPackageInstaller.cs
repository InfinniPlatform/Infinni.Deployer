using System;
using System.Threading;
using System.Threading.Tasks;

using Infinni.Deployer.Settings;

using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;

namespace Infinni.Deployer.Nuget
{
    public class NugetPackageInstaller
    {
        private const string PackagesFolderPath = "packages";

        public NugetPackageInstaller(NugetSettings nugetSettings, AppSettings appSettings)
        {
            _nugetSettings = nugetSettings;
            _appSettings = appSettings;
        }

        private readonly AppSettings _appSettings;
        private readonly NugetSettings _nugetSettings;

        public async Task Install(string packageId, string version)
        {
            var sourceRepository = new SourceRepository(_nugetSettings.PackageSource.Value, _nugetSettings.ResourceProviders);
            var sourceRepositoryProvider = new SourceRepositoryProvider(_nugetSettings.Configuration.Value, _nugetSettings.ResourceProviders);
            var project = new FolderNuGetProject(_appSettings.InstallDirectoryPath);

            var nuGetPackageManager = new NuGetPackageManager(sourceRepositoryProvider, _nugetSettings.Configuration.Value, PackagesFolderPath)
                                          { PackagesFolderNuGetProject = project };

            var resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, false, false, VersionConstraints.None);
            var emptyNuGetProjectContext = new EmptyNuGetProjectContext();
            var sourceRepositories = Array.Empty<SourceRepository>();

            await nuGetPackageManager.InstallPackageAsync(project, new PackageIdentity(packageId, NuGetVersion.Parse(version)), resolutionContext, emptyNuGetProjectContext, sourceRepository, sourceRepositories, CancellationToken.None);
        }
    }
}