namespace Husa.Uploader.Data.Tests
{
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.Amarillo.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Extensions.Api.Contracts.Request.SaleRequest;
    using Husa.Quicklister.Extensions.Api.Contracts.Response;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AborResponseContracts = Husa.Quicklister.Abor.Api.Contracts.Response;
    using AmarilloResponseContracts = Husa.Quicklister.Amarillo.Api.Contracts.Response;
    using CtxResponseContracts = Husa.Quicklister.CTX.Api.Contracts.Response;
    using DfwResponseContracts = Husa.Quicklister.Dfw.Api.Contracts.Response;
    using HarResponseContracts = Husa.Quicklister.Har.Api.Contracts.Response;
    using SaborEnums = Husa.Quicklister.Sabor.Domain.Enums;
    using SaborResponseContracts = Husa.Quicklister.Sabor.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class ListingRequestRepositoryTests
    {
        private readonly Mock<IQuicklisterSaborClient> mockSaborClient = new();
        private readonly Mock<IQuicklisterCtxClient> mockCtxClient = new();
        private readonly Mock<IQuicklisterAborClient> mockAborClient = new();
        private readonly Mock<IQuicklisterHarClient> mockHarClient = new();
        private readonly Mock<IQuicklisterDfwClient> mockDfwClient = new();
        private readonly Mock<IQuicklisterAmarilloClient> mockAmarilloClient = new();
        private readonly Mock<IServiceSubscriptionClient> mockServiceSubscriptionClient = new();
        private readonly Mock<ILogger<ListingRequestRepository>> mockLogger = new();
        private readonly ApplicationServicesFixture fixture;

        public ListingRequestRepositoryTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.mockServiceSubscriptionClient.SetupGet(m => m.Company).Returns(new Mock<ICompany>().Object);
        }

        [Fact]
        public async Task GetListingRequests_ReturnsResidentialListingRequests()
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

            var harResponse = new HarResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var harResult = GetListingRequestGridResponse(new[] { harResponse });

            this.mockHarClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harResult);

            var dfwResponse = new DfwResponseContracts.ListingRequest.SaleRequest.SaleListingRequestQueryResponse();
            var dfwResult = GetListingRequestGridResponse(new[] { dfwResponse });

            this.mockDfwClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dfwResult);

            var amarilloResponse = new AmarilloResponseContracts.ListingRequest.SaleRequest.SaleListingRequestQueryResponse();
            var amarilloResult = GetListingRequestGridResponse(new[] { amarilloResponse });

            this.mockAmarilloClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amarilloResult);

            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingRequests();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(6, result.Count());
        }

        [Theory]
        [InlineData(MarketCode.SanAntonio, RequestFieldChange.ListPrice)]
        [InlineData(MarketCode.Houston, RequestFieldChange.ListPrice)]
        [InlineData(MarketCode.Austin, RequestFieldChange.ListPrice)]
        [InlineData(MarketCode.CTX, RequestFieldChange.ListPrice)]
        [InlineData(MarketCode.DFW, RequestFieldChange.ListPrice)]
        [InlineData(MarketCode.Amarillo, RequestFieldChange.ListPrice)]
        public async Task GetListingRequestsByMarketAndAction_ReturnsResidentialListingRequests(MarketCode marketCode, RequestFieldChange requestFieldChange)
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
            var harResponse = new HarResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestQueryResponse();
            var harResult = GetListingRequestGridResponse(new[] { harResponse });
            this.mockHarClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harResult);
            var dfwResponse = new DfwResponseContracts.ListingRequest.SaleRequest.SaleListingRequestQueryResponse();
            var dfwResult = GetListingRequestGridResponse(new[] { dfwResponse });
            this.mockDfwClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dfwResult);
            var amarilloResponse = new AmarilloResponseContracts.ListingRequest.SaleRequest.SaleListingRequestQueryResponse();
            var amarilloResult = GetListingRequestGridResponse(new[] { amarilloResponse });
            this.mockAmarilloClient
                .Setup(x => x.ListingSaleRequest.GetListRequestAsync(It.IsAny<SaleListingRequestFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(amarilloResult);
            var sut = this.GetSut();
            // Act
            var result = await sut.GetListingRequestsByMarketAndAction(marketCode, requestFieldChange);
            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Theory]
        [InlineData(MarketCode.SanAntonio)]
        [InlineData(MarketCode.Houston)]
        [InlineData(MarketCode.Austin)]
        [InlineData(MarketCode.CTX)]
        [InlineData(MarketCode.DFW)]
        [InlineData(MarketCode.Amarillo)]
        public async Task GetListingRequest_ReturnsResidentialListingRequest(MarketCode marketCode)
        {
            // Arrange
            var requestId = Guid.NewGuid();
            this.SetUpGetRequestById(marketCode, requestId);
            var sut = this.GetSut();

            // Act
            var result = await sut.GetListingRequest(requestId, marketCode: marketCode);

            // Assert
            Assert.Equal(requestId, result.ResidentialListingRequestID);
            Assert.Equal(marketCode, result.MarketCode);
        }

        [Theory]
        [InlineData(MarketCode.SanAntonio)]
        [InlineData(MarketCode.Houston)]
        [InlineData(MarketCode.Austin)]
        [InlineData(MarketCode.CTX)]
        [InlineData(MarketCode.DFW)]
        [InlineData(MarketCode.Amarillo)]
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

        private static DocumentGridResponse<TSaleListingRequestResponse> GetListingRequestGridResponse<TSaleListingRequestResponse>(IEnumerable<TSaleListingRequestResponse> data)
            where TSaleListingRequestResponse : class, ISaleListingRequestResponse
            => new DocumentGridResponse<TSaleListingRequestResponse>(
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
            this.mockHarClient.Object,
            this.mockDfwClient.Object,
            this.mockAmarilloClient.Object,
            this.mockServiceSubscriptionClient.Object,
            this.mockLogger.Object);

        private void SetUpGetListingById(MarketCode market, Guid listingId, string mlsNumber)
        {
            switch (market)
            {
                case MarketCode.SanAntonio:
                    {
                        this.mockSaborClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new SaborResponseContracts.ListingSaleDetailResponse()
                            {
                                Id = listingId,
                                MlsNumber = mlsNumber,
                            });
                        break;
                    }

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

                case MarketCode.CTX:
                    {
                        this.mockCtxClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new CtxResponseContracts.ListingSaleDetailResponse()
                            {
                                Id = listingId,
                                MlsNumber = mlsNumber,
                            });
                        break;
                    }

                case MarketCode.DFW:
                    {
                        this.mockDfwClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new DfwResponseContracts.SaleListingDetailResponse()
                            {
                                Id = listingId,
                                MlsNumber = mlsNumber,
                            });
                        break;
                    }

                case MarketCode.Amarillo:
                    {
                        this.mockAmarilloClient
                            .Setup(x => x.SaleListing.GetByIdAsync(listingId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new AmarilloResponseContracts.SaleListingDetailResponse()
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
                case MarketCode.SanAntonio:
                    {
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
                        break;
                    }

                case MarketCode.Houston:
                    {
                        var response = new HarResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse
                        {
                            Id = requestId,
                            ListingSaleId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
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
                                OpenHouses = new List<HarResponseContracts.OpenHouseResponse>(),
                                Rooms = new List<HarResponseContracts.RoomResponse>(),
                            },
                            StatusFieldsInfo = new(),
                        };
                        this.mockHarClient
                            .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                case MarketCode.Austin:
                    {
                        var response = new AborResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse
                        {
                            Id = requestId,
                            ListingSaleId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
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
                                OpenHouses = new List<AborResponseContracts.OpenHouseResponse>(),
                                Rooms = new List<AborResponseContracts.RoomResponse>(),
                            },
                            StatusFieldsInfo = new(),
                        };
                        this.mockAborClient
                            .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                case MarketCode.CTX:
                    {
                        var response = new CtxResponseContracts.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse
                        {
                            Id = requestId,
                            ListingSaleId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
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
                                OpenHouses = new List<CtxResponseContracts.OpenHouseResponse>(),
                                Rooms = new List<CtxResponseContracts.RoomResponse>(),
                            },
                            StatusFieldsInfo = new(),
                        };
                        this.mockCtxClient
                            .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                case MarketCode.DFW:
                    {
                        var response = new DfwResponseContracts.ListingRequest.SaleRequest.SaleListingRequestDetailResponse
                        {
                            Id = requestId,
                            ListingSaleId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
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
                                OpenHouses = new List<DfwResponseContracts.OpenHouseResponse>(),
                                Rooms = new List<DfwResponseContracts.RoomResponse>(),
                            },
                            StatusFieldsInfo = new(),
                        };
                        this.mockDfwClient
                            .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                case MarketCode.Amarillo:
                    {
                        var response = new AmarilloResponseContracts.ListingRequest.SaleRequest.SaleListingRequestDetailResponse
                        {
                            Id = requestId,
                            ListingSaleId = listingId,
                            ListPrice = 1234567,
                            MlsNumber = "fake-mls-num",
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
                                OpenHouses = new List<AmarilloResponseContracts.OpenHouseResponse>(),
                                Rooms = new List<AmarilloResponseContracts.RoomResponse>(),
                            },
                            StatusFieldsInfo = new(),
                        };
                        this.mockAmarilloClient
                            .Setup(x => x.ListingSaleRequest.GetListRequestSaleByIdAsync(requestId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(response);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
