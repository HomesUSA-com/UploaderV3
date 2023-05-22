namespace Husa.Uploader.Data.Tests
{
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class SqlDataLoaderTest
    {
        private readonly Mock<CosmosClient> cosmosClient = new();
        private readonly Mock<IOptions<CosmosDbOptions>> options = new();
        private readonly Mock<IOptions<ApplicationOptions>> applicationOptions = new();
        private readonly Mock<ILogger<SqlDataLoader>> logger = new();
        private readonly Mock<IQuicklisterSaborClient> quicklisterSaborClient = new();
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
            this.cosmosClient.Object,
            this.fixture.CosmosDbOptions.Object,
            this.fixture.ApplicationOptions.Object,
            this.quicklisterSaborClient.Object,
            this.logger.Object);
    }
}
