namespace Husa.Uploader.Data.Tests.LotRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AborResponseContracts = Husa.Quicklister.Abor.Api.Contracts.Response;
    using HarResponseContracts = Husa.Quicklister.Har.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class LotListingRequestRepositoryTests
    {
        private readonly Mock<IQuicklisterAborClient> mockAborClient = new();
        private readonly Mock<IQuicklisterHarClient> mockHarClient = new();
        private readonly Mock<IServiceSubscriptionClient> mockServiceSubscriptionClient = new();
        private readonly Mock<ILogger<LotListingRequestRepository>> mockLogger = new();
        private readonly ApplicationServicesFixture fixture;

        public LotListingRequestRepositoryTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.mockServiceSubscriptionClient.SetupGet(m => m.Company).Returns(new Mock<ICompany>().Object);
        }

        [Theory]
        [InlineData(MarketCode.Houston)]
        [InlineData(MarketCode.Austin)]
        public async Task GetLotListingRequest_ReturnsResidentialListingRequest(MarketCode marketCode)
        {
            // Arrange
            var requestId = Guid.NewGuid();
            this.SetUpGetRequestById(marketCode, requestId);
            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingRequest(requestId, marketCode: marketCode);

            // Assert
            Assert.Equal(requestId, result.LotListingRequestID);
            Assert.Equal(marketCode, result.MarketCode);
        }

        [Theory]
        [InlineData(MarketCode.Houston)]
        [InlineData(MarketCode.Austin)]
        public async Task GetListingMlsNumber_Succeess(MarketCode marketCode)
        {
            // Arrange
            var listingId = Guid.NewGuid();
            var mlsNumber = "14526";
            this.SetUpGetListingById(marketCode, listingId, mlsNumber);
            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingMlsNumber(listingId, marketCode: marketCode);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(mlsNumber, result);
        }

        [Fact]
        public async Task GetListingMlsNumber_Exception()
        {
            // Arrange
            var sut = this.GetSut();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => sut.GetListingMlsNumber(Guid.Empty, marketCode: MarketCode.SanAntonio));
        }

        private LotListingRequestRepository GetSut() => new(
            this.fixture.ApplicationOptions.Object,
            this.mockAborClient.Object,
            this.mockHarClient.Object,
            this.mockLogger.Object);

        private void SetUpGetListingById(MarketCode market, Guid listingId, string mlsNumber)
        {
            switch (market)
            {
                case MarketCode.Houston:
                    {
                        this.mockHarClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new HarResponseContracts.ListingSaleDetailResponse()
                            {
                                Id = listingId,
                                MlsNumber = mlsNumber,
                            });
                        break;
                    }

                case MarketCode.Austin:
                    {
                        this.mockAborClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new AborResponseContracts.ListingSaleDetailResponse()
                            {
                                Id = listingId,
                                MlsNumber = mlsNumber,
                            });
                        break;
                    }

                default:
                    break;
            }
        }

        private void SetUpGetRequestById(MarketCode market, Guid requestId)
        {
            var listingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var requestDate = DateTime.UtcNow;

            switch (market)
            {
                case MarketCode.Houston:
                    {
                        var response = new HarResponseContracts.ListingRequest.LotRequest.LotListingRequestDetailResponse
                        {
                            Id = requestId,
                            ListingId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
                            SysCreatedOn = requestDate,
                            SysCreatedBy = userId,
                            SysModifiedOn = requestDate.AddMonths(1),
                            SysModifiedBy = userId,
                            StatusFieldsInfo = new(),
                            AddressInfo = new(),
                            FeaturesInfo = new(),
                            FinancialInfo = new(),
                            PropertyInfo = new(),
                            SchoolsInfo = new(),
                            ShowingInfo = new(),
                        };
                        this.mockHarClient
                            .Setup(x => x.ListingLotRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                case MarketCode.Austin:
                    {
                        var response = new AborResponseContracts.ListingRequest.LotRequest.LotListingRequestDetailResponse
                        {
                            Id = requestId,
                            ListingId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
                            SysCreatedOn = requestDate,
                            SysCreatedBy = userId,
                            SysModifiedOn = requestDate.AddMonths(1),
                            SysModifiedBy = userId,
                            AddressInfo = new(),
                            FeaturesInfo = new(),
                            FinancialInfo = new(),
                            PropertyInfo = new(),
                            SchoolsInfo = new(),
                            ShowingInfo = new(),
                            StatusFieldsInfo = new(),
                        };
                        this.mockAborClient
                            .Setup(x => x.ListingLotRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
