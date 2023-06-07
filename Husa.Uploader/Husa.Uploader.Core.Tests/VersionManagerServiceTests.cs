namespace Husa.Uploader.Core.Tests
{
    using System.IO.Abstractions;
    using System.Text.RegularExpressions;
    using Husa.Uploader.Core.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class VersionManagerServiceTests
    {
        private readonly Mock<IHttpClientFactory> httpClientFactory = new();
        private readonly Mock<ILogger<VersionManagerService>> logger = new();
        private readonly Mock<IFileSystem> mockFileProvider = new();
        private readonly Mock<IFile> mockFile = new();
        private readonly ApplicationServicesFixture fixture;

        public VersionManagerServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public void CurrentClickOnceVersion_ParseValidVersionString_ReturnsVersion()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ClickOnce_CurrentVersion", "1.0.0.0");

            // Act
            var version = VersionManagerService.CurrentClickOnceVersion;

            // Assert
            Assert.NotNull(version);
            Assert.Equal(new Version(1, 0, 0, 0), version);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new VersionManagerService(
                options: null,
                httpClientFactory: this.httpClientFactory.Object,
                fileProvider: this.mockFileProvider.Object,
                logger: this.logger.Object));
        }

        [Fact]
        public void Constructor_NullClientFactory_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new VersionManagerService(
                options: this.fixture.ApplicationOptions,
                httpClientFactory: null,
                fileProvider: this.mockFileProvider.Object,
                logger: this.logger.Object));
        }

        [Fact]
        public void Constructor_NullFileProvider_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new VersionManagerService(
                options: this.fixture.ApplicationOptions,
                httpClientFactory: this.httpClientFactory.Object,
                fileProvider: null,
                logger: this.logger.Object));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new VersionManagerService(
                options: this.fixture.ApplicationOptions,
                httpClientFactory: this.httpClientFactory.Object,
                fileProvider: this.mockFileProvider.Object,
                logger: null));
        }

        [Fact]
        public async Task CheckForUpdateAsync_VersionCheckDisabled_ReturnsFalse()
        {
            // Arrange
            this.fixture.ApplicationOptions.Value.FeatureFlags.IsVersionCheckEnabled = false;
            var service = this.GetSut();

            // Act
            var result = await service.CheckForUpdateAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckForUpdateAsync_VersionCheckEnabledButCurrentVersionIsHigher_ReturnsFalse()
        {
            // Arrange
            var applicationManifest = GetApplicationManifest("1.0.0.0");
            this.fixture.ApplicationOptions.Value.FeatureFlags.IsVersionCheckEnabled = true;
            this.mockFileProvider.Setup(fs => fs.File).Returns(this.mockFile.Object);
            this.mockFile
                .Setup(file => file.Exists(It.IsAny<string>()))
                .Returns(true);
            this.mockFile
                .Setup(file => file.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(applicationManifest);

            Environment.SetEnvironmentVariable("ClickOnce_CurrentVersion", "2.0.0.0");
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // The remote server version is set to 1.0.0.0 in the stream returned by the HttpClient.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { Content = new StringContent(applicationManifest) });
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            this.httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);
            var service = this.GetSut();

            // Act
            var result = await service.CheckForUpdateAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckForUpdateAsync_VersionCheckEnabledAndCurrentVersionIsLower_ReturnsTrue()
        {
            // Arrange
            const string currentVersion = "1.0.0.0";
            var applicationManifest = GetApplicationManifest("2.0.0.0");
            this.fixture.ApplicationOptions.Value.FeatureFlags.IsVersionCheckEnabled = true;
            this.mockFileProvider.Setup(fs => fs.File).Returns(this.mockFile.Object);
            this.mockFile
                .Setup(file => file.Exists(It.IsAny<string>()))
                .Returns(true);
            this.mockFile
                .Setup(file => file.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetApplicationManifest(currentVersion));

            Environment.SetEnvironmentVariable("ClickOnce_CurrentVersion", currentVersion);
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // The remote server version is set to 2.0.0.0 in the stream returned by the HttpClient.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { Content = new StringContent(applicationManifest) });
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            this.httpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(mockHttpClient);

            var service = this.GetSut();

            // Act
            var result = await service.CheckForUpdateAsync();

            // Assert
            Assert.True(result);
        }

        private static string GetApplicationManifest(string version)
        {
            var folderVersion = Regex.Replace(version, @"\.", "_");
            var manifest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <asmv1:assembly xsi:schemaLocation=""urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd"" manifestVersion=""1.0"" xmlns:asmv1=""urn:schemas-microsoft-com:asm.v1"" xmlns=""urn:schemas-microsoft-com:asm.v2"" xmlns:asmv2=""urn:schemas-microsoft-com:asm.v2"" xmlns:xrml=""urn:mpeg:mpeg21:2003:01-REL-R-NS"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:asmv3=""urn:schemas-microsoft-com:asm.v3"" xmlns:dsig=""http://www.w3.org/2000/09/xmldsig#"" xmlns:co.v1=""urn:schemas-microsoft-com:clickonce.v1"" xmlns:co.v2=""urn:schemas-microsoft-com:clickonce.v2"">
                  <assemblyIdentity name=""Husa.Uploader.Desktop.application"" version=""{version}"" publicKeyToken=""0000000000000000"" language=""en-US"" processorArchitecture=""msil"" xmlns=""urn:schemas-microsoft-com:asm.v1"" />
                  <description asmv2:publisher=""HomesUSA.com"" co.v1:suiteName=""HomesUSA"" asmv2:product=""HomesUSA Uploader Core"" asmv2:supportUrl=""https://www.homesusa.com/mls_department/"" co.v1:errorReportUrl=""https://www.homesusa.com/mls_department/"" xmlns=""urn:schemas-microsoft-com:asm.v1"" />
                  <deployment install=""true"" mapFileExtensions=""true"" co.v1:createDesktopShortcut=""true"">
                    <subscription>
                      <update>
                        <beforeApplicationStartup />
                      </update>
                    </subscription>
                    <deploymentProvider codebase=""https://localhost/Husa.Uploader.Desktop.application"" />
                  </deployment>
                  <compatibleFrameworks xmlns=""urn:schemas-microsoft-com:clickonce.v2"">
                    <framework targetVersion=""4.5"" profile=""Full"" supportedRuntime=""4.0.30319"" />
                  </compatibleFrameworks>
                  <dependency>
                    <dependentAssembly dependencyType=""install"" codebase=""Application Files\Husa.Uploader.Desktop_{folderVersion}\Husa.Uploader.Desktop.dll.manifest"" size=""184658"">
                      <assemblyIdentity name=""Husa.Uploader.Desktop.exe"" version=""{version}"" publicKeyToken=""0000000000000000"" language=""en-US"" processorArchitecture=""msil"" type=""win32"" />
                      <hash>
                        <dsig:Transforms>
                          <dsig:Transform Algorithm=""urn:schemas-microsoft-com:HashTransforms.Identity"" />
                        </dsig:Transforms>
                        <dsig:DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha256"" />
                        <dsig:DigestValue>vQwB6MeVXt11j28W959RMA7acPHRt4VIbUaFuRbB44U=</dsig:DigestValue>
                      </hash>
                    </dependentAssembly>
                  </dependency>
                </asmv1:assembly>";

            return manifest;
        }

        private VersionManagerService GetSut()
            => new(
                options: this.fixture.ApplicationOptions,
                httpClientFactory: this.httpClientFactory.Object,
                fileProvider: this.mockFileProvider.Object,
                logger: this.logger.Object);
    }
}
