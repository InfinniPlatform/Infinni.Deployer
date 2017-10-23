using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("start", HelpText = "Starts application.")]
    public class StartOptions : ICommandOptions
    {
        [Option('i', "packageId", HelpText = "Application package id.", Required = true)]
        public string PackageId { get; set; }

        [Option('v', "version", HelpText = "Application package version.", Required = true)]
        public string Version { get; set; }

        [Option('f', "file", HelpText = "Executable dll to start application.")]
        public string ExecutableFile { get; set; }
    }
}