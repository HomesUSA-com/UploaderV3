namespace Husa.Uploader.Core.Tests
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums.Domain;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;
    using HarResponse = Husa.Quicklister.Har.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class HarUploadServiceTests : MarketUploadServiceTests<HarUploadService, HarListingRequest>
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<HarUploadService>> logger = new();
        private readonly ApplicationServicesFixture fixture;

        public HarUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
            this.uploaderClient.Setup(x => x.FillFieldSingleOption(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            this.uploaderClient.Setup(x => x.FindElements(It.IsAny<By>(), It.IsAny<bool>())).Returns(new ReadOnlyCollection<IWebElement>(Array.Empty<IWebElement>())).Verifiable();
        }

        [Theory]
        [InlineData(HousingType.MultiFamily)]
        [InlineData(HousingType.SingleFamily)]
        [InlineData(HousingType.TownhouseCondo)]
        [InlineData(HousingType.CountryHomesAcreage)]
        public async Task UploadByHousingType_ReturnSuccess(HousingType housingType)
        {
            // Arrange
            this.SetUpConfigs();

            var request = this.GetResidentialListingRequest();
            request.HousingTypeDesc = housingType.ToStringFromEnumMember();
            request.ExpiredDate = DateTime.UtcNow;
            request.LegalSubdivision = "LegalSubdivision";
            request.TaxID = "200";
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(request);

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_HoldSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var harListing = new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            harListing.ListStatus = "Hold";
            harListing.BackOnMarketDate = DateTime.Now;
            harListing.OffMarketDate = DateTime.Now;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(harListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_PendingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var harListing = new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            harListing.ListStatus = "Pending";
            harListing.PendingDate = DateTime.Now;
            harListing.EstClosedDate = DateTime.Now;
            harListing.ExpiredDate = DateTime.Now;
            harListing.HasContingencyInfo = false;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(harListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ClosedSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var harListing = new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            harListing.ListStatus = "Closed";
            harListing.PendingDate = DateTime.Now;
            harListing.ClosedDate = DateTime.Now;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(harListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_ActiveUnderContractSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var harListing = new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            harListing.ListStatus = "ActiveUnderContract";
            harListing.PendingDate = DateTime.Now;
            harListing.ClosedDate = DateTime.Now;
            harListing.EstClosedDate = DateTime.Now;
            harListing.HasContingencyInfo = false;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(harListing);

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
                         Refreshments.Dinner,
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

            var harListing = new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            harListing.OpenHouse = openHouses;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(harListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateOpenHouse(harListing);

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

        protected override HarUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.sqlDataLoader.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);

        protected override HarListingRequest GetEmptyListingRequest()
            => new HarListingRequest(new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());

        protected override ResidentialListingRequest GetResidentialListingRequest()
        {
            var listingSale = GetListingRequestDetailResponse();
            return new HarListingRequest(listingSale).CreateFromApiResponseDetail();
        }

        private static HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse GetListingRequestDetailResponse()
        {
            var spacesDimensionsInfo = new Mock<HarResponse.SalePropertyDetail.SpacesDimensionsResponse>();
            var addressInfo = new Mock<HarResponse.SalePropertyDetail.AddressInfoResponse>();
            var propertyInfo = new HarResponse.SalePropertyDetail.PropertyInfoResponse()
            {
                ConstructionStage = ConstructionStage.Incomplete,
                IsPlannedCommunity = true,
                PlannedCommunity = PlannedCommunity.Tavola,
                LegalSubdivision = LegalSubdivision.AlcornBend,
                TaxId = "100",
                ConstructionCompletionDate = DateTime.UtcNow,
            };
            var featuresInfo = new HarResponse.SalePropertyDetail.FeaturesResponse()
            {
                GolfCourseName = GolfCourseName.AprilSoundCountryClub,
            };
            var financialInfo = new HarResponse.SalePropertyDetail.FinancialResponse()
            {
                HasBonusWithAmount = true,
                BuyersAgentCommissionType = CommissionType.Amount,
                AgentBonusAmountType = CommissionType.Percent,
            };
            var schoolsInfo = new Mock<HarResponse.SchoolsResponse>();
            var showingInfo = new HarResponse.SalePropertyDetail.ShowingResponse()
            {
                RealtorContactEmail = new string[] { "RealtorContactEmail@tst.com" },
                OccupantPhone = "8888888888",
            };
            var salePropertyInfo = new HarResponse.SalePropertyDetail.SalePropertyResponse()
            {
                OwnerName = "OwnerName",
                PlanName = "PlanName",
                CompanyId = Guid.NewGuid(),
            };
            var roomInfo = new HarResponse.RoomResponse()
            {
                Id = Guid.NewGuid(),
                Level = RoomLevel.First,
                RoomType = Husa.Quicklister.Har.Domain.Enums.RoomType.ExtraRoom,
            };
            var saleProperty = new HarResponse.SalePropertyDetail.SalePropertyDetailResponse()
            {
                SpacesDimensionsInfo = spacesDimensionsInfo.Object,
                AddressInfo = addressInfo.Object,
                PropertyInfo = propertyInfo,
                FeaturesInfo = featuresInfo,
                FinancialInfo = financialInfo,
                SchoolsInfo = schoolsInfo.Object,
                ShowingInfo = showingInfo,
                SalePropertyInfo = salePropertyInfo,
                Rooms = new List<HarResponse.RoomResponse>()
                {
                    roomInfo,
                },
            };

            var openHouses = new List<HarResponse.OpenHouseResponse>()
            {
              new HarResponse.OpenHouseResponse()
              {
                  StartTime = new TimeSpan(10, 0, 0),
                  EndTime = new TimeSpan(12, 0, 0),
                  Refreshments = new List<Refreshments>()
                  {
                         Refreshments.Dinner,
                  },
                  Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Wednesday,
              },
              new HarResponse.OpenHouseResponse()
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

            var statusFields = new Mock<HarResponse.ListingSaleStatusFieldsResponse>();

            var listingSale = new HarResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse()
            {
                SaleProperty = saleProperty,
                ListPrice = 127738,
                StatusFieldsInfo = statusFields.Object,
            };
            listingSale.SaleProperty.OpenHouses = openHouses;

            return listingSale;
        }
    }
}
