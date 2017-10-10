using CommandLine;

namespace Infinni.Deployer.CommandOptions
{
    [Verb("apps", HelpText = "List installed applications.")]
    public class AppsOptions : ICommandOptions
    {
    }
}