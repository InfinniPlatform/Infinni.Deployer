using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("install", HelpText = "Install application.")]
    public class InstallOptions : ICommandOptions
    {
        [Option('i', "packageId", HelpText = "Application package id.")]
        public string PackageId { get; set; }

        [Option('v', "version", HelpText = "Application package version.")]
        public string Version { get; set; }
    }
}