using System.Threading.Tasks;
using Infinni.Deployer.CommandOptions;

namespace Infinni.Deployer.CommandHandlers
{
    public interface ICommandHandler
    {
    }


    public interface ICommandHandler<in TOptions> : ICommandHandler where TOptions : ICommandOptions
    {
        Task Handle(TOptions options);
    }
}