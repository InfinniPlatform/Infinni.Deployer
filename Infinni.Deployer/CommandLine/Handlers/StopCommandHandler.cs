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
            var appInfo = new AppInfo(options.PackageId, options.Version);

            var appDirectoryName = appInfo.ToString();

            Log.Information("Stopping application {AppDirectoryName}", appDirectoryName);
            
            _systemServiceManager.Stop(appInfo);

            return Task.CompletedTask;
        }
    }
}