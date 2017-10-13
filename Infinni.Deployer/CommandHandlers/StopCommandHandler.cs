using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Infinni.Deployer.ApplicationHelpers;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.Helpers;
using Serilog;

namespace Infinni.Deployer.CommandHandlers
{
    public class StopCommandHandler : ICommandHandler<StopOptions>
    {
        public Task Handle(StopOptions options)
        {
            var appDirectoryName = AppsHelper.GetAppDirectoryName(options.PackageId, options.Version);

            Log.Information("Stopping application {AppDirectoryName}", appDirectoryName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ServiceControlWrapper.Stop(options.PackageId, options.Version);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException();
            }

            return Task.CompletedTask;
        }
    }
}