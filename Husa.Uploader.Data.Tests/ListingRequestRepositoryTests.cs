namespace Husa.Uploader.Data.Tests
{
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Extensions.Api.Contracts.Request.SaleRequest;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AborResponseContracts = Husa.Quicklister.Abor.Api.Contracts.Response;
    using CtxResponseContracts = Husa.Quicklister.CTX.Api.Contracts.Response;
    using SaborEnums = Husa.Quicklister.Sabor.Domain.Enums;
    using SaborResponseContracts = Husa.Quicklister.Sabor.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class ListingRequestRepositoryTests
    {
        private readonly Mock<IQuicklisterSaborClient> mockSaborClient = new();
        private readonly Mock<IQuicklisterCtxClient> mockCtxClient = new();
        private readonly Mock<IQuicklisterAborClient> mockAborClient = new();
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
            var saborResult = GetListingRequestGridResponse(new[] { saborResponse });

            this.mockSaborClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(saborResult);

            var ctxResponse = new CtxResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var ctxResult = GetListingRequestGridResponse(new[] { ctxResponse });

            this.mockCtxClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ctxResult);

            var aborResponse = new AborResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var aborResult = GetListingRequestGridResponse(new[] { aborResponse });

            this.mockAborClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborResult);

            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingData();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count());
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
                StatusFieldsInfo = new(),
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

        private static ListingRequestGridResponse<TSaleListingRequestResponse> GetListingRequestGridResponse<TSaleListingRequestResponse>(IEnumerable<TSaleListingRequestResponse> data)
            where TSaleListingRequestResponse : class, ISaleListingRequestResponse
            => new ListingRequestGridResponse<TSaleListingRequestResponse>(
                data: data,
                total: 1,
                continuationToken: string.Empty,
                currentToken: string.Empty,
                previousToken: string.Empty);

        private ListingRequestRepository GetSut() => new(
            this.fixture.ApplicationOptions.Object,
            this.mockSaborClient.Object,
            this.mockCtxClient.Object,
            this.mockAborClient.Object,
            this.mockLogger.Object);
    }
}
