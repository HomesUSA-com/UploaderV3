namespace Husa.Uploader.Desktop.Configuration
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using Husa.CompanyServicesManager.Api.Client;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Api.Client;
    using Husa.Extensions.Api.Handlers;
    using Husa.MediaService.Client;
    using Husa.Migration.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Constants;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Data.Repositories;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.ViewModels;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Serilog;
    using Serilog.Events;

    public static class ApplicationBootstrapper
    {
        public static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<ApplicationOptions>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(ApplicationOptions.Section).Bind(settings))
                .ValidateOnStart();

            services.BindIdentitySettings();

            return services;
        }

        public static void ConfigureSerilog(
            this LoggerConfiguration loggerConfiguration,
            ApplicationOptions options,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithProperty(nameof(ApplicationOptions.ApplicationId), options.ApplicationId);

            var featureFlags = configuration.GetSection("Application:FeatureFlags").Get<FeatureFlags>();
            if (featureFlags.EnableDetailedLogs)
            {
                loggerConfiguration
                    .WriteTo.Trace()
                    .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose);
            }

            loggerConfiguration.ConfigureEventLog(
                isDevelopment: featureFlags.EnableDetailedLogs,
                environmentName: environment.EnvironmentName,
                machineName: Environment.MachineName);

            loggerConfiguration.ReadFrom.Configuration(configuration);
        }

        public static LoggerConfiguration ConfigureEventLog(this LoggerConfiguration loggerConfiguration, bool isDevelopment, string environmentName, string machineName)
        {
            var minimumLogLevel = !isDevelopment ? LogEventLevel.Information : LogEventLevel.Debug;
            var loggingTemplate = $"[{Assembly.GetExecutingAssembly().GetName().Name}-{environmentName}][{{Timestamp:HH:mm:ss}} {{Level}}] {{SourceContext}}{{NewLine}}{{Message:lj}}{{NewLine}}{{Exception}}{{NewLine}}";
            loggerConfiguration.WriteTo.EventLog(
                source: ApplicationConstants.ApplicationSource,
                logName: ApplicationConstants.ApplicationLogName,
                manageEventSource: true,
                machineName: machineName,
                outputTemplate: loggingTemplate,
                restrictedToMinimumLevel: minimumLogLevel);

            return loggerConfiguration;
        }

        public static void ConfigureDataAccess(this IServiceCollection services)
        {
            services.AddTransient<IMediaRepository, MediaRepository>();
            services.AddTransient<IListingRequestRepository, ListingRequestRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IVersionManagerService, VersionManagerService>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        public static void ConfigureHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddHttpClient<IMigrationClient, MigrationClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.MigrationService);
            });

            services.AddHttpClient<HusaClient<IMediaServiceClient>>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.Media);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IQuicklisterSaborClient, QuicklisterSaborClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterSabor);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IQuicklisterCtxClient, QuicklisterCtxClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterCtx);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IServiceSubscriptionClient, ServiceSubscriptionClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.CompanyServicesManager);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddTransient<IMediaServiceClient, MediaServiceClient>();
        }

        public static IServiceCollection ConfigureNavigationServices(this IServiceCollection services)
        {
            services.AddTransient<IWebDriver>(provider =>
            {
                var appOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                var driverPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var chromeOptions = GetOptions(appOptions.Uploader, driverPath);
                var driverService = ChromeDriverService.CreateDefaultService(driverPath);
                driverService.HideCommandPromptWindow = true;
                return new ChromeDriver(driverService, options: chromeOptions);
            });

            services.AddSingleton<IUploadFactory, UploaderFactory>();
            services.AddTransient<IUploaderClient, UploaderClient>();
            services.AddTransient<ISaborUploadService, SaborUploadService>();
            services.AddTransient<ICtxUploadService, CtxUploadService>();

            return services;

            static ChromeOptions GetOptions(UploaderSettings uploaderOptions, string driverPath)
            {
                var options = new ChromeOptions
                {
                    BinaryLocation = @$"{driverPath}\ChromeForTesting\chrome.exe",
                };
                options.AddArguments(uploaderOptions.ChromeOptions.Arguments);
                foreach (var preference in uploaderOptions.ChromeOptions.UserProfilePreferences)
                {
                    options.AddUserProfilePreference(preference.Key, preference.Value);
                }

                return options;
            }
        }

        public static void RegisterViews(this IServiceCollection services)
        {
            services.AddSingleton<IChildViewFactory, ChildViewFactory>();
            services.AddSingleton<ShellView>();
            services.AddSingleton<ShellViewModel>();
            services.AddViewFactory<MlsnumInputView, MlsnumInputViewModel>();
            services.AddViewFactory<LatLonInputView, LatLonInputViewModel>();
            services.AddViewFactory<MlsIssueReportView, MlsIssueReportViewModel>();
        }

        public static void AddViewFactory<TForm, TModel>(this IServiceCollection services)
            where TForm : class
            where TModel : class
        {
            services.AddTransient<TForm>();
            services.AddTransient<TModel>();
            services.AddTransient<Func<TForm>>(serviceProvider => () => serviceProvider.GetService<TForm>());
            services.AddSingleton<IAbstractFactory<TForm>, AbstractFactory<TForm>>();
        }

        public static void ConfigureSignalR(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;

                return new HubConnectionBuilder()
                .WithUrl($"{options.SignalRURLServer}/uploaderHub")
                .Build();
            });
        }
    }
}
