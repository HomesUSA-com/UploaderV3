namespace Husa.Uploader.Core.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.MediaService.Domain.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.CTX.Domain.Enums;
    using Husa.Quicklister.CTX.Domain.Enums.Entities;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
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
        public async Task PartialUploadWithExistingListing()
        {
            // Arrange
            this.SetUpConfigs();
            var request = this.GetResidentialListingRequest(false);
            // Act
            var sut = this.GetSut();
            var result = await sut.PartialUpload(request);
            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Theory]
        [InlineData("CSLD")] // UpdateStatus_Sold
        [InlineData("AUC")] // UpdateStatus_ActiveUnderContract
        [InlineData("PND")] // UpdateStatus_Pending
        [InlineData("WDN")] // UpdateStatus_Withdrawn
        [InlineData("CS")] // UpdateStatus_Hold
        public async Task UpdateStatus_Success(string status)
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse());
            ctxListing.ListStatus = status;
            ctxListing.Directions = "This is a test for the directions info field";
            ctxListing.StreetNum = "10";
            ctxListing.BackOnMarketDate = DateTime.Now;
            ctxListing.OffMarketDate = DateTime.Now;
            ctxListing.EstClosedDate = DateTime.Now;
            ctxListing.AgentMarketUniqueId = "12234";
            ctxListing.SecondAgentMarketUniqueId = "354752";
            ctxListing.SoldPrice = 150000;
            ctxListing.SoldTerms = "CASH";
            ctxListing.WithdrawnDate = DateTime.Now;
            ctxListing.WithdrawalReason = "this is a test";
            ctxListing.IsWithdrawalListingAgreement = "1";
            ctxListing.OffMarketDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_InvalidStatusFail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse());
            ctxListing.ListStatus = "TEST";
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Failure, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SetGeocodesSuccess(bool updateGeocodes)
        {
            // Arrange
            this.SetUpConfigs();
            var request = this.GetResidentialListingRequest();
            request.UpdateGeocodes = updateGeocodes;
            request.Longitude = 24;
            request.Latitude = -97;
            var sut = this.GetSut();

            // Act
            var result = await sut.Upload(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateImagesSuccess()
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(false);
            var listingImages = new List<ResidentialListingMedia>()
            {
                new()
                {
                    Caption = "test.jpg",
                },
                new()
                {
                    Caption = string.Empty,
                },
            };

            this.mediaRepository
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>(), MediaType.ListingRequest))
                .ReturnsAsync(listingImages);
            this.uploaderClient
                .Setup(x => x.FindElement(It.IsAny<By>(), false, false).FindElement(It.IsAny<By>()).SendKeys(It.IsAny<string>()));

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateImages(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateImagesFailure()
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(false);

            this.mediaRepository
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>(), MediaType.ListingRequest))
                .ThrowsAsync(new Exception("error getting images"));
            this.uploaderClient
                .Setup(x => x.FindElement(It.IsAny<By>(), false, false).FindElement(It.IsAny<By>()).SendKeys(It.IsAny<string>()));

            // Act
            var sut = this.GetSut();
            var exception = await Assert.ThrowsAsync<Exception>(() => sut.UpdateImages(request));
            Assert.Equal("error getting images", exception.Message);
        }

        [Fact]
        public async Task UpdateImages_ProcessesMultipleImages()
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(false);

            // Configure mediaRepository to return at least five images.
            var listingImages = new List<ResidentialListingMedia>();
            for (int i = 0; i < 5; i++)
            {
                listingImages.Add(new ResidentialListingMedia { Caption = $"Image{i}.jpg", PathOnDisk = $"/path/to/image{i}.jpg" });
            }

            this.mediaRepository
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>(), MediaType.ListingRequest))
                .ReturnsAsync(listingImages);

            this.uploaderClient
                 .Setup(x => x.FindElement(It.IsAny<By>(), false, false).FindElement(It.IsAny<By>()).SendKeys(It.IsAny<string>()));

            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateImages(request);

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
                EnableOpenHouse = true,
            };

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateOpenHouse(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateLotPrice_NotImplementedException_Fails()
        {
            // Arrange
            var lotListingRequest = new Mock<LotListingRequest>();

            // Act && Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await this.GetSut().UpdateLotPrice(lotListingRequest.Object));
        }

        protected override LotListingRequest GetLotListingRequest(bool isNewListing = true)
        {
            throw new NotImplementedException();
        }

        protected override CtxUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
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
                UpdateGeocodes = true,
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
