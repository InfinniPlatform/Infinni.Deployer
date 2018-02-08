using System.Collections.Generic;
using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("install", HelpText = "Install application.")]
    public class InstallOptions : ICommandOptions
    {
        [Value(0, Required = true, HelpText = "Full package name, e.g. AwesomePackage.1.2.3.4@InstanceName.")]
        public IEnumerable<string> PackageFullNames { get; set; }
    }
}