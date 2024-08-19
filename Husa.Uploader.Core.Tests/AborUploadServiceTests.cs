namespace Husa.Uploader.Core.Tests
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Entities.MarketRequests.LotRequest;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;
    using AborResponse = Husa.Quicklister.Abor.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class AborUploadServiceTests : MarketUploadServiceTests<AborUploadService, AborListingRequest>
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<AborUploadService>> logger = new();
        private readonly ApplicationServicesFixture fixture;

        public AborUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
        }

        [Fact]
        public async Task UploadWithExistingListing()
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(false);

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
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

        [Fact]
        public async Task UpdateStatus_HoldSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Hold";
            aborListing.BackOnMarketDate = DateTime.Now;
            aborListing.OffMarketDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_PendingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Pending";
            aborListing.PendingDate = DateTime.Now;
            aborListing.EstClosedDate = DateTime.Now;
            aborListing.ExpiredDate = DateTime.Now;
            aborListing.HasContingencyInfo = false;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ClosedSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Closed";
            aborListing.PendingDate = DateTime.Now;
            aborListing.ClosedDate = DateTime.Now;
            aborListing.AgentMarketUniqueId = "123456789";
            aborListing.SecondAgentMarketUniqueId = "1234567";
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ActiveUnderContractSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "ActiveUnderContract";
            aborListing.PendingDate = DateTime.Now;
            aborListing.ClosedDate = DateTime.Now;
            aborListing.EstClosedDate = DateTime.Now;
            aborListing.HasContingencyInfo = false;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Theory]
        [InlineData("Canceled")] // UpdateStatus_Canceled
        [InlineData("Hold")] // UpdateStatus_Hold
        [InlineData("Pending")] // UpdateStatus_Pending
        public async Task UpdateLotStatus_Success(string status)
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborLotListingRequest(new AborResponse.ListingRequest.LotRequest.LotListingRequestDetailResponse());
            aborListing.ListStatus = status;
            aborListing.Directions = "This is a test for the directions info field";
            aborListing.StreetNum = "10";
            aborListing.BackOnMarketDate = DateTime.Now;
            aborListing.OffMarketDate = DateTime.Now;
            aborListing.PendingDate = DateTime.Now;
            aborListing.EstClosedDate = DateTime.Now;
            aborListing.ExpiredDate = DateTime.Now;
            aborListing.HasContingencyInfo = false;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateLotStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateLotStatus_InvalidStatusFail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var aborListing = new AborLotListingRequest(new AborResponse.ListingRequest.LotRequest.LotListingRequestDetailResponse());
            aborListing.ListStatus = "TEST";
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateLotStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Failure, result);
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
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
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
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
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
                .Setup(x => x.GetListingImages(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
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
        public async Task AddOpenHouseSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var openHouses = new List<OpenHouseRequest>()
            {
                new OpenHouseRequest()
                {
                    Type = Crosscutting.Enums.OpenHouseType.Public,
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Date = OpenHouseExtensions.GetNextWeekday(DateTime.Today, DayOfWeek.Monday),
                    Active = true,
                    Comments = "Test",
                    Refreshments = new List<Refreshments>()
                     {
                         Refreshments.Beverages,
                     }.ToStringFromEnumMembers(),
                },
                new OpenHouseRequest()
                {
                    Type = Crosscutting.Enums.OpenHouseType.Public,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    Date = OpenHouseExtensions.GetNextWeekday(DateTime.Today, DayOfWeek.Thursday),
                    Active = true,
                    Comments = "Test",
                    Refreshments = new List<Refreshments>()
                     {
                         Refreshments.Pastries,
                     }.ToStringFromEnumMembers(),
                },
            };

            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.OpenHouse = openHouses;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SetLotGeocodesSuccess(bool updateGeocodes)
        {
            // Arrange
            this.SetUpConfigs();
            var request = this.GetLotListingRequest();
            request.UpdateGeocodes = updateGeocodes;
            request.Longitude = 24;
            request.Latitude = -97;
            var sut = this.GetSut();

            // Act
            var result = await sut.UploadLot(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public void GetComments_RefreshmentsAndLunch_ReturnsCombinedString()
        {
            bool refreshments = true;
            bool lunch = true;

            string result = OpenHouseExtensions.GetComments(refreshments, lunch);

            Assert.Equal("refreshments, lunch", result);
        }

        [Fact]
        public void GetComments_RefreshmentsOnly_ReturnsRefreshments()
        {
            bool refreshments = true;
            bool lunch = false;

            string result = OpenHouseExtensions.GetComments(refreshments, lunch);

            Assert.Equal("refreshments", result);
        }

        [Fact]
        public void GetComments_NoRefreshmentsOrLunch_ReturnsEmptyString()
        {
            bool refreshments = false;
            bool lunch = false;

            string result = OpenHouseExtensions.GetComments(refreshments, lunch);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task UpdateLotPrice_Success()
        {
            // Arrange
            this.SetUpConfigs();
            var lotListingRequest = this.GetLotListingRequest();

            // Act
            var result = await this.GetSut().UpdateLotPrice(lotListingRequest);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateLotPrice_ListingException_Fails()
        {
            // Arrange
            this.SetUpConfigs();
            var lotListingRequest = this.GetLotListingRequest();
            this.uploaderClient
                .Setup(x => x.WaitUntilElementIsDisplayed(It.Is<By>(x => x == By.LinkText("Price Change")), It.IsAny<CancellationToken>()))
                .Throws(new Exception("price update failure"));

            // Act
            var result = await this.GetSut().UpdateLotPrice(lotListingRequest);

            // Assert
            Assert.Equal(UploadResult.Failure, result);
        }

        protected override AborUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);

        protected override AborListingRequest GetEmptyListingRequest()
            => new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());

        protected override ResidentialListingRequest GetResidentialListingRequest(bool isNewListing = true)
        {
            var listingSale = GetListingRequestDetailResponse(isNewListing);
            return new AborListingRequest(listingSale).CreateFromApiResponseDetail();
        }

        protected override LotListingRequest GetLotListingRequest(bool isNewListing = true)
        {
            var listingSale = GetLotListingRequestDetailResponse(isNewListing);
            return new AborLotListingRequest(listingSale).CreateFromApiResponseDetail();
        }

        private static AborResponse.ListingRequest.LotRequest.LotListingRequestDetailResponse GetLotListingRequestDetailResponse(bool isNewListing)
        {
            var propertyInfo = new AborResponse.LotListing.LotPropertyResponse()
            {
                PropCondition = new List<PropCondition>() { PropCondition.UnderConstruction },
                UpdateGeocodes = true,
            };

            var showingInfo = new AborResponse.LotListing.LotShowingResponse()
            {
                ApptPhone = "8888888888",
            };

            var lotListing = new AborResponse.ListingRequest.LotRequest.LotListingRequestDetailResponse()
            {
                AddressInfo = new Mock<AborResponse.LotListing.LotAddressResponse>().Object,
                CommunityId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                CreatedBy = "CreatedBy",
                FeaturesInfo = new Mock<AborResponse.LotListing.LotFeaturesResponse>().Object,
                FinancialInfo = new Mock<AborResponse.LotListing.LotFinancialResponse>().Object,
                Id = Guid.NewGuid(),
                MlsNumber = isNewListing ? null : "mlsNumber",
                PropertyInfo = propertyInfo,
                ShowingInfo = showingInfo,
                SchoolsInfo = new Mock<AborResponse.LotListing.LotSchoolsResponse>().Object,
                StatusFieldsInfo = new Mock<AborResponse.ListingStatusFieldsResponse>().Object,
            };
            return lotListing;
        }

        private static AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse GetListingRequestDetailResponse(bool isNewListing)
        {
            var spacesDimensionsInfo = new Mock<AborResponse.SalePropertyDetail.SpacesDimensionsResponse>();
            var addressInfo = new Mock<AborResponse.SalePropertyDetail.SaleAddressResponse>();
            var propertyInfo = new AborResponse.SalePropertyDetail.PropertyInfoResponse()
            {
                ConstructionStage = ConstructionStage.Incomplete,
                UpdateGeocodes = true,
            };
            var featuresInfo = new Mock<AborResponse.SalePropertyDetail.FeaturesResponse>();
            var financialInfo = new AborResponse.SalePropertyDetail.FinancialResponse()
            {
                HasBonusWithAmount = true,
                BuyersAgentCommissionType = CommissionType.Amount,
                AgentBonusAmountType = CommissionType.Percent,
            };
            var schoolsInfo = new Mock<AborResponse.SchoolsResponse>();
            var showingInfo = new AborResponse.SalePropertyDetail.ShowingResponse()
            {
                RealtorContactEmail = new string[] { "RealtorContactEmail@tst.com" },
                OccupantPhone = "8888888888",
            };
            var salePropertyInfo = new AborResponse.SalePropertyDetail.SalePropertyResponse()
            {
                OwnerName = "OwnerName",
                PlanName = "PlanName",
                CompanyId = Guid.NewGuid(),
            };
            var roomInfo = new AborResponse.RoomResponse()
            {
                Id = Guid.NewGuid(),
                Level = RoomLevel.First,
                RoomType = Husa.Quicklister.Abor.Domain.Enums.RoomType.PrimaryBathroom,
                Features = new List<RoomFeatures>()
                {
                    RoomFeatures.DiningArea,
                    RoomFeatures.BreakfastBar,
                    RoomFeatures.CeilingFans,
                },
            };
            var saleProperty = new AborResponse.SalePropertyDetail.SalePropertyDetailResponse()
            {
                SpacesDimensionsInfo = spacesDimensionsInfo.Object,
                AddressInfo = addressInfo.Object,
                PropertyInfo = propertyInfo,
                FeaturesInfo = featuresInfo.Object,
                FinancialInfo = financialInfo,
                SchoolsInfo = schoolsInfo.Object,
                ShowingInfo = showingInfo,
                SalePropertyInfo = salePropertyInfo,
                Rooms = new List<AborResponse.RoomResponse>()
                {
                    roomInfo,
                },
            };

            var openHouses = new List<AborResponse.OpenHouseResponse>()
            {
              new AborResponse.OpenHouseResponse()
              {
                  StartTime = new TimeSpan(10, 0, 0),
                  EndTime = new TimeSpan(12, 0, 0),
                  Refreshments = new List<Refreshments>()
                  {
                         Refreshments.Beverages,
                  },
                  Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Wednesday,
              },
              new AborResponse.OpenHouseResponse()
              {
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Refreshments = new List<Refreshments>()
                    {
                             Refreshments.Appetizers,
                    },
                    Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Saturday,
              },
            };

            var statusFields = new Mock<AborResponse.ListingSaleStatusFieldsResponse>();

            var listingSale = new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse()
            {
                SaleProperty = saleProperty,
                ListPrice = 127738,
                StatusFieldsInfo = statusFields.Object,
                MlsNumber = isNewListing ? null : "mlsNumber",
            };
            listingSale.SaleProperty.OpenHouses = openHouses;

            return listingSale;
        }
    }
}
