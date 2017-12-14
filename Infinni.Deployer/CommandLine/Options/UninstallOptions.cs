using System.Collections.Generic;

using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("uninstall", HelpText = "Uninstall application.")]
    public class UninstallOptions : ICommandOptions
    {
        [Value(0)]
        public IEnumerable<string> PackageFullNames { get; set; }
    }
}