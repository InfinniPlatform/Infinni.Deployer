namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Manager for Windows services.
    /// </summary>
    public class ServiceControlWrapper : ISystemServiceManager
    {
        private const string ScExecutable = "sc.exe";
        private const string DotnetExecutable = "dotnet.exe";
        private const string WindowsServiceOption = "--windows-service";

        public void Create(string packageId, string version, string executablePath)
        {
            var arguments = executablePath.EndsWith("exe")
                ? $"create {packageId}.{version} DisplayName= \"{packageId}.{version}\" binpath= \"{executablePath}\""
                : $"create {packageId}.{version} DisplayName= \"{packageId}.{version}\" binpath= \"{DotnetExecutable} {executablePath} {WindowsServiceOption}\"";

            ProcessExecutor.Execute(ScExecutable, nameof(Create), arguments);
        }

        public void Delete(string packageId, string version)
        {
            var arguments = $"delete {packageId}.{version}";

            ProcessExecutor.Execute(ScExecutable, nameof(Delete), arguments);
        }

        public void Start(string packageId, string version)
        {
            var arguments = $"start {packageId}.{version}";

            ProcessExecutor.Execute(ScExecutable, nameof(Start), arguments);
        }

        public void Stop(string packageId, string version)
        {
            var arguments = $"stop {packageId}.{version}";

            ProcessExecutor.Execute(ScExecutable, nameof(Stop), arguments);
        }
    }
}