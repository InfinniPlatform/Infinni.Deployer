using System.Diagnostics;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Manager for Windows services.
    /// </summary>
    public class ServiceControlWrapper : ISystemServiceManager
    {
        private const string ScExecutable = "sc.exe";
        private const string DotnetExecutable = "dotnet.exe";
        private const string WindowsServiceKey = "--windowsService";

        public void Create(string packageId, string version, string binPath)
        {
            var arguments = $"create {packageId}.{version} DisplayName= \"{packageId}.{version}\" binpath= \"{DotnetExecutable} {binPath} {WindowsServiceKey}\"";

            Execute(nameof(Create), arguments);
        }

        public void Delete(string packageId, string version)
        {
            var arguments = $"delete {packageId}.{version}";

            Execute(nameof(Delete), arguments);
        }

        public void Start(string packageId, string version)
        {
            var arguments = $"start {packageId}.{version}";

            Execute(nameof(Start), arguments);
        }

        public void Stop(string packageId, string version)
        {
            var arguments = $"stop {packageId}.{version}";

            Execute(nameof(Stop), arguments);
        }

        private void Execute(string commandName, string arguments)
        {
            Log.Information("Executing {File} {arguments}", ScExecutable, arguments);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ScExecutable,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += (sender, args) => Log.Information(args.Data);
            process.ErrorDataReceived += (sender, args) => Log.Error(args.Data);
            process.WaitForExit();

            Log.Information("{CommandName} command completed.", commandName);
        }
    }
}