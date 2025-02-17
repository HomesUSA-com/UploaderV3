namespace Husa.Uploader.Core.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.MediaService.Domain.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;
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
            var uploadInformationMock = new Mock<Husa.Uploader.Core.Models.UploadCommandInfo>();
            uploadInformationMock.Object.IsNewListing = true;
            this.uploaderClient.Setup(x => x.UploadInformation).Returns(uploadInformationMock.Object);
            this.uploaderClient.Setup(x => x.WaitUntilElementIsDisplayed(
                It.Is<By>(x => x == By.ClassName("stickypush")),
                It.IsAny<CancellationToken>())).Returns(true);
            var deletedElements = new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { new Mock<IWebElement>().Object });
            this.uploaderClient.Setup(x => x.FindElements(By.LinkText("Delete"), false)).Returns(deletedElements);
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

        [Theory]
        [InlineData(HousingType.CondoTownhome)]
        [InlineData(HousingType.AttachedDuplex)]
        [InlineData(HousingType.GardenZeroLotLine)]
        [InlineData(HousingType.SingleDetached)]
        public async Task UploadByHousingType_ReturnSuccess(HousingType housingType)
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest();
            request.HousingTypeDesc = housingType.ToStringFromEnumMember();
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
        [InlineData("SLD")] // UpdateStatus_ClosedSuccess
        [InlineData("PND")] // UpdateStatus_PendingSuccess
        [InlineData("ACT")] // UpdateStatus_ActiveOptionContractSuccess
        [InlineData("CSN")] // UpdateStatus_CommingSoonSuccess
        public async Task UploadListStatus_ReturnSuccess(string listStatus)
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(true);
            request.ListStatus = listStatus;
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
        [InlineData("RESI", true)] // Residential. Single Family. New Listing
        [InlineData("RESI", false)] // Residential. Single Family
        [InlineData("RINC", true)] // Residential Income (Multi-Family). New Listing
        [InlineData("RINC", false)] // Residential Income (Multi-Family)
        public async Task UploadNewListingSingleAndMultiFamily_ReturnSuccess(string propType, bool isNewListing)
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest(isNewListing);
            request.ListStatus = MarketStatuses.Sold.ToStringFromEnumMember();
            request.ExpiredDate = DateTime.UtcNow;
            request.LegalSubdivision = "LegalSubdivision";
            request.TaxID = "200";
            request.PropType = propType;
            request.StreetType = "ALY";
            request.Directions = "Test Direction";
            request.BuyerIncentive = "Test BuyerIncentive";
            request.IsForLease = "Yes";

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Theory]
        [InlineData("SLD")] // UpdateStatus_ClosedSuccess
        [InlineData("PND")] // UpdateStatus_PendingSuccess
        [InlineData("ACT")] // UpdateStatus_ActiveOptionContractSuccess
        [InlineData("AKO")] // UpdateStatus_ActiveKickOut
        [InlineData("AOC")] // UpdateStatus_ActiveOptionContractSuccess
        [InlineData("CAN")] // UpdateStatus_CanSuccess
        [InlineData("HOLD")] // UpdateStatus_OffMarketDateSuccess
        public async Task UpdateStatus_Success(string status)
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = status;
            dfwListing.PendingDate = DateTime.Now;
            dfwListing.EstClosedDate = DateTime.Now;
            dfwListing.ContractDate = DateTime.Now;
            dfwListing.ClosedDate = DateTime.Now;
            dfwListing.ExpiredDate = DateTime.Now;
            dfwListing.OffMarketDate = DateTime.Now;
            dfwListing.ContingencyInfo = "This is a test for the contengency info field";
            dfwListing.HasContingencyInfo = false;
            dfwListing.SqFtTotal = 10;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_InvalidStatusFail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = "TEST";
            dfwListing.PendingDate = DateTime.Now;
            dfwListing.EstClosedDate = DateTime.Now;
            dfwListing.ExpiredDate = DateTime.Now;
            dfwListing.HasContingencyInfo = false;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Failure, result);
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
        public async Task UpdateStatus_CancelledSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.ListStatus = MarketStatuses.Cancelled.ToStringFromEnumMember();
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task AddOpenHouseNoOpenHousesSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var openHouses = new List<OpenHouseRequest>();

            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse());
            dfwListing.OpenHouse = openHouses;
            dfwListing.ListStatus = MarketStatuses.Active.ToStringFromEnumMember();
            dfwListing.EnableOpenHouse = true;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task AddOpenActiveHouseSuccess()
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
            dfwListing.ListStatus = MarketStatuses.Active.ToStringFromEnumMember();
            dfwListing.EnableOpenHouse = true;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(dfwListing);
            this.uploaderClient.Verify(client => client.ScrollDown(It.IsAny<int>()), Times.AtLeastOnce);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AddOpenPendingShowingHouseSuccess(bool allowPendingList)
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
            dfwListing.ListStatus = MarketStatuses.Pending.ToStringFromEnumMember();
            dfwListing.AllowPendingList = allowPendingList;
            dfwListing.EnableOpenHouse = true;
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(dfwListing);
            this.uploaderClient.Verify(client => client.ScrollDown(It.IsAny<int>()), Times.AtLeastOnce);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task AddOpenHouseNoListing_Fail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var sut = this.GetSut();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.UpdateOpenHouse(null));
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
                    Caption = "test.jpeg",
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
        public async Task VirtualTour_Success()
        {
            // Arrange
            var dfwListing = new DfwListingRequest(new DfwResponse.ListingRequest.SaleRequest.SaleListingRequestDetailResponse())
            {
                ListStatus = MarketStatuses.Sold.ToStringFromEnumMember(),
                BackOnMarketDate = DateTime.Now,
                OffMarketDate = DateTime.Now,
            };
            this.SetUpConfigs(dfwListing, setUpVirtualTours: true);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(dfwListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task VirtualTourNoListing_Fail()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var sut = this.GetSut();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.UploadVirtualTour(null));
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
            var company = new Mock<CompanyDetail>().Object;
            var result = new DfwListingRequest(listingSale).CreateFromApiResponseDetail(company);
            result.BrokerName = "Ben Caballero";
            result.SellingAgentSupervisor = "Ben Caballero";
            result.PropType = ListPropertyType.Residencial.ToStringFromEnumMember();
            return result;
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
            var roomInfo1 = new DfwResponse.RoomResponse()
            {
                Level = RoomLevel.One,
                RoomType = Husa.Quicklister.Dfw.Domain.Enums.RoomType.OfficeRoom,
            };
            var roomInfo2 = new DfwResponse.RoomResponse()
            {
                Level = RoomLevel.Two,
                RoomType = Husa.Quicklister.Dfw.Domain.Enums.RoomType.KitchenRoom,
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
                    roomInfo1,
                    roomInfo2,
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
