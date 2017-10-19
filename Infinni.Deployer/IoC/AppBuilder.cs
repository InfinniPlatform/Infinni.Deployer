using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Builder;
using Infinni.Deployer.CommandLine.Handlers;
using Infinni.Deployer.CommandLine.Options;
using Infinni.Deployer.Helpers;
using Infinni.Deployer.Logging;
using Infinni.Deployer.Settings;
using Newtonsoft.Json;
using Serilog;
using ILogger = NuGet.Common.ILogger;

namespace Infinni.Deployer.IoC
{
    public static class AppBuilder
    {
        public static IContainer Resolver { get; private set; }

        public static void InitializeAutofac()
        {
            var assembly = Assembly.GetAssembly(typeof(AppBuilder));

            var builder = new ContainerBuilder();

            builder.RegisterSettings();
            builder.RegisterCommandHandlers(assembly);
            builder.RegisterCommandOptions(assembly);

            builder.RegisterType<NugetLogger>()
                   .As<ILogger>()
                   .SingleInstance();

            builder.RegisterType<NugetSettings>()
                   .AsSelf()
                   .SingleInstance();

            builder.Register(SystemServiceManagerFactory)
                   .As<ISystemServiceManager>()
                   .SingleInstance();

            Resolver = builder.Build();
        }

        private static ISystemServiceManager SystemServiceManagerFactory(IComponentContext context)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new ServiceControlWrapper();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new SystemCtlWrapper();
            }
            
            throw new NotImplementedException($"Infinni.Deployer is not implemented for {RuntimeInformation.OSDescription}.");
        }

        private static void RegisterSettings(this ContainerBuilder builder)
        {
            builder.Register(r =>
                   {
                       var appSettingsFile = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, AppSettings.FileName));
                       var settings = JsonConvert.DeserializeObject<AppSettings>(appSettingsFile);
                       return settings;
                   })
                   .SingleInstance();
        }

        private static void RegisterCommandHandlers(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .SingleInstance();
        }

        private static void RegisterCommandOptions(this ContainerBuilder builder, Assembly assembly)
        {
            var options = assembly.GetTypes().Where(t => t.IsAssignableTo<ICommandOptions>());

            foreach (var option in options)
            {
                builder.RegisterType(option)
                       .As<ICommandOptions>()
                       .SingleInstance();
            }
        }

        public static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine("logs", "events-{Date}.log"))
                .CreateLogger();
        }
    }
}