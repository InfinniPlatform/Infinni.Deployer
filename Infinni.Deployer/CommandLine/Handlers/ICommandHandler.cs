using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public interface ICommandHandler
    {
    }


    public interface ICommandHandler<in TOptions> : ICommandHandler where TOptions : ICommandOptions
    {
        Task Handle(TOptions options);
    }
}