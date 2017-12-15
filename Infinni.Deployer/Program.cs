using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using CommandLine;

using Infinni.Deployer.CommandLine.Handlers;
using Infinni.Deployer.CommandLine.Options;
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
                                      Log.Error("{Exception}", task.Exception);
                                  }
                              })
                .Wait();

            Log.CloseAndFlush();
        }

        private static Task ParseCommandLine(IEnumerable<string> args)
        {
            var optionTypes = typeof(Program).Assembly.GetTypes()
                                             .Where(t => t.IsClass && t.IsAssignableTo<ICommandOptions>())
                                             .ToArray();

            var result = Parser.Default.ParseArguments(args, optionTypes);


            if (!(result is Parsed<object> parsed))
            {
                return Task.FromResult(-1);
            }

            switch (parsed.Value)
            {
                case InstallOptions installOptions:
                    return AppBuilder.Resolver.Resolve<ICommandHandler<InstallOptions>>().Handle(installOptions);
                case ListOptions listOptions:
                    return AppBuilder.Resolver.Resolve<ICommandHandler<ListOptions>>().Handle(listOptions);
                case UninstallOptions uninstallOptions:
                    return AppBuilder.Resolver.Resolve<ICommandHandler<UninstallOptions>>().Handle(uninstallOptions);
                case StartOptions startOptions:
                    return AppBuilder.Resolver.Resolve<ICommandHandler<StartOptions>>().Handle(startOptions);
                case StopOptions stopOptions:
                    return AppBuilder.Resolver.Resolve<ICommandHandler<StopOptions>>().Handle(stopOptions);
            }

            throw new InvalidOperationException();
        }
    }
}