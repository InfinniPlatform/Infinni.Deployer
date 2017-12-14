using System.Collections.Generic;

using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("install", HelpText = "Install application.")]
    public class InstallOptions : ICommandOptions
    {
        [Value(0)]
        public IEnumerable<string> PackageFullNames { get; set; }
    }
}