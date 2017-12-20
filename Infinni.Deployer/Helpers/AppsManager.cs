using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infinni.Deployer.Settings;

namespace Infinni.Deployer.Helpers
{
    public class AppsManager
    {
        private readonly AppSettings _appSettings;

        public AppsManager(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public bool IsInstalled(AppInfo appInfo)
        {
            EnsureInstallDirectory();

            return Directory.Exists(Path.Combine(Path.GetFullPath(_appSettings.InstallDirectoryPath), appInfo.ToString()));
        }

        public IEnumerable<AppInfo> GetAppsList()
        {
            EnsureInstallDirectory();

            var appDirectories = Directory.GetDirectories(_appSettings.InstallDirectoryPath);

            var appInfos = appDirectories.Select(AppInfo.FromPath);

            return appInfos;
        }

        public IEnumerable<AppInfo> GetByName(string appName)
        {
            EnsureInstallDirectory();

            var appDirectories = Directory.GetDirectories(_appSettings.InstallDirectoryPath);

            var appInfos = appDirectories.Select(AppInfo.FromPath)
                                         .Where(i => i.PackageId == appName);

            return appInfos;
        }

        public void EnsureInstallDirectory()
        {
            if (!Directory.Exists(_appSettings.InstallDirectoryPath))
            {
                Directory.CreateDirectory(_appSettings.InstallDirectoryPath);
            }
        }

        public string GetExecutablePath(AppInfo appInfo)
        {
            return Path.Combine(GetAppPath(appInfo), _appSettings.PackageExecutables[appInfo.PackageId]);
        }

        public string GetAppPath(AppInfo appInfo)
        {
            return Path.Combine(Path.GetFullPath(_appSettings.InstallDirectoryPath), appInfo.ToString());
        }
    }
}