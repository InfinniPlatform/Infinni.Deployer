using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;
using Serilog;

namespace Infinni.Deployer.CommandHandlers
{
    public class StartCommandHandler : ICommandHandler<StartOptions>
    {
        private readonly AppSettings _appSettings;

        public StartCommandHandler(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Task Handle(StartOptions options)
        {
            var appPath = Path.Combine(_appSettings.InstallDirectoryPath, $"{options.PackageId}.{options.Version}");

            if (Directory.Exists(appPath) && Directory.EnumerateFileSystemEntries(appPath).Any())
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ServiceControlWrapper.Start(options.PackageId, options.Version);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                Log.Information("Directory {AppPath} is empty.", appPath);
            }


            return Task.CompletedTask;
        }
    }
}