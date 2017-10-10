using Serilog;
using ILogger = NuGet.Common.ILogger;

namespace Infinni.Deployer.Logging
{
    public class NugetLogger : ILogger
    {
        public void LogDebug(string data)
        {
            Log.ForContext<NugetLogger>().Debug("{Data}", data);
        }

        public void LogVerbose(string data)
        {
            Log.ForContext<NugetLogger>().Verbose("{Data}", data);
        }

        public void LogInformation(string data)
        {
            Log.ForContext<NugetLogger>().Verbose("{Data}", data);
        }

        public void LogMinimal(string data)
        {
            Log.ForContext<NugetLogger>().Verbose("{Data}", data);
        }

        public void LogWarning(string data)
        {
            Log.ForContext<NugetLogger>().Warning("{Data}", data);
        }

        public void LogError(string data)
        {
            Log.ForContext<NugetLogger>().Error("{Data}", data);
        }

        public void LogInformationSummary(string data)
        {
            Log.ForContext<NugetLogger>().Verbose("{Data}", data);
        }

        public void LogErrorSummary(string data)
        {
            Log.ForContext<NugetLogger>().Error("{Data}", data);
        }
    }
}