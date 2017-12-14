using System;
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

        public void Create(AppInfo appInfo, string executablePath)
        {
            if (executablePath.EndsWith("exe"))
            {
                throw new ArgumentException($"Aplication {appInfo.PackageId} is incompatible with Linux.");
            }

            string template;

            using (var streamReader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("Infinni.Deployer.template.service")))
            {
                template = streamReader.ReadToEnd();
            }

            var filledTemplate = template.Replace("{{description}}", "New ASP.NET Core service.")
                                         .Replace("{{dotnetExecutablePath}}", DotnetExecutable)
                                         .Replace("{{appExecutablePath}}", executablePath)
                                         .Replace("{{workingDirectory}}", Path.GetDirectoryName(executablePath));

            var serviceFileName = GetServiceFileName(appInfo);

            using (var fileStream = File.Create(Path.Combine(ServicesPath, serviceFileName)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(filledTemplate);
            }

            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Create), "daemon-reload");
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Create), $"enable {serviceFileName}");
        }

        public void Delete(AppInfo appInfo)
        {
            var serviceName = GetServiceName(appInfo);
            var serviceFileName = GetServiceFileName(appInfo);

            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Delete), $"disable {serviceName}");
            File.Delete(Path.Combine(ServicesPath, serviceFileName));
        }

        public void Start(AppInfo appInfo)
        {
            var serviceName = GetServiceName(appInfo);
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Start), $"start {serviceName}");
        }

        public void Stop(AppInfo appInfo)
        {
            var serviceName = GetServiceName(appInfo);
            ProcessExecutor.Execute(SystemCtlExecutable, nameof(Stop), $"stop {serviceName}");
        }

        private static string GetServiceName(AppInfo appInfo)
        {
            return $"{appInfo.PackageId}.{appInfo.Version}".ToLowerInvariant();
        }

        private static string GetServiceFileName(AppInfo appInfo)
        {
            return $"{GetServiceName(appInfo)}.service";
        }
    }
}