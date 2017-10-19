using System.Threading.Tasks;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Nuget;

namespace Infinni.Deployer.CommandLine.Handlers
{
    public class ListCommandHandler : ICommandHandler<ListOptions>
    {
        private readonly NugetPackageSearcher _nugetPackageSearcher;


        public ListCommandHandler(NugetPackageSearcher nugetPackageSearcher)
        {
            _nugetPackageSearcher = nugetPackageSearcher;
        }

        public async Task Handle(ListOptions options)
        {
            await _nugetPackageSearcher.Search(options.PackageId, options.Count);
        }
    }
}