namespace Husa.Uploader.Data.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using ListingWithInvalidTaxIdResponse = Husa.Quicklister.Extensions.Api.Contracts.Response.Listing.InvalidTaxIdListingsResponse;

    public class ListingRepositoryTests
    {
        private readonly Mock<IQuicklisterDfwClient> quicklisterDfwClientMock;
        private readonly Mock<IQuicklisterHarClient> quicklisterHarClientMock;
        private readonly Mock<IQuicklisterAborClient> quicklisterAborClientMock;
        private readonly Mock<ILogger<ListingRepository>> loggerMock;

        public ListingRepositoryTests()
        {
            this.quicklisterDfwClientMock = new Mock<IQuicklisterDfwClient>();
            this.quicklisterHarClientMock = new Mock<IQuicklisterHarClient>();
            this.quicklisterAborClientMock = new Mock<IQuicklisterAborClient>();
            this.loggerMock = new Mock<ILogger<ListingRepository>>();
        }

        private ListingRepository Sut => new(
                this.quicklisterDfwClientMock.Object,
                this.quicklisterHarClientMock.Object,
                this.quicklisterAborClientMock.Object,
                this.loggerMock.Object);

        [Theory]
        [InlineData(MarketCode.DFW)]
        [InlineData(MarketCode.Houston)]
        public async Task GetListingsWithInvalidTaxId_DfwMarket_ReturnsListings(MarketCode marketCode)
        {
            // Arrange
            var mockListings = new List<ListingWithInvalidTaxIdResponse>
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

            this.SetupClient(marketCode, mockListings);

            // Act
            var result = await this.Sut.GetListingsWithInvalidTaxId(marketCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var firstItem = result.First();
            Assert.Equal("12345", firstItem.MlsNumber);
            Assert.Equal("123 Main St", firstItem.Address);
            Assert.Equal("invalid-tax-id", firstItem.TaxId);
            Assert.Equal(marketCode, firstItem.Market);
            this.VerifyClientInvocation(marketCode);
        }

        [Fact]
        public async Task GetListingsWithInvalidTaxId_UnsupportedMarket_ThrowsNotSupportedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                this.Sut.GetListingsWithInvalidTaxId(MarketCode.CTX));
        }

        [Fact]
        public void Constructor_NullQuicklisterDfwClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ListingRepository(
                null,
                null,
                null,
                this.loggerMock.Object));

            Assert.Equal("quicklisterDfwClient", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ListingRepository(
                this.quicklisterDfwClientMock.Object,
                this.quicklisterHarClientMock.Object,
                this.quicklisterAborClientMock.Object,
                null));

            Assert.Equal("logger", exception.ParamName);
        }

        private void SetupClient(MarketCode marketCode, List<ListingWithInvalidTaxIdResponse> mockListings)
        {
            switch (marketCode)
            {
                case MarketCode.DFW:
                    this.quicklisterDfwClientMock
                    .Setup(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(mockListings);
                    break;
                case MarketCode.Houston:
                    this.quicklisterHarClientMock
                    .Setup(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(mockListings);
                    break;
                case MarketCode.Austin:
                    this.quicklisterAborClientMock
                    .Setup(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(mockListings);
                    break;
                default:
                    break;
            }
        }

        private void VerifyClientInvocation(MarketCode marketCode)
        {
            switch (marketCode)
            {
                case MarketCode.DFW:
                    this.quicklisterDfwClientMock.Verify(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()), Times.Once);
                    break;
                case MarketCode.Houston:
                    this.quicklisterHarClientMock.Verify(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()), Times.Once);
                    break;
                case MarketCode.Austin:
                    this.quicklisterAborClientMock.Verify(x => x.SaleListing.GetListingsWithInvalidTaxId(It.IsAny<CancellationToken>()), Times.Once);
                    break;
                default:
                    break;
            }
        }
    }
}
