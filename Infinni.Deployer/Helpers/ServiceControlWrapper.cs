using System.Diagnostics;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    public static class ServiceControlWrapper
    {
        private const string FileName = "sc.exe";

        public static void Create(string packageId, string version, string binPath)
        {
            var arguments = $"create {packageId}.{version} DisplayName= \"{packageId}.{version}\" binpath= \"{binPath} --asService\"";

            Execute(nameof(Create), arguments);
        }

        public static void Delete(string packageId, string version)
        {
            var arguments = $"delete {packageId}.{version}";

            Execute(nameof(Delete), arguments);
        }

        public static void Start(string packageId, string version)
        {
            var arguments = $"start {packageId}.{version}";

            Execute(nameof(Start), arguments);
        }

        public static void Stop(string packageId, string version)
        {
            var arguments = $"stop {packageId}.{version}";

            Execute(nameof(Stop), arguments);
        }

        private static void Execute(string commandName, string arguments)
        {
            Log.Information("Executing {File} {arguments}", FileName, arguments);

            var process = Process.Start(FileName, arguments);
            process.WaitForExit();

            Log.Information("{CommandName} command completed.", commandName);
        }
    }
}