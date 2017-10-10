using System;
using System.Collections.Generic;
using NuGet.Common;
using NuGet.Configuration;

namespace Infinni.Deployer.Settings
{
    public class MachineWideSettings : IMachineWideSettings
    {
        private readonly Lazy<IEnumerable<NuGet.Configuration.Settings>> _settings;

        public MachineWideSettings()
        {
            _settings = new Lazy<IEnumerable<NuGet.Configuration.Settings>>(() =>
            {
                var folderPath = NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory);

                return NuGet.Configuration.Settings.LoadMachineWideSettings(folderPath);
            });
        }

        public IEnumerable<NuGet.Configuration.Settings> Settings => _settings.Value;
    }
}