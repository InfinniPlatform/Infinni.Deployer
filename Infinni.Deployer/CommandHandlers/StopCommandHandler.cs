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
            const string processName = "dotnet";
            var appDirectoryName = AppsHelper.GetAppDirectoryName(options.PackageId, options.Version);

            Log.Information("Stopping application {AppDirectoryName}", appDirectoryName);
            
            var processesByName = Process.GetProcessesByName(processName);

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
                            Log.Information("Found {ProcessName} process {ProcessID}", processName, process.Id);

                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                WindowsHelper.SendSigIntToApplication(process);

                                return Task.CompletedTask;
                            }

                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                LinuxHelper.SendSigIntToApplication(process);

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