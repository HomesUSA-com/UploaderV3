namespace Husa.Uploader.Core.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.CTX.Domain.Enums;
    using Husa.Quicklister.CTX.Domain.Enums.Entities;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AddressInfoResponse = Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail.AddressInfoResponse;

    [Collection(nameof(ApplicationServicesFixture))]
    public class CtxUploadServiceTests : MarketUploadServiceTests<CtxUploadService, CtxListingRequest>
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<CtxUploadService>> logger = new();
        private readonly ApplicationServicesFixture fixture;

        public CtxUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
            this.uploaderClient.Setup(x => x.FillFieldSingleOption(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
        }

        [Fact]
        public async Task UpdateStatus_SoldSuccess()
        {
            // Arrange
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse())
            {
                ListStatus = "CSLD",
                BackOnMarketDate = DateTime.Now,
                OffMarketDate = DateTime.Now,
            };
            this.SetUpConfigs(ctxListing, setUpVirtualTours: false);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateStatus(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateOpenHouseNoOpenHousesSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse())
            {
                MLSNum = "mlsNum",
            };

            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ctxListing);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateOpenHouse(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        protected override CtxUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.sqlDataLoader.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);

        protected override CtxListingRequest GetEmptyListingRequest()
            => new CtxListingRequest(new ListingSaleRequestDetailResponse());

        protected override ResidentialListingRequest GetResidentialListingRequest(bool isNewListing = true)
        {
            var listingSale = GetListingRequestDetailResponse(isNewListing);
            return new CtxListingRequest(listingSale).CreateFromApiResponseDetail();
        }

        private static ListingSaleRequestDetailResponse GetListingRequestDetailResponse(bool isNewListing)
        {
            var spacesDimensionsInfo = new Mock<SpacesDimensionsResponse>();
            var addressInfo = new AddressInfoResponse()
            {
                City = Cities.Abbott,
                County = Counties.Atascosa,
                State = States.Texas,
                StreetType = StreetType.Street,
            };
            var propertyInfo = new PropertyInfoResponse()
            {
                ConstructionStage = ConstructionStage.UnderConstruction,
                DocumentsAvailable = new List<DocumentsAvailableDescription> { DocumentsAvailableDescription.BuildingPlans },
            };
            var featuresInfo = new Mock<FeaturesResponse>();
            var financialInfo = new FinancialResponse()
            {
                HoaAmount = 0,
                TaxRate = 1000,
                TaxYear = 2018,
            };
            var schoolsInfo = new SchoolsResponse()
            {
                ElementarySchool = ElementarySchool.AcademyElementarySchool,
                HighSchool = HighSchool.AcademyHighSchool,
                MiddleSchool = MiddleSchool.AcademyMiddleSchool,
                SchoolDistrict = SchoolDistrict.Academy,
            };
            var showingInfo = new ShowingResponse()
            {
                BuyersAgentCommissionType = Quicklister.Extensions.Domain.Enums.CommissionType.Amount,
                AgentBonusAmountType = Quicklister.Extensions.Domain.Enums.CommissionType.Percent,
                HasBonusWithAmount = true,
                LockboxLocation = new List<LockboxLocationDescription>() { LockboxLocationDescription.None },
                Showing = new List<ShowingInstructionsDescription>() { ShowingInstructionsDescription.None },
            };
            var salePropertyInfo = new SalePropertyResponse()
            {
                OwnerName = "OwnerName",
                PlanName = "PlanName",
                CompanyId = Guid.NewGuid(),
                CommunityId = Guid.NewGuid(),
                PlanId = Guid.NewGuid(),
            };
            var roomInfo = new RoomResponse()
            {
                Id = Guid.NewGuid(),
                Level = RoomLevel.Upper,
                RoomType = RoomType.Bedroom,
                Length = 100,
                Width = 100,
            };
            var saleProperty = new SalePropertyDetailResponse()
            {
                SpacesDimensionsInfo = spacesDimensionsInfo.Object,
                AddressInfo = addressInfo,
                PropertyInfo = propertyInfo,
                FeaturesInfo = featuresInfo.Object,
                FinancialInfo = financialInfo,
                SchoolsInfo = schoolsInfo,
                ShowingInfo = showingInfo,
                SalePropertyInfo = salePropertyInfo,
                Rooms = new List<RoomResponse>()
                {
                    roomInfo,
                },
            };

            var statusFields = new Mock<ListingSaleStatusFieldsResponse>();

            var listingSale = new ListingSaleRequestDetailResponse()
            {
                SaleProperty = saleProperty,
                ListPrice = 127738,
                StatusFieldsInfo = statusFields.Object,
                Id = Guid.NewGuid(),
                ListingSaleId = Guid.NewGuid(),
                LockedStatus = Quicklister.Extensions.Domain.Enums.LockedStatus.NoLocked,
                MlsStatus = MarketStatuses.Active,
                MlsNumber = isNewListing ? null : "mlsNumber",
            };

            return listingSale;
        }
    }
}
