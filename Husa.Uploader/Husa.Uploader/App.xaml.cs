namespace Husa.Uploader;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Husa.Uploader.Configuration;
using Husa.Uploader.Crosscutting.Options;
using Husa.Uploader.Data.Repositories;
using Husa.Uploader.Deployment;
using Husa.Uploader.Factories;
using Husa.Uploader.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

public partial class App : Application
{
    public App()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Trace().CreateBootstrapLogger();

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((host, configuration) =>
            {
                configuration
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                if (host.HostingEnvironment.IsDevelopment())
                {
                    configuration.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                }

                configuration.AddEnvironmentVariables();

                if (host.HostingEnvironment.IsDevelopment())
                {
                    configuration.AddUserSecrets<Startup>();
                }
                ////else
                ////{
                ////    configuration.AddKeyVault(host);
                ////}
            })
            .UseSerilog((context, loggerConfig) =>
            {
                var applicationOptions = context.Configuration.GetSection("Application").Get<ApplicationOptions>();
                loggerConfig.ConfigureSerilog(
                    options: applicationOptions,
                    configuration: context.Configuration,
                    environment: context.HostingEnvironment);
            })
            .ConfigureServices((host, services) =>
            {
                services.RegisterViews();
                services.ConfigureApplicationOptions();
                services.ConfigureDataAccess();
                services.ConfigureServices();
                services.ConfigureWebDriver();
                services.ConfigureSignalR();
                VersionManager.ConfigureVersionInfo(isDevelopment: host.HostingEnvironment.IsDevelopment());
            })
            .Build();
    }

    public static IHost AppHost { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        CleanTempFolder();

        var startupForm = AppHost.Services.GetRequiredService<ShellView>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        await GracefulShutdown();
        base.OnExit(e);
    }

    protected async void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "The uploader CRASHED with an UnhandledException");
        await GracefulShutdown();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        e.Handled = true;
    }

    private static void CleanTempFolder()
    {
        try
        {
            var folder = Path.Combine(Path.GetTempPath(), MediaRepository.MediaFolderName);
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }
        }
        catch
        {
            // Ignoring error because the outcome of the delete is not important
        }
    }

    private static async Task GracefulShutdown()
    {
        var uploadFactory = AppHost.Services.GetRequiredService<IUploadFactory>();
        uploadFactory.CloseDriver();

        var hubConnection = AppHost.Services.GetRequiredService<HubConnection>();
        await hubConnection.StopAsync();
    }
}
