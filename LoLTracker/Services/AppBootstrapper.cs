using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Avalonia.ReactiveUI;
using LoLTracker.Models;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Autofac;
using Splat.Serilog;

namespace LoLTracker.Services
{
    internal static class AppBootstrapper
    {
        public static void RegisterServices()
        {
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            var builder = new ContainerBuilder();

            RegisterStatsServices(builder);

            // Use Autofac for ReactiveUI dependency resolution.
            // After we call the method below, Locator.Current and
            // Locator.CurrentMutable start using Autofac locator.
            AutofacDependencyResolver resolver = new AutofacDependencyResolver(builder);
            Locator.SetLocator(resolver);

            // These .InitializeX() methods will add ReactiveUI platform
            // registrations to your container. They MUST be present if
            // you *override* the default Locator.
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            RegisterLogging();
            RegisterConfiguration();

            // Registering Views
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            resolver.SetLifetimeScope(builder.Build());
        }

        public static void RegisterStatsServices(ContainerBuilder builder)
        {
            builder.RegisterType<RiotApi>().AsSelf().SingleInstance();
            builder.RegisterType<CacheService>().AsSelf().SingleInstance();
            builder.RegisterType<StatisticsService>().AsSelf().SingleInstance();
            builder.RegisterType<LeagueContext>().AsSelf().SingleInstance();
            builder.RegisterType<CacheService>().AsSelf().SingleInstance();
            builder.RegisterType<IconsService>().AsSelf().SingleInstance();
        }

        private static void RegisterConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonStream(new MemoryStream(Properties.Resources.AppConfig))
                .Build();
            Locator.CurrentMutable.RegisterConstant<IConfiguration>(config);
        }

        private static void RegisterLogging()
        {
            var config = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                config.WriteTo.File("logs/tracker.log", Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day);
            var logger = config.CreateLogger();
            Locator.CurrentMutable.UseSerilogFullLogger(logger);
        }
    }
}