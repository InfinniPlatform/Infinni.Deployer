using System.IO;
using System.Reflection;

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

        public void Create(string packageId, string version, string executablePath)
        {
            string template;

            using (var streamReader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("Infinni.Deployer.template.service")))
            {
                template = streamReader.ReadToEnd();
            }

            var filledTemplate = template.Replace("{{description}}", "New ASP.NET Core service.")
                                         .Replace("{{dotnetExecutablePath}}", DotnetExecutable)
                                         .Replace("{{appExecutablePath}}", executablePath)
                                         .Replace("{{workingDirectory}}", Path.GetDirectoryName(executablePath));

            var serviceFileName = GetServiceFileName(packageId, version);

            using (var fileStream = File.Create(Path.Combine(ServicesPath, serviceFileName)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(filledTemplate);
            }

            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Create), "daemon-reload");
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Create), $"enable {serviceFileName}");
        }

        public void Delete(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            var serviceFileName = GetServiceFileName(packageId, version);

            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Delete), $"disable {serviceName}");
            File.Delete(Path.Combine(ServicesPath, serviceFileName));
        }

        public void Start(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Start), $"start {serviceName}");
        }

        public void Stop(string packageId, string version)
        {
            var serviceName = GetServiceName(packageId, version);
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Stop), $"stop {serviceName}");
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