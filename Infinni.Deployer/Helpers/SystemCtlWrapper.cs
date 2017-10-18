using System.Diagnostics;
using System.IO;
using System.Reflection;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Manager for Linux daemons.
    /// </summary>
    public class SystemCtlWrapper : ISystemServiceManager
    {
        private const string SystemCtlExecutable = "systemctl";
        private const string DotnetExecutable = "/usr/bin/dotnet";
        private const string ServicesPath = "/lib/systemd/system/";

        public void Create(string packageId, string version, string binPath)
        {
            string template;

            using (var streamReader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("Infinni.Deployer.template.service")))
            {
                template = streamReader.ReadToEnd();
            }

            var filledTemplate = template.Replace("{{description}}", "New ASP.NET Core service.")
                                         .Replace("{{dotnetExecutable}}", DotnetExecutable)
                                         .Replace("{{binPath}}", binPath)
                                         .Replace("{{workingDirectory}}", Path.GetDirectoryName(binPath));

            var serviceFileName = GetServiceFileName(packageId, version);

            using (var fileStream = File.Create(Path.Combine(ServicesPath, serviceFileName)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(filledTemplate);
            }

            Execute(nameof(Create), "daemon-reload");
            Execute(nameof(Create), $"enable {serviceFileName}");
        }

        public void Start(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            Execute(nameof(Start), $"start {serviceName}");
        }

        public void Stop(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            Execute(nameof(Stop), $"stop {serviceName}");
        }

        public void Delete(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            var serviceFileName = GetServiceFileName(packageId, version);

            Execute(nameof(Delete), $"disable {serviceName}");
            File.Delete(Path.Combine(ServicesPath, serviceFileName));
        }

        private static void Execute(string commandName, string arguments)
        {
            Log.Information("Executing {File} {arguments}", SystemCtlExecutable, arguments);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = SystemCtlExecutable,
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

        private static string GetServiceName(string packageId, string version)
        {
            return $"{packageId}.{version}".ToLowerInvariant();
        }

        private static string GetServiceFileName(string packageId, string version)
        {
            return $"{GetServiceName(packageId, version)}.service";
        }
    }
}