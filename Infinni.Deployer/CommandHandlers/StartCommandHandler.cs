using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
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
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet.exe",
                    Arguments = $"{appPath}{Path.DirectorySeparatorChar}{options.ExecutableFile}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(processStartInfo);

                Log.Information("Process started.");
            }
            else
            {
                Log.Information("Directory {AppPath} is empty.", appPath);
            }


            return Task.CompletedTask;
        }
    }
}