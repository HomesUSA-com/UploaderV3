namespace Husa.Uploader.Core.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;
    using DfwResponse = Husa.Quicklister.Dfw.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class DfwUploadServiceTests : MarketUploadServiceTests<DfwUploadService, DfwListingRequest>
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<DfwUploadService>> logger = new();
        private readonly ApplicationServicesFixture fixture;

        public DfwUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
            this.uploaderClient.Setup(x => x.FillFieldSingleOption(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            this.uploaderClient.Setup(x => x.FindElements(It.IsAny<By>(), It.IsAny<bool>())).Returns(new ReadOnlyCollection<IWebElement>(Array.Empty<IWebElement>())).Verifiable();
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.LinkText("Manage Tours Links")),
                It.IsAny<CancellationToken>())).Returns(true);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
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

        [Theory]
        [InlineData(PropertySubType.Townhouse)]
        [InlineData(PropertySubType.SingleFamilyResidence)]
        public async Task UploadByHousingType_ReturnSuccess(PropertySubType propertySubType)
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest();
            request.HousingTypeDesc = propertySubType.ToStringFromEnumMember();
            request.ExpiredDate = DateTime.UtcNow;
            request.LegalSubdivision = "LegalSubdivision";
            request.TaxID = "200";

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(request);

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

        [Fact]
        public async Task UpdateStatus_PendingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "PND";
            dfwListing.PendingDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ClosedSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "CLOSD";
            dfwListing.PendingDate = DateTime.Now;
            dfwListing.ClosedDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ActiveContingentSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "AC";
            dfwListing.ContractDate = DateTime.Now;
            dfwListing.ContingencyInfo = "ContingencyInfo test";
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ActiveOptionContractSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "AOC";
            dfwListing.ContractDate = DateTime.Now;
            dfwListing.EstClosedDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ActiveKickOutSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "AKO";
            dfwListing.ContractDate = DateTime.Now;
            dfwListing.ContingencyInfo = "ContingencyInfo test";
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task FillCompletionDate_Fail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var request = this.GetEmptyListingRequest();
            request.MLSNum = "MLSNum";
            request.BuildCompletionDate = DateTime.Now;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateCompletionDate(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
            this.uploaderClient.Verify(client => client.WriteTextbox(By.Id("Input_301"), "12/31/2023", false, true, false, false), Times.Never);
            this.uploaderClient.Verify(client => client.WriteTextbox(By.Id("Input_249"), string.Empty, true, true, false, false), Times.Never);
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
                         Refreshments.Drinks,
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
                         Refreshments.Lunch,
                     }.ToStringFromEnumMembers(),
                },
            };

            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.OpenHouse = openHouses;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(dfwListing);
            this.uploaderClient.Verify(client => client.ScrollDown(It.IsAny<int>()), Times.AtLeastOnce);

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

        protected override DfwUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);

        protected override DfwListingRequest GetEmptyListingRequest()
            => new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());

        protected override ResidentialListingRequest GetResidentialListingRequest(bool isNewListing = true)
        {
            var listingSale = GetListingRequestDetailResponse(isNewListing);
            return new DfwListingRequest(listingSale).CreateFromApiResponseDetail();
        }

        private static DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse GetListingRequestDetailResponse(bool isNewListing)
        {
            var spacesDimensionsInfo = new Mock<DfwResponse.SalePropertyDetail.SpacesDimensionsResponse>();
            var addressInfo = new Mock<DfwResponse.SalePropertyDetail.AddressInfoResponse>();
            var propertyInfo = new DfwResponse.SalePropertyDetail.PropertyInfoResponse()
            {
                ConstructionStage = ConstructionStage.Incomplete,
                TaxId = "100",
                ConstructionCompletionDate = DateTime.UtcNow,
                UpdateGeocodes = true,
            };
            var featuresInfo = new DfwResponse.SalePropertyDetail.FeaturesResponse()
            {
                HasGarage = true,
            };
            var financialInfo = new DfwResponse.SalePropertyDetail.FinancialResponse()
            {
                HasBonusWithAmount = true,
                BuyersAgentCommissionType = CommissionType.Amount,
                AgentBonusAmountType = CommissionType.Percent,
            };
            var schoolsInfo = new Mock<DfwResponse.SchoolsResponse>();
            var showingInfo = new DfwResponse.SalePropertyDetail.ShowingResponse()
            {
                RealtorContactEmail = new string[] { "RealtorContactEmail@tst.com" },
                OccupantPhone = "8888888888",
            };
            var salePropertyInfo = new DfwResponse.SalePropertyDetail.SalePropertyResponse()
            {
                OwnerName = "OwnerName",
                PlanName = "PlanName",
                CompanyId = Guid.NewGuid(),
            };
            var roomInfo = new DfwResponse.RoomResponse()
            {
                Level = RoomLevel.One,
                RoomType = Husa.Quicklister.Dfw.Domain.Enums.RoomType.OfficeRoom,
            };
            var saleProperty = new DfwResponse.SalePropertyDetail.SalePropertyDetailResponse()
            {
                SpacesDimensionsInfo = spacesDimensionsInfo.Object,
                AddressInfo = addressInfo.Object,
                PropertyInfo = propertyInfo,
                FeaturesInfo = featuresInfo,
                FinancialInfo = financialInfo,
                SchoolsInfo = schoolsInfo.Object,
                ShowingInfo = showingInfo,
                SalePropertyInfo = salePropertyInfo,
                Rooms = new List<DfwResponse.RoomResponse>()
                {
                    roomInfo,
                },
            };

            var openHouses = new List<DfwResponse.OpenHouseResponse>()
            {
              new DfwResponse.OpenHouseResponse()
              {
                  StartTime = new TimeSpan(10, 0, 0),
                  EndTime = new TimeSpan(12, 0, 0),
                  Refreshments = new List<Refreshments>()
                  {
                         Refreshments.Drinks,
                  },
                  Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Wednesday,
              },
              new DfwResponse.OpenHouseResponse()
              {
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Refreshments = new List<Refreshments>()
                    {
                             Refreshments.Snacks,
                    },
                    Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Saturday,
              },
            };

            var statusFields = new Mock<DfwResponse.SaleListingStatusFieldsResponse>();

            var listingSale = new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse()
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
