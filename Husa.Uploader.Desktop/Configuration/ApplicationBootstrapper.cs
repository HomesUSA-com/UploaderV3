namespace Husa.Uploader.Desktop.Configuration
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Abstractions;
    using System.IO.Compression;
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

            return services;

            static ChromeOptions GetOptions(UploaderSettings uploaderOptions, string driverPath, IConfiguration configuration)
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

                // Handle proxy configuration
                var isEnabledProxy = configuration.GetValue<bool>("Application:Uploader:ChromeOptions:Proxy:Enabled");
                if (isEnabledProxy)
                {
                    var proxyOptions = uploaderOptions.ChromeOptions.Proxy;
                    options.AddArgument("--disable-blink-features=AutomationControlled");
                    options.AddArgument("--disable-extensions");
                    options.AddArgument("--disable-popup-blocking");
                    options.AddArgument("--enable-automation");

                    options.AddArgument("--auth-server-whitelist=*");
                    options.AddArgument("--auth-negotiate-delegate-whitelist=*");

                    options.AddUserProfilePreference("security.insecure_field_warning.contextual.enabled", false);
                    options.AddUserProfilePreference("security.insecure_password_field_warning.enabled", false);

                    var proxyUri = new Uri(proxyOptions.Url);
                    ConfigureProxyAuthentication(options, proxyUri.Host, proxyUri.Port, proxyOptions.Username, proxyOptions.Password);
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

        private static void ConfigureProxyAuthentication(ChromeOptions options, string proxyHost, int proxyPort, string username, string password)
        {
            var extensionScript = $@"
                const config = {{
                    mode: 'fixed_servers',
                    rules: {{
                        singleProxy: {{
                            scheme: 'http',
                            host: '{proxyHost}',
                            port: {proxyPort}
                        }},
                        bypassList: ['localhost', '127.0.0.1']
                    }}
                }};

                chrome.proxy.settings.set({{value: config, scope: 'regular'}}, () => {{
                    console.log('Proxy working...');
                }});

                chrome.webRequest.onAuthRequired.addListener(
                    function(details) {{
                        return {{
                            authCredentials: {{
                                username: '{username}',
                                password: '{password}'
                            }}
                        }};
                    }},
                    {{ urls: ['<all_urls>'] }},
                    ['blocking']
                );

                chrome.webRequest.onBeforeSendHeaders.addListener(
                    function(details) {{
                        const authString = btoa('{username}:{password}');
                        let headers = details.requestHeaders || [];
                        const hasAuth = headers.some(h => h.name.toLowerCase() === 'proxy-authorization');

                        if (!hasAuth) {{
                            headers.push({{
                                name: 'Proxy-Authorization',
                                value: 'Basic ' + authString
                            }});
                        }}

                        return {{ requestHeaders: headers }};
                    }},
                    {{ urls: ['<all_urls>'] }},
                    ['blocking', 'requestHeaders']
                );

                setInterval(() => {{
                    chrome.proxy.settings.set({{value: config, scope: 'regular'}});
                }}, 30000);
            ";

            var popupBlockerScript = @"
                const originalAlert = window.alert;
                window.alert = function(message) {
                    if (message.includes('proxy') || message.includes('authentication')) {
                        return;
                    }
                    originalAlert(message);
                };

                const observer = new MutationObserver(mutations => {
                    mutations.forEach(mutation => {
                        mutation.addedNodes.forEach(node => {
                            if (node.nodeType === 1) {
                                const element = node;
                                if (element.tagName === 'DIALOG' ||
                                    element.tagName === 'INPUT' && element.type === 'password' ||
                                    element.innerText.includes('Sign in') ||
                                    element.innerText.includes('Authentication')) {
                                    element.style.display = 'none';
                                }
                            }
                        });
                    });
                });

                observer.observe(document.body, { childList: true, subtree: true });
            ";

            var manifest = @"
            {
                ""manifest_version"": 3,
                ""name"": ""Proxy Auth Enforcer"",
                ""version"": ""1.3"",
                ""permissions"": [
                    ""proxy"",
                    ""webRequest"",
                    ""webRequestAuthProvider"",
                    ""scripting"",
                    ""tabs""
                ],
                ""host_permissions"": [""<all_urls>""],
                ""background"": {
                    ""service_worker"": ""background.js""
                },
                ""content_scripts"": [
                    {
                        ""matches"": [""<all_urls>""],
                        ""js"": [""popupBlocker.js""],
                        ""run_at"": ""document_start""
                    }
                ]
            }";

            using (var zipStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    // Adding manifest.json
                    var manifestEntry = archive.CreateEntry("manifest.json");
                    using (var writer = new StreamWriter(manifestEntry.Open()))
                    {
                        writer.Write(manifest);
                    }

                    // Adding background.js
                    var bgEntry = archive.CreateEntry("background.js");
                    using (var writer = new StreamWriter(bgEntry.Open()))
                    {
                        writer.Write(extensionScript);
                    }

                    // Adding popupBlocker.js
                    var popupEntry = archive.CreateEntry("popupBlocker.js");
                    using (var writer = new StreamWriter(popupEntry.Open()))
                    {
                        writer.Write(popupBlockerScript);
                    }
                }

                // 5. Creating a temporal file ZIP
                zipStream.Position = 0;
                string tempZip = Path.GetRandomFileName() + ".zip";
                File.WriteAllBytes(tempZip, zipStream.ToArray());

                // 6. Adding extension to chrome
                options.AddExtension(tempZip);
            }

            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--ignore-certificate-errors-spki-list");
            options.AddArgument("--disable-popup-blocking");

            options.AddArgument($"--proxy-server=http://{proxyHost}:{proxyPort}");

            options.AddUserProfilePreference("automatic_https_upgrade", 0);
            options.AddUserProfilePreference("security.insecure_field_warning.contextual.enabled", false);

            options.AddUserProfilePreference("proxy.config", new
            {
                mode = "fixed_servers",
                rules = new
                {
                    proxyForHttp = $"{proxyHost}:{proxyPort}",
                    proxyForHttps = $"{proxyHost}:{proxyPort}",
                    bypassList = "localhost,127.0.0.1",
                },
                username = username,
                password = password,
            });
        }
    }
}
