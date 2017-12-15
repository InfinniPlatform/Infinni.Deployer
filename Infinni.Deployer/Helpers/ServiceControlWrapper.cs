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
        private const string SetWorkingDirectoryOption = "--set-working-directory";

        public void Create(AppInfo appInfo, string executablePath)
        {
            var arguments = executablePath.EndsWith(".exe")
                ? $"create {appInfo} DisplayName= \"{appInfo}\" binpath= \"{executablePath} {WindowsServiceOption} {SetWorkingDirectoryOption}\""
                : $"create {appInfo} DisplayName= \"{appInfo}\" binpath= \"{DotnetExecutable} {executablePath} {WindowsServiceOption} {SetWorkingDirectoryOption}\"";

            ProcessExecutor.Execute(ScExecutable, nameof(Create), arguments);
        }

        public void Delete(AppInfo appInfo)
        {
            var arguments = $"delete {appInfo}";

            ProcessExecutor.Execute(ScExecutable, nameof(Delete), arguments);
        }

        public void Start(AppInfo appInfo)
        {
            var arguments = $"start {appInfo}";

            ProcessExecutor.Execute(ScExecutable, nameof(Start), arguments);
        }

        public void Stop(AppInfo appInfo)
        {
            var arguments = $"stop {appInfo}";

            ProcessExecutor.Execute(ScExecutable, nameof(Stop), arguments);
        }
    }
}