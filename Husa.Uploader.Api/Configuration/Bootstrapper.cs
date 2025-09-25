namespace Husa.Uploader.Api.Configuration
{
    using System;
    using System.Runtime.InteropServices;
    using AutoMapper;
    using Husa.CompanyServicesManager.Api.Client;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Api;
    using Husa.Extensions.Api.Client;
    using Husa.Extensions.Api.Handlers;
    using Husa.Extensions.Api.Mvc;
    using Husa.Extensions.Authorization.Extensions;
    using Husa.Extensions.ServiceBus.Extensions;
    using Husa.MediaService.Client;
    using Husa.Migration.Api.Client;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.Amarillo.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Api.Filters;
    using Husa.Uploader.Api.ServiceBus.Handlers;
    using Husa.Uploader.Api.ServiceBus.Subscribers;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Crosscutting.Services;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Data.Interfaces.LotListing;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    public static class Bootstrapper
    {
        public static IServiceCollection BindOptions(this IServiceCollection services)
        {
            services
                .BindApplicationSettings()
                .BindUploaderSettings();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Core upload services
            services.AddScoped<IUploaderClient, UploaderClient>();
            services.AddScoped<IUploaderServiceFactory, UploaderServiceFactory>();
            services.AddScoped<ISleepService, SleepService>();

            // Market-specific upload services
            services.AddScoped<AborUploadService>();
            services.AddScoped<SaborUploadService>();
            services.AddScoped<HarUploadService>();
            services.AddScoped<DfwUploadService>();
            services.AddScoped<CtxUploadService>();
            services.AddScoped<AmarilloUploadService>();

            return services;
        }

        public static IServiceCollection AddDataRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IListingRequestRepository, ListingRequestRepository>();
            services.AddScoped<ILotListingRequestRepository, LotListingRequestRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();

            return services;
        }

        public static IServiceCollection ConfigureWebDriver(this IServiceCollection services) => services
            .AddScoped<IWebDriver>(sp =>
            {
                var appOptions = sp.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                var options = GetOptions(appOptions);

                var service = ChromeDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = true;
                service.HideCommandPromptWindow = true;
                service.EnableVerboseLogging = false;
                var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(90));

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);

                return driver;
            });

        public static IServiceCollection SwaggerConfiguration(this IServiceCollection services) => services
            .AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Husa.Uploader.Api", Version = "v1" }));

        public static IServiceCollection ControllerConfiguration(this IServiceCollection services)
        {
            services
                .AddControllersWithViews(options => options.AddControllerPrefixConventions("api"))
                .AddJsonOptions(options => options.JsonSerializerOptions.SetConfiguration());

            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter()));

            return services;
        }

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg => { });

            services.AddSingleton(config.CreateMapper());
            return services;
        }

        public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
        {
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

            return services;
        }

        public static IServiceCollection AddMvcOptions(this IServiceCollection services)
        {
            services.AddScoped<ExceptionHandlingFilterAttribute>();
            services.AddScoped<BrowserLogoutActionFilter>();

            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ExceptionHandlingFilterAttribute>();
                    options.Filters.Add<BrowserLogoutActionFilter>();
                });
            return services;
        }

        public static IServiceCollection ConfigureWorker(this IServiceCollection services)
        {
            services.ConfigureServiceBusOptions();
            services.AddTransient<IResidentialRequestMessagesHandler, ResidentialRequestMessagesHandler>();
            services.AddSingleton<IResidentialRequestSubscriber, ResidentialRequestSubscriber>();

            return services;
        }

        public static IServiceCollection RegisterDelegatedServices(this IServiceCollection services) => services
            .UseAuthorizationContext()
            .UseTraceIdProvider();
        private static ChromeOptions GetOptions(ApplicationOptions uploaderOptions)
        {
            var options = new ChromeOptions();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                options.AddArguments("--no-sandbox", "--disable-dev-shm-usage");
            }

            options.AddExcludedArgument("enable-logging");   // stops a lot of Chrome spam

            if (uploaderOptions.Uploader?.ChromeOptions?.Arguments is { } extraArgs)
            {
                Console.WriteLine($"Loading {extraArgs.Count} arguments from configuration:");
                foreach (var arg in extraArgs)
                {
                    Console.WriteLine($"  - {arg}");
                }

                options.AddArguments(extraArgs);
            }
            else
            {
                Console.WriteLine("No ChromeOptions Arguments found in configuration");
            }

            if (uploaderOptions.Uploader?.ChromeOptions?.UserProfilePreferences is { } prefs)
            {
                foreach (var kv in prefs)
                {
                    options.AddUserProfilePreference(kv.Key, kv.Value);
                }
            }

            return options;
        }

        private static IServiceCollection BindUploaderSettings(this IServiceCollection services)
        {
            services.AddOptions<UploaderUserSettings>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(UploaderUserSettings.Section).Bind(settings));
            return services;
        }

        private static IServiceCollection BindApplicationSettings(this IServiceCollection services)
        {
            services
                .AddOptions<ApplicationOptions>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(ApplicationOptions.Section).Bind(settings));

            return services;
        }

        private static IServiceCollection ConfigureServiceBusOptions(this IServiceCollection services)
        {
            services
                .AddOptions<ServiceBusSettings>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(ServiceBusSettings.Section).Bind(settings))
                .ValidateDataAnnotations();

            return services;
        }
    }
}
