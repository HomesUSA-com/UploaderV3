namespace Husa.Uploader.Desktop.Configuration
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using Husa.CompanyServicesManager.Api.Client;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Api.Client;
    using Husa.Extensions.Api.Handlers;
    using Husa.MediaService.Client;
    using Husa.Migration.Api.Client;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.Amarillo.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload;
    using Husa.Uploader.Core.Interfaces.ShowingTime;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Core.Services.BulkUpload;
    using Husa.Uploader.Crosscutting.Constants;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Data.Interfaces.LotListing;
    using Husa.Uploader.Data.Repositories;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.ViewModels;
    using Husa.Uploader.Desktop.Views;
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
            services.AddTransient<ILotListingRequestRepository, LotListingRequestRepository>();
            services.AddTransient<IListingRepository, ListingRepository>();
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

            services.AddHttpClient<IQuicklisterAborClient, QuicklisterAborClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterAbor);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IQuicklisterHarClient, QuicklisterHarClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterHar);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IQuicklisterDfwClient, QuicklisterDfwClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterDfw);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IQuicklisterAmarilloClient, QuicklisterAmarilloClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                client.BaseAddress = new Uri(options.Services.QuicklisterAmarillo);
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
                var configuration = provider.GetRequiredService<IConfiguration>();
                var appOptions = provider.GetRequiredService<IOptionsMonitor<ApplicationOptions>>().CurrentValue;
                var driverPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var chromeOptions = GetOptions(appOptions.Uploader, driverPath, configuration);
                var driverService = ChromeDriverService.CreateDefaultService(driverPath);
                driverService.HideCommandPromptWindow = true;
                return new ChromeDriver(driverService, options: chromeOptions);
            });

            services.AddSingleton<IUploadFactory, UploaderFactory>();
            services.AddTransient<IUploaderClient, UploaderClient>();
            services.AddTransient<ISaborUploadService, SaborUploadService>();
            services.AddTransient<ICtxUploadService, CtxUploadService>();
            services.AddTransient<IAborUploadService, AborUploadService>();
            services.AddTransient<IAmarilloUploadService, AmarilloUploadService>();
            services.AddTransient<IHarUploadService, HarUploadService>();
            services.AddTransient<IDfwUploadService, DfwUploadService>();
            services.AddSingleton<IBulkUploadFactory, BulkUploadFactory>();
            services.AddTransient<ISaborBulkUploadService, SaborBulkUploadService>();
            services.AddTransient<IDfwBulkUploadService, DfwBulkUploadService>();
            services.AddTransient<IHarBulkUploadService, HarBulkUploadService>();
            services.AddTransient<ICtxBulkUploadService, CtxBulkUploadService>();
            services.AddTransient<IAborBulkUploadService, AborBulkUploadService>();
            services.AddTransient<IShowingTimeUploadService, ShowingTimeUploadService>();
            services.AddTransient<ITaxIdBulkUploadFactory, TaxIdBulkUploadFactory>();

            return services;

            static ChromeOptions GetOptions(UploaderSettings uploaderOptions, string driverPath, IConfiguration configuration)
            {
                var options = new ChromeOptions
                {
                    BinaryLocation = @$"{driverPath}\ChromeForTesting\chrome.exe",
                };

                foreach (var preference in uploaderOptions.ChromeOptions.UserProfilePreferences)
                {
                    options.AddUserProfilePreference(preference.Key, preference.Value);
                }

                // Handle proxy configuration
                var isEnabledProxy = configuration.GetValue<bool>("Application:Uploader:ChromeOptions:Proxy:Enabled");
                if (!isEnabledProxy)
                {
                    options.AddArguments(uploaderOptions.ChromeOptions.Arguments);
                }
                else
                {
                    var proxyOptions = uploaderOptions.ChromeOptions.Proxy;
                    options.AddArgument("--enable-extensions");
                    options.AddArgument("--disable-popup-blocking");
                    options.AddArgument("--auth-server-whitelist=*");
                    options.AddArgument("--auth-negotiate-delegate-whitelist=*");

                    options.AddUserProfilePreference("security.insecure_field_warning.contextual.enabled", false);
                    options.AddUserProfilePreference("security.insecure_password_field_warning.enabled", false);
                    options.AddUserProfilePreference("security.ssl.enable_ocsp_stapling", false);
                    options.AddUserProfilePreference("security.ssl.enable_false_start", false);
                    options.AddUserProfilePreference("security.ssl.errorReporting.enabled", false);

                    var proxyUri = new Uri(proxyOptions.Url);
                    var extensionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "husa_proxy_extension.crx");
                    options.AddExtension(extensionPath);
                    options.AddArgument("--disable-blink-features=AutomationControlled");
                    options.AddArgument("--ignore-certificate-errors");
                    options.AddArguments($"--proxy-server={proxyUri.Host}:{proxyUri.Port}", "--ignore-certificate-errors", "--allow-running-insecure-content", "--no-sandbox", "--disable-dev-shm-usage", "--disable-gpu", "--disable-popup-blocking", "--disable-web-security", "--allow-insecure-localhost", "--ignore-certificate-errors-spki-list", "--disable-save-password-bubble", "--disable-autofill-assistant", "--disable-infobars", "--disable-notifications", "--disable-component-update", "start-maximized", "--no-default-browser-check");
                    options.AddUserProfilePreference("credentials_enable_service", false);
                    options.AddUserProfilePreference("profile.password_manager_enabled", false);
                    options.AddUserProfilePreference("autofill.enabled", false);
                    options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
                    options.AddUserProfilePreference("profile.managed_default_content_settings.popups", 2);
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
            services.AddViewFactory<BulkUploadView, BulkUploadViewModel>();
            services.AddViewFactory<TaxIdBulkUploadView, TaxIdBulkUploadViewModel>();
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

        public static IHostBuilder SetCultureInfo(this IHostBuilder hostBuilder)
        {
            var customCulture = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = customCulture;
            CultureInfo.DefaultThreadCurrentUICulture = customCulture;
            Thread.CurrentThread.CurrentCulture = customCulture;
            Thread.CurrentThread.CurrentUICulture = customCulture;

            return hostBuilder;
        }

        public static void ConfigureSignalR(this IServiceCollection services)
        {
            services.AddTransient<ISignalRConnectionService, SignalRConnectionService>();
        }
    }
}
