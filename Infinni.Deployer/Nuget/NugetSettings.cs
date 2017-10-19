using System;
using System.IO;
using System.Linq;
using Infinni.Deployer.Settings;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Infinni.Deployer.Nuget
{
    public class NugetSettings
    {
        public NugetSettings(AppSettings appSettings)
        {
            ResourceProviders = Repository.Provider.GetCoreV3().ToArray();
            PackageSource = new Lazy<PackageSource>(new PackageSource(appSettings.PackageSource));
            Configuration = new Lazy<ISettings>(NuGet.Configuration.Settings.LoadDefaultSettings(Directory.GetCurrentDirectory(), null, new MachineWideSettings()));
        }

        public Lazy<INuGetResourceProvider>[] ResourceProviders { get; }
        public Lazy<PackageSource> PackageSource { get; }
        public Lazy<ISettings> Configuration { get; }
    }
}