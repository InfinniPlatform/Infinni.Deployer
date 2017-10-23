using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Settings;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class AppsManager
    {
        private readonly AppSettings _appSettings;

        public AppsManager(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public bool IsInstalled(string appName, string appVersion)
        {
            EnsureInstallDirectory();

            return Directory.Exists(GetFullPath(appName, appVersion));
        }

        public IEnumerable<AppInfo> GetAppsList()
        {
            EnsureInstallDirectory();

            var appDirectories = Directory.GetDirectories(_appSettings.InstallDirectoryPath);

            var appInfos = appDirectories.Select(Apps.GetInfoByPath);

            return appInfos;
        }

        public IEnumerable<AppInfo> GetByName(string appName)
        {
            EnsureInstallDirectory();

            var appDirectories = Directory.GetDirectories(_appSettings.InstallDirectoryPath);

            var appInfos = appDirectories.Select(Apps.GetInfoByPath)
                                         .Where(i => i.Name == appName);

            return appInfos;
        }

        public void EnsureInstallDirectory()
        {
            if (!Directory.Exists(_appSettings.InstallDirectoryPath))
            {
                Directory.CreateDirectory(_appSettings.InstallDirectoryPath);
            }
        }

        private string GetFullPath(string appName, string appVersion)
        {
            return Path.Combine(Path.GetFullPath(_appSettings.InstallDirectoryPath), Apps.GetAppFullName(appName, appVersion));
        }
    }
}