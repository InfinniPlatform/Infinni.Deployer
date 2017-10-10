using System.Diagnostics;
using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;
using Serilog;

namespace Infinni.Deployer.CommandHandlers
{
    public class StartCommandHandler : ICommandHandler<StartOptions>
    {
        public Task Handle(StartOptions options)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(processStartInfo);

            Log.Information("Process started.");

            return Task.CompletedTask;
        }
    }
}