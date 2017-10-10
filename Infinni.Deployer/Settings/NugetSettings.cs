using System;
using System.IO;
using System.Linq;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Infinni.Deployer.Settings
{
    public class NugetSettings
    {
        public NugetSettings(AppSettings appSettings)
        {
            ResourceProviders = Repository.Provider.GetCoreV3().ToArray();
            PackageSource = new PackageSource(appSettings.PackageSource);
            Configuration = NuGet.Configuration.Settings.LoadDefaultSettings(Directory.GetCurrentDirectory(), null, new MachineWideSettings());
        }

        public Lazy<INuGetResourceProvider>[] ResourceProviders { get; }
        public PackageSource PackageSource { get; }
        public ISettings Configuration { get; }
    }
}