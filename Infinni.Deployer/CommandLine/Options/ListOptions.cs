using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("list", HelpText = "List available versions of application package.")]
    public class ListOptions : ICommandOptions
    {
        [Value(0, HelpText = "Full package name, e.g. AwesomePackage.1.2.3.4@InstanceName.")]
        public string PackageId { get; set; }

        [Option('i', "installed", HelpText = "List installed packages.")]
        public bool ShowInstalled { get; set; }

        [Option('a', "available", HelpText = "List available packages.")]
        public bool ShowAvailable { get; set; }

        [Option('p', "prerelease", HelpText = "Include prerelease packages to search results,", Default = false)]
        public bool IncludePrerelease { get; set; }

        [Option('c', "take", HelpText = "Max number of package versions in list.", Default = 10)]
        public int Take { get; set; }
    }
}