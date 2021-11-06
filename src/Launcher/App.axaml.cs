using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DryIoc;
using Infrastructure;
using Launcher.ViewModels;
using Launcher.Views;
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace Launcher
{
    public class App : PrismApplication
    {
        public override void Initialize()
        {
            // ReSharper disable once UnusedVariable
            var svgType = typeof(Avalonia.Svg.Skia.Svg); // HACK
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        public static Window Window => (Window)(Current as PrismApplication)?.MainWindow!;

        public static T? Resolve<T>()
        {
            try
            {
                return ((Current as PrismApplication)!).Container.Resolve<T>();
            }
            catch (Exception)
            {
                // ignored
            }

            return default;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // TODO: Register services here

            containerRegistry.RegisterInstance(new LoggerFactory());
            containerRegistry.RegisterInstance(Program.Configuration);
            containerRegistry.RegisterInstance(Program.HttpClient);

            containerRegistry.AddInfrastructure();
            
            // navigations
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<WelcomeView, WelcomeViewModel>();

            // create logger factory
            ILoggerFactory loggerFactory = new LoggerFactory();

            // get the container
            var container = containerRegistry.GetContainer();

            // register factory
            container.UseInstance(loggerFactory);

            // get the factory method
            var loggerFactoryMethod = typeof(LoggerFactoryExtensions).GetMethod("CreateLogger", new[] { typeof(ILoggerFactory) });

            container.Register(typeof(ILogger<>), made: Made.Of(req =>
            {
                if (loggerFactoryMethod is not null)
                    return loggerFactoryMethod.MakeGenericMethod(req.Parent.ImplementationType);
                return null;
            }));
        }

        protected override IAvaloniaObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewname = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewname}ViewModel, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Bootstrapper>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }
    }
}