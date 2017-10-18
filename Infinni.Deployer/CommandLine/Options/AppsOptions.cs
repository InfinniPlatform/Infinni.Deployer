using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("apps", HelpText = "List installed applications.")]
    public class AppsOptions : ICommandOptions
    {
    }
}