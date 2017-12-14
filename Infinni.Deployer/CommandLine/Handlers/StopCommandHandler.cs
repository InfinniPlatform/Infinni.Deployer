using System.Threading.Tasks;

using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;

using Serilog;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class StopCommandHandler : ICommandHandler<StopOptions>
    {
        public StopCommandHandler(ISystemServiceManager systemServiceManager)
        {
            _systemServiceManager = systemServiceManager;
        }

        private readonly ISystemServiceManager _systemServiceManager;

        public Task Handle(StopOptions options)
        {
            var appDirectoryName = Apps.GetAppFullName(options.PackageId, options.Version);

            Log.Information("Stopping application {AppDirectoryName}", appDirectoryName);

            _systemServiceManager.Stop(options.PackageId, options.Version);

            return Task.CompletedTask;
        }
    }
}