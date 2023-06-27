namespace Husa.Uploader.Core.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Migration.Api.Client;
    using Husa.Migration.Api.Client.Interfaces;
    using Husa.Migration.Api.Contracts.Request;
    using Husa.Migration.Api.Contracts.Response;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class AuthenticationClientTests
    {
        private readonly Mock<IMigrationClient> migrationClient = new();
        private readonly Mock<IConnection> connectionResource = new();
        private readonly Mock<ILogger<AuthenticationService>> logger = new();
        private readonly IOptions<ApplicationOptions> appOptions;

        public AuthenticationClientTests()
        {
            this.appOptions = Options.Create(new ApplicationOptions
            {
                MarketInfo = new(),
                Uploader = new(),
                Services = new(),
                FeatureFlags = new(),
            });
        }

        [Fact]
        public async Task LoginAsync_ReturnsUserResponse_WhenLoginSuccessful()
        {
            // Arrange
            const string username = "test";
            const string password = "password";
            var userResponse = new UserAuthResponse
            {
                UserID = 10,
                UserGUID = Guid.NewGuid().ToString(),
                Username = "TestUser",
            };

            this.connectionResource
                .Setup(c => c.LoginAsync(
                    It.Is<UserRequest>(u => u.UserName == username && u.Password == password),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userResponse)
                .Verifiable();

            this.migrationClient
                .SetupGet(c => c.Connections)
                .Returns(this.connectionResource.Object);

            var sut = this.GetSut();

            // Act
            var result = await sut.LoginAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userResponse.UserID, result.UserID);
            Assert.Equal(userResponse.UserGUID, result.UserGUID);
            Assert.Equal(userResponse.Username, result.Username);
            this.connectionResource.Verify();
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenResponseIsNotOk()
        {
            // Arrange
            const string username = "test";
            const string password = "password";

            this.connectionResource
                .Setup(c => c.LoginAsync(
                    It.Is<UserRequest>(u => u.UserName == username && u.Password == password),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserAuthResponse)null)
                .Verifiable();

            this.migrationClient
                .SetupGet(c => c.Connections)
                .Returns(this.connectionResource.Object);

            var sut = this.GetSut();

            // Act
            var result = await sut.LoginAsync(username, password);

            // Assert
            Assert.Null(result);
        }

        private AuthenticationService GetSut() => new(
            this.migrationClient.Object,
            this.appOptions,
            this.logger.Object);
    }
}
