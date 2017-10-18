using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("uninstall", HelpText = "Uninstall application.")]
    public class UninstallOptions : ICommandOptions
    {
        [Option('i', "packageId", HelpText = "Application package id.")]
        public string PackageId { get; set; }

        [Option('v', "version", HelpText = "Application package version.")]
        public string Version { get; set; }
    }
}