namespace Husa.Uploader.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Exceptions;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class VersionManagerService : IClickOnceUpdateService
    {
        private const string HttpClientKey = nameof(VersionManagerService) + "_httpclient";
        private const string DevelopmentModeVersion = "Development";
        private const string VersionNotSet = "Unknown";

        private static readonly XNamespace NamespaceFirstVersion = "urn:schemas-microsoft-com:asm.v1";
        private static string appBuildDate;
        private static string applicationBuildVersion;

        private readonly ApplicationOptions options;
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        private Version currentVersion;
        private string applicationName;
        private string applicationPath;

        public VersionManagerService(
            IOptions<ApplicationOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<VersionManagerService> logger)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.Initialize();
        }

        public static Version CurrentClickOnceVersion
        {
            get
            {
                _ = Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_CurrentVersion"), out Version version);
                return version;
            }
        }

        public static string ApplicationBuildVersion
        {
            get => applicationBuildVersion;
            private set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    applicationBuildVersion = value;
                    return;
                }

                applicationBuildVersion = VersionNotSet;
            }
        }

        public static string ApplicationBuildDate
        {
            get
            {
                if (appBuildDate != null)
                {
                    return appBuildDate;
                }

                var timestamp = RetrieveLinkerTimestamp();
                appBuildDate = $"Version 3: {(timestamp.HasValue ? timestamp.Value.ToString("yyyy.MM.dd-HH.mm") : VersionNotSet)}";

                return appBuildDate;
            }
        }

        public async Task<Version> CurrentVersionAsync()
        {
            if (string.IsNullOrEmpty(this.applicationName))
            {
                throw new ClickOnceDeploymentException("Application name is empty!");
            }

            if (this.currentVersion is not null)
            {
                return this.currentVersion;
            }

            var path = Path.Combine(this.applicationPath!, $"{this.applicationName}.exe.manifest");
            if (!File.Exists(path))
            {
                throw new ClickOnceDeploymentException($"Can't find manifest file at path {path}");
            }

            this.logger.LogDebug("Looking for local manifest: {path}", path);

            string fileContent = await File.ReadAllTextAsync(path);

            var xmlDoc = XDocument.Parse(fileContent, LoadOptions.None);
            var xmlElement = xmlDoc.Descendants(NamespaceFirstVersion + "assemblyIdentity").FirstOrDefault();
            if (xmlElement == null)
            {
                throw new ClickOnceDeploymentException($"Invalid manifest document for {path}");
            }

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceDeploymentException("Local version info is empty!");
            }

            this.currentVersion = new Version(version);
            ApplicationBuildVersion = version;
            return this.currentVersion;
        }

        public async Task<Version> ServerVersionAsync()
        {
            using var client = HttpClientFactory();
            this.logger.LogDebug("Looking for remote manifest: {publishUrl} {applicationName}.application", this.options.PublishingPath ?? string.Empty, this.applicationName);
            await using Stream stream = await client.GetStreamAsync($"{this.applicationName}.application");
            var serverVersion = await this.ReadServerManifestAsync(stream);
            return serverVersion is not null ?
                serverVersion :
                throw new ClickOnceDeploymentException("Remote version info is empty!");

            HttpClient HttpClientFactory()
            {
                this.logger.LogDebug("HttpClientFactory > returning HttpClient for url: {publishUrl}", this.options.PublishingPath);
                var uri = new Uri(this.options.PublishingPath);
                var client = this.httpClientFactory.CreateClient(name: HttpClientKey);
                if (uri is not null)
                {
                    client.BaseAddress = uri;
                }

                return client;
            }
        }

        public async Task<bool> UpdateAvailableAsync()
        {
            if (!this.options.FeatureFlags.IsVersionCheckEnabled)
            {
                this.logger.LogDebug("Running the application in development mode, skipping update validation");
                return false;
            }

            this.logger.LogInformation("Checking for application updates");
            var currentVer = await this.CurrentVersionAsync();
            var serverVer = await this.ServerVersionAsync();
            return currentVer < serverVer;
        }

        private static DateTime? RetrieveLinkerTimestamp()
        {
            var assembly = Assembly.GetCallingAssembly();
            var fileInfo = new FileInfo(assembly.Location);
            return fileInfo.Exists ?
                fileInfo.CreationTime.ToLocalTime() :
                null;
        }

        private void Initialize()
        {
            this.applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty;
            this.applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            if (string.IsNullOrEmpty(this.applicationName))
            {
                throw new ClickOnceDeploymentException("Can't find entry assembly name!");
            }

            ApplicationBuildVersion = !this.options.FeatureFlags.IsVersionCheckEnabled ?
                DevelopmentModeVersion :
                CurrentClickOnceVersion?.ToString();
        }

        private async Task<Version> ReadServerManifestAsync(Stream stream)
        {
            var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            var xmlElement = xmlDoc
                .Descendants(NamespaceFirstVersion + "assemblyIdentity")
                .FirstOrDefault() ?? throw new ClickOnceDeploymentException($"Invalid manifest document for {this.applicationName}.application");

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceDeploymentException($"Version info is empty!");
            }

            return new(version);
        }
    }
}
