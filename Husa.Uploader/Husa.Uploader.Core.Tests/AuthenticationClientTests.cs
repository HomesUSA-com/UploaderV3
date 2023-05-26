namespace Husa.Uploader.Core.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Core.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Moq;
    using Moq.Protected;
    using Xunit;

    public class AuthenticationClientTests
    {
        private readonly Mock<HttpMessageHandler> mockHttpMessageHandler = new();
        private readonly HttpClient httpClient;
        private readonly IOptions<JsonOptions> jsonOptions;

        public AuthenticationClientTests()
        {
            this.httpClient = new HttpClient(this.mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/auth/"),
            };
            this.jsonOptions = Options.Create(new JsonOptions());
        }

        [Fact]
        public async Task LoginAsync_ReturnsUserResponse_WhenLoginSuccessful()
        {
            // Arrange
            var userRequest = new UserRequest(
                username: "test",
                password: "password");

            var userResponse = new UserResponse
            {
                UserID = 10,
                UserGUID = Guid.NewGuid().ToString(),
                Username = "TestUser",
            };

            var serializedUserResponse = JsonSerializer.Serialize(userResponse);

            this.mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(serializedUserResponse, Encoding.UTF8, "application/json"),
                });
            var sut = this.GetSut();

            // Act
            var result = await sut.LoginAsync(userRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userResponse.UserID, result.UserID);
            Assert.Equal(userResponse.UserGUID, result.UserGUID);
            Assert.Equal(userResponse.Username, result.Username);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenResponseIsNotOk()
        {
            // Arrange
            var userRequest = new UserRequest(
                username: "test",
                password: "password");

            this.mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

            var sut = this.GetSut();

            // Act
            var result = await sut.LoginAsync(userRequest);

            // Assert
            Assert.Null(result);
        }

        private AuthenticationClient GetSut() => new(this.httpClient, this.jsonOptions);
    }
}
