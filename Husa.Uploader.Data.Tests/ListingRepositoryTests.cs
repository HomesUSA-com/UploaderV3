namespace Husa.Uploader.Data.Tests.Repositories
{
    ////using System;
    ////using System.Collections.Generic;
    ////using System.Linq;
    ////using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.Listing;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class ListingRepositoryTests
    {
        private readonly Mock<IOptions<ApplicationOptions>> applicationOptionsMock;
        private readonly Mock<IQuicklisterDfwClient> quicklisterDfwClientMock;
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClientMock;
        private readonly Mock<ILogger<ListingRepository>> loggerMock;

        public ListingRepositoryTests()
        {
            this.applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            this.quicklisterDfwClientMock = new Mock<IQuicklisterDfwClient>();
            this.serviceSubscriptionClientMock = new Mock<IServiceSubscriptionClient>();
            this.loggerMock = new Mock<ILogger<ListingRepository>>();
        }

        [Fact]
        public async Task GetListingsWithInvalidTaxId_DfwMarket_ReturnsListings()
        {
            // Arrange
            var mockListings = new List<InvalidTaxIdListingsResponse>
            {
                new()
                {
                    MlsNumber = "12345",
                    Address = "123 Main St",
                    TaxId = "invalid-tax-id",
                },
                new()
                {
                    MlsNumber = "67890",
                    Address = "456 Oak Ave",
                    TaxId = "another-invalid-tax-id",
                },
            };

            this.quicklisterDfwClientMock
                .Setup(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockListings);

            var repository = new ListingRepository(
                this.applicationOptionsMock.Object,
                this.quicklisterDfwClientMock.Object,
                this.serviceSubscriptionClientMock.Object,
                this.loggerMock.Object);

            // Act
            var result = await repository.GetListingsWithInvalidTaxId(MarketCode.DFW);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var firstItem = result.First();
            Assert.Equal("12345", firstItem.MlsNumber);
            Assert.Equal("123 Main St", firstItem.Address);
            Assert.Equal("invalid-tax-id", firstItem.TaxId);
            Assert.Equal("DFW", firstItem.Market);

            this.quicklisterDfwClientMock.Verify(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListingsWithInvalidTaxId_UnsupportedMarket_ThrowsNotSupportedException()
        {
            // Arrange
            var repository = new ListingRepository(
                this.applicationOptionsMock.Object,
                this.quicklisterDfwClientMock.Object,
                this.serviceSubscriptionClientMock.Object,
                this.loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                repository.GetListingsWithInvalidTaxId(MarketCode.CTX));
        }

        [Fact]
        public void Constructor_NullQuicklisterDfwClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ListingRepository(
                this.applicationOptionsMock.Object,
                null,
                this.serviceSubscriptionClientMock.Object,
                this.loggerMock.Object));

            Assert.Equal("quicklisterDfwClient", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ListingRepository(
                this.applicationOptionsMock.Object,
                this.quicklisterDfwClientMock.Object,
                this.serviceSubscriptionClientMock.Object,
                null));

            Assert.Equal("logger", exception.ParamName);
        }
    }
}
