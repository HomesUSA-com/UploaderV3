namespace Husa.Uploader.Data.Tests
{
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class SqlDataLoaderTest
    {
        private readonly Mock<ILogger<SqlDataLoader>> logger = new();
        private readonly Mock<IQuicklisterSaborClient> quicklisterSaborClient = new();
        private readonly Mock<IQuicklisterCtxClient> quicklisterCtxClient = new();
        private readonly ApplicationServicesFixture fixture;

        public SqlDataLoaderTest(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetListingDataSuccess()
        {
            // Arrange
            var repository = this.GetSut();

            // Act
            var result = await repository.GetListingData();

            // Assert
            Assert.NotEmpty(result);
        }

        private SqlDataLoader GetSut() => new(
            this.fixture.ApplicationOptions.Object,
            this.quicklisterSaborClient.Object,
            this.quicklisterCtxClient.Object,
            this.logger.Object);
    }
}
