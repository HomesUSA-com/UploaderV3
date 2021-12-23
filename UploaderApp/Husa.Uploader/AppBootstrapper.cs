using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Husa.Cargador.Support;
using Serilog;

namespace Husa.Cargador
{
    public class AppBootstrapper : BootstrapperBase
    {
        SimpleContainer _container;

        public AppBootstrapper()
        {
            Log.Logger = LoggingSupport.GetLogger("", Guid.Empty);

            Initialize();

            Application.DispatcherUnhandledException += (sender, args) =>
            {
                Log.Fatal(args.Exception, "The uploader CRASHED with an UnhandledException");
                UploaderEngine.CloseDrivers();
                Thread.Sleep(TimeSpan.FromSeconds(2));
            };

            Application.Exit += (sender, args) =>
            {
                try { UploaderEngine.CloseDrivers(); } catch { }
            };

            CleanTempFolder();
        }

        private static void CleanTempFolder()
        {
            try
            {
                var folder = Path.Combine(Path.GetTempPath(), "Husa.Core.Uploader");
                Directory.Delete(folder, true);
            }
            catch { }
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.PerRequest<IShell, ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}