using Husa.Uploader.Configuration;
using Husa.Uploader.Crosscutting.Options;
using Husa.Uploader.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;

namespace Husa.Uploader;

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
                services.ConfigureCosmosDb();
                services.ConfigureDataAccess();
                services.ConfigureServices();
                services.ConfigureWebDriver();
            })
            .Build();
    }

    public static IHost AppHost { get; private set; }

    protected override async void OnStartup(StartupEventArgs args)
    {
        await AppHost.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<ShellView>();
        startupForm.Show();

        base.OnStartup(args);
    }

    protected override async void OnExit(ExitEventArgs args)
    {
        await AppHost.StopAsync();
        base.OnExit(args);
    }
}
