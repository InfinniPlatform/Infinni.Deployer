using System.Diagnostics;
using System.IO;
using System.Reflection;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    public static class SystemCtlWrapper
    {
        private const string SystemCtlExecutable = "systemctl";
        private const string DotnetExecutable = "/usr/bin/dotnet";
        private const string ServicesPath = "/lib/systemd/system/";

        public static void Create(string packageId, string version, string binPath)
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

            var serviceFilename = $"{packageId}.{version}.service".ToLowerInvariant();

            using (var fileStream = File.Create(Path.Combine(ServicesPath, serviceFilename)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(filledTemplate);
            }

            Execute(nameof(Create), "daemon-reload");
            Execute(nameof(Create), $"enable {serviceFilename}");
        }

        public static void Delete(string packageId, string version)
        {
            Execute(nameof(Delete), $"disable {packageId}.{version}");
            Execute(nameof(Delete), "");
        }

        public static void Start(string packageId, string version)
        {
            Execute(nameof(Start), $"start {packageId}.{version}");
            File.Delete(Path.Combine(ServicesPath, $"{packageId}.{version}.service"));
        }

        public static void Stop(string packageId, string version)
        {
            Execute(nameof(Stop), $"stop {packageId}.{version}");
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
    }
}