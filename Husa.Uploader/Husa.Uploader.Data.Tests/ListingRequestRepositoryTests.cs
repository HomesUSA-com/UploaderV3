namespace Husa.Uploader.Data.Tests
{
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using CtxRequestContracts = Husa.Quicklister.CTX.Api.Contracts.Request;
    using CtxResponseContracts = Husa.Quicklister.CTX.Api.Contracts.Response;
    using SaborEnums = Husa.Quicklister.Sabor.Domain.Enums;
    using SaborRequestContracts = Husa.Quicklister.Sabor.Api.Contracts.Request;
    using SaborResponseContracts = Husa.Quicklister.Sabor.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class ListingRequestRepositoryTests
    {
        private readonly Mock<IQuicklisterSaborClient> mockSaborClient = new();
        private readonly Mock<IQuicklisterCtxClient> mockCtxClient = new();
        private readonly Mock<ILogger<ListingRequestRepository>> mockLogger = new();
        private readonly ApplicationServicesFixture fixture;

        public ListingRequestRepositoryTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetListingData_ReturnsResidentialListingRequests()
        {
            // Arrange
            var saborResponse = new SaborResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var saborResult = new SaborResponseContracts.ListingRequest.SaleRequest.ListingRequestGridResponse<SaborResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse>(
                data: new[] { saborResponse },
                total: 1,
                continuationToken: string.Empty,
                currentToken: string.Empty,
                previousToken: string.Empty);

            this.mockSaborClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(
                    It.IsAny<SaborRequestContracts.SaleRequest.ListingSaleRequestFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(saborResult);

            var ctxResponse = new CtxResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var ctxResult = new CtxResponseContracts.ListingRequest.SaleRequest.ListingRequestGridResponse<CtxResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse>(
                data: new[] { ctxResponse },
                total: 1,
                continuationToken: string.Empty,
                currentToken: string.Empty,
                previousToken: string.Empty);

            this.mockCtxClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(
                    It.IsAny<CtxRequestContracts.SaleRequest.ListingSaleRequestFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ctxResult);

            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingData();

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetListingRequest_ReturnsResidentialListingRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var listingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var requestDate = DateTime.UtcNow;
            var saborResponse = new SaborResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse
            {
                Id = requestId,
                ListingSaleId = listingId,
                ListPrice = 1234567,
                MlsNumber = "fake-mls-num",
                MlsStatus = SaborEnums.MarketStatuses.Active,
                SysCreatedOn = requestDate,
                SysCreatedBy = userId,
                SysModifiedOn = requestDate.AddMonths(1),
                SysModifiedBy = userId,
                SaleProperty = new()
                {
                    AddressInfo = new(),
                    FeaturesInfo = new(),
                    FinancialInfo = new(),
                    SalePropertyInfo = new(),
                    PropertyInfo = new(),
                    SchoolsInfo = new(),
                    ShowingInfo = new(),
                    SpacesDimensionsInfo = new(),
                    Hoas = new List<SaborResponseContracts.SalePropertyDetail.HoaResponse>(),
                    OpenHouses = new List<SaborResponseContracts.OpenHouseResponse>(),
                    Rooms = new List<SaborResponseContracts.RoomResponse>(),
                },
            };
            this.mockSaborClient
                .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(saborResponse);
            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingRequest(requestId, marketCode: MarketCode.SanAntonio);

            // Assert
            var type = Assert.IsType<SaborListingRequest>(result);
            Assert.Equal(saborResponse.Id, result.ResidentialListingRequestID);
            Assert.Equal(MarketCode.SanAntonio, type.MarketCode);
        }

        private ListingRequestRepository GetSut() => new(
            this.fixture.ApplicationOptions.Object,
            this.mockSaborClient.Object,
            this.mockCtxClient.Object,
            this.mockLogger.Object);
    }
}
