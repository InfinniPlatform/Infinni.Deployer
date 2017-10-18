using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("list", HelpText = "List available versions of application package.")]
    public class ListOptions : ICommandOptions
    {
        [Option('i', "packageId", HelpText = "Application package id.")]
        public string PackageId { get; set; }

        [Option('c', "count", HelpText = "Max number of package versions in list.",Default = 10)]
        public int Count { get; set; }
    }
}