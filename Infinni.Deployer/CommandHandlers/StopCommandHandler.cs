using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

            var processesByName = Process.GetProcessesByName("dotnet");

            foreach (var process in processesByName)
            {
                foreach (ProcessModule processModule in process.Modules)
                {
                    if (processModule.FileName.Contains(appDirectoryName))
                    {
                        var appInfo = AppsHelper.GetAppInfoFromPath(processModule.FileName);

                        if (string.IsNullOrEmpty(appInfo.PackageId))
                        {
                            Log.Warning("Provided path: {Path} does not contain packageId.", processModule.FileName);

                            continue;
                        }

                        if (string.IsNullOrEmpty(appInfo.Version))
                        {
                            Log.Warning("Provided path: {Path} does not contain version.", processModule.FileName);

                            continue;
                        }

                        try
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                var taskkill = Process.Start("taskkill", $"/pid {process.Id} /f");
                                taskkill.WaitForExit();

                                return Task.CompletedTask;
                            }

                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                var taskkill = Process.Start("kill", $"{process.Id}");
                                taskkill.WaitForExit();

                                return Task.CompletedTask;
                            }

                            Log.Information("Application {AppDirectoryName} successfully stopped.", appDirectoryName);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Unable to stop application {AppDirectoryName} with error: {Error}", appDirectoryName, e);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}