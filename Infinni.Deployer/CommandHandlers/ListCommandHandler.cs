using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.Settings;
using NuGet.Protocol.Core.Types;
using Serilog;
using ILogger = NuGet.Common.ILogger;

namespace Infinni.Deployer.CommandHandlers
{
    public class ListCommandHandler : ICommandHandler<ListOptions>
    {
        private readonly ILogger _logger;
        private readonly NugetSettings _nugetSettings;

        public ListCommandHandler(NugetSettings nugetSettings,
                                  ILogger logger)
        {
            _nugetSettings = nugetSettings;
            _logger = logger;
        }

        public async Task Handle(ListOptions options)
        {
            Log.Information("Searching last {Count} versions of {PackageId} app in {Source}", options.Count, options.PackageId, _nugetSettings.PackageSource.SourceUri);

            var sourceRepository = new SourceRepository(_nugetSettings.PackageSource, _nugetSettings.ResourceProviders);

            var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();

            var searchMetadata = (await packageMetadataResource.GetMetadataAsync(options.PackageId, false, false, _logger, CancellationToken.None))
                .ToArray();

            Log.Information("Found {TotalCount}:", searchMetadata.Length);

            if (searchMetadata.Length > options.Count)
            {
                var packageSearchMetadatas = searchMetadata.OrderByDescending(m => m.Identity.Version)
                                                           .Take(options.Count);

                foreach (var metadata in packageSearchMetadatas)
                {
                    Log.Information("{PackageId}.{Version}.", metadata.Identity.Id, metadata.Identity.Version);
                }

                Log.Information("And {NotVisibleVersions} more.", searchMetadata.Length - options.Count);
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
    }
}