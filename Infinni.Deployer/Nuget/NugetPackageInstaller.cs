using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Infinni.Deployer.Helpers;
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

        private readonly AppSettings _appSettings;
        private readonly NugetSettings _nugetSettings;

        public NugetPackageInstaller(NugetSettings nugetSettings, AppSettings appSettings)
        {
            _nugetSettings = nugetSettings;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Installs application package.
        /// </summary>
        /// <remarks>
        /// На текущий момент это вся документация по использованию библиотек Nuget:
        ///  https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-1
        ///  https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-2
        ///  https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-3
        /// </remarks>
        /// <returns></returns>
        public async Task Install(AppInfo appInfo)
        {
            var sourceRepository = new SourceRepository(_nugetSettings.PackageSource.Value, _nugetSettings.ResourceProviders);
            var sourceRepositoryProvider = new SourceRepositoryProvider(_nugetSettings.Configuration.Value, _nugetSettings.ResourceProviders);
            var project = new FolderNuGetProject(_appSettings.InstallDirectoryPath);

            var nuGetPackageManager = new NuGetPackageManager(sourceRepositoryProvider, _nugetSettings.Configuration.Value, PackagesFolderPath)
                {PackagesFolderNuGetProject = project};

            var resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, true, false, VersionConstraints.None);
            var emptyNuGetProjectContext = new EmptyNuGetProjectContext();
            var sourceRepositories = Array.Empty<SourceRepository>();

            await nuGetPackageManager.InstallPackageAsync(project, new PackageIdentity(appInfo.PackageId, NuGetVersion.Parse(appInfo.Version)), resolutionContext, emptyNuGetProjectContext, sourceRepository, sourceRepositories, CancellationToken.None);

            if (appInfo.HasInstance())
            {
                var directoryName = Path.Combine(_appSettings.InstallDirectoryPath, $"{appInfo.PackageId}.{appInfo.Version}");
                var directoryForInstanceName = Path.Combine(_appSettings.InstallDirectoryPath, appInfo.ToString());

                var directoryInfo = new DirectoryInfo(directoryName);

                if (directoryInfo.Exists && !Directory.Exists(directoryForInstanceName))
                {
                    directoryInfo.MoveTo(directoryForInstanceName);
                }
            }
        }
    }
}