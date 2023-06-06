namespace Husa.Uploader.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ClickOnceUpdateService : IClickOnceUpdateService
    {
        public const string SectionKey = "ClickOnce";
        public const string HttpClientKey = nameof(ClickOnceUpdateService) + "_httpclient";

        private static readonly EventId EventId = new(id: 0x1A4, name: "ClickOnce");

        private readonly ApplicationOptions options;
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        private Version currentVersion;

        private string applicationName;

        private string applicationPath;

        public ClickOnceUpdateService(
            IOptions<ApplicationOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<ClickOnceUpdateService> logger)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.Initialize();
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

            this.logger.LogDebug(EventId, "Looking for local manifest: {path}", path);

            string fileContent = await File.ReadAllTextAsync(path).ConfigureAwait(false);

            var xmlDoc = XDocument.Parse(fileContent, LoadOptions.None);
            XNamespace nsSys = "urn:schemas-microsoft-com:asm.v1";
            var xmlElement = xmlDoc.Descendants(nsSys + "assemblyIdentity").FirstOrDefault();
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
            return this.currentVersion;
        }

        public async Task<Version> ServerVersionAsync()
        {
            using var client = this.HttpClientFactory(new Uri(this.options.PublishingPath));
            this.logger.LogDebug(EventId, "Looking for remote manifest: {publishUrl} {applicationName}.application", this.options.PublishingPath ?? string.Empty, this.applicationName);
            await using Stream stream = await client.GetStreamAsync($"{this.applicationName}.application");
            var serverVersion = await this.ReadServerManifestAsync(stream);
            return serverVersion is not null ?
                serverVersion :
                throw new ClickOnceDeploymentException("Remote version info is empty!");
        }

        public async Task<bool> UpdateAvailableAsync()
        {
            var currentVer = await this.CurrentVersionAsync();
            var serverVer = await this.ServerVersionAsync();

            return currentVer < serverVer;
        }

        private void Initialize()
        {
            this.applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty;
            this.applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            if (string.IsNullOrEmpty(this.applicationName))
            {
                throw new ClickOnceDeploymentException("Can't find entry assembly name!");
            }
        }

        private async Task<Version> ReadServerManifestAsync(Stream stream)
        {
            var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None).ConfigureAwait(false);
            XNamespace namespaceVersion1 = "urn:schemas-microsoft-com:asm.v1";

            var xmlElement = xmlDoc
                .Descendants(namespaceVersion1 + "assemblyIdentity")
                .FirstOrDefault() ?? throw new ClickOnceDeploymentException($"Invalid manifest document for {this.applicationName}.application");

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceDeploymentException($"Version info is empty!");
            }

            return new(version);
        }

        private HttpClient HttpClientFactory(Uri uri = null)
        {
            this.logger.LogDebug(EventId, $"HttpClientFactory > returning HttpClient for url: {(uri is null ? "[to ba allocated]" : uri.ToString())}");

            var client = this.httpClientFactory.CreateClient(name: HttpClientKey);
            if (uri is not null)
            {
                client.BaseAddress = uri;
            }

            return client;
        }
    }
}
