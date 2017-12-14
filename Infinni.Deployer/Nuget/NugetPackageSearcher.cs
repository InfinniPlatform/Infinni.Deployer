using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Protocol.Core.Types;

using Serilog;

using ILogger = NuGet.Common.ILogger;

namespace Infinni.Deployer.Nuget
{
    public class NugetPackageSearcher
    {
        public NugetPackageSearcher(NugetSettings nugetSettings,
                                    ILogger logger)
        {
            _nugetSettings = nugetSettings;
            _logger = logger;
        }

        private readonly ILogger _logger;
        private readonly NugetSettings _nugetSettings;

        public async Task Search(string packageId, int count, bool includePrerelease)
        {
            EnsurePackageId(packageId);

            Log.Information("Searching last {Take} versions of {PackageId} app in {Source}", count, packageId, _nugetSettings.PackageSource.Value.SourceUri);

            var sourceRepository = new SourceRepository(_nugetSettings.PackageSource.Value, _nugetSettings.ResourceProviders);

            var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();

            var searchMetadata = (await packageMetadataResource.GetMetadataAsync(packageId, includePrerelease, false, _logger, CancellationToken.None)).ToArray();

            Log.Information("Found {TotalCount}:", searchMetadata.Length);

            if (searchMetadata.Length > count)
            {
                var packageSearchMetadatas = searchMetadata.OrderByDescending(m => m.Identity.Version)
                                                           .Take(count);

                foreach (var metadata in packageSearchMetadatas)
                {
                    Log.Information("{PackageId}.{Version}.", metadata.Identity.Id, metadata.Identity.Version);
                }

                Log.Information("And {NotVisibleVersions} more.", searchMetadata.Length - count);
            }
            else
            {
                var packageSearchMetadatas = searchMetadata.OrderByDescending(m => m.Identity.Version)
                                                           .Take(searchMetadata.Length);

                foreach (var metadata in packageSearchMetadatas)
                {
                    Log.Information("{PackageId}.{Version}.", metadata.Identity.Id, metadata.Identity.Version);
                }
            }
        }

        private static void EnsurePackageId(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentException("PackageId should be specified.");
            }
            
        }
    }
}