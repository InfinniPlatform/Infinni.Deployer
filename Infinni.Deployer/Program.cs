using System;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using Infinni.Deployer.CommandHandlers;
using Infinni.Deployer.CommandOptions;
using Infinni.Deployer.IoC;
using Serilog;

namespace Infinni.Deployer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppBuilder.InitializeLogger();
            AppBuilder.InitializeAutofac();

            ParseCommandLine(args)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Log.Error(task.Exception, string.Empty);
                    }
                })
                .Wait();
        }

        private static Task ParseCommandLine(string[] args)
        {
            var optionTypes = new[] {typeof(InstallOptions), typeof(ListOptions), typeof(AppsOptions), typeof(UninstallOptions), typeof(StartOptions)};

            var result = Parser.Default.ParseArguments(args, optionTypes);

            var parsed = result as Parsed<object>;

            if (parsed == null)
            {
                return Task.FromResult(-1);
            }

            if (parsed.Value is InstallOptions installOptions)
            {
                return AppBuilder.Resolver.Resolve<ICommandHandler<InstallOptions>>().Handle(installOptions);
            }

            if (parsed.Value is ListOptions listOptions)
            {
                return AppBuilder.Resolver.Resolve<ICommandHandler<ListOptions>>().Handle(listOptions);
            }

            if (parsed.Value is AppsOptions appsOptions)
            {
                return AppBuilder.Resolver.Resolve<ICommandHandler<AppsOptions>>().Handle(appsOptions);
            }

            if (parsed.Value is UninstallOptions uninstallOptions)
            {
                return AppBuilder.Resolver.Resolve<ICommandHandler<UninstallOptions>>().Handle(uninstallOptions);
            }

            if (parsed.Value is StartOptions startOptions)
            {
                return AppBuilder.Resolver.Resolve<ICommandHandler<StartOptions>>().Handle(startOptions);
            }

            throw new InvalidOperationException();
        }
    }
}