namespace Husa.Uploader.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Sabor.Api.Contracts.Response;
    using Husa.Quicklister.Sabor.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Sabor.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.Sabor.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;
    using AddressInfoResponse = Husa.Quicklister.Sabor.Api.Contracts.Response.SalePropertyDetail.AddressInfoResponse;

    [Collection(nameof(ApplicationServicesFixture))]
    public class SaborUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<IMediaRepository> mediaRepository = new();
        private readonly Mock<IListingRequestRepository> sqlDataLoader = new();
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();
        private readonly Mock<ILogger<SaborUploadService>> logger = new();
        private readonly Mock<Models.UploadCommandInfo> uploadCommandInfo = new();
        private readonly ApplicationServicesFixture fixture;

        public SaborUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678")]
        public async Task UploadSuccess(string mlsNumber)
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpVirtualTours();
            var company = new Mock<CompanyDetail>().Object;
            var listingSale = GetListingRequestDetailResponse();
            var saborListing = new SaborListingRequest(listingSale).CreateFromApiResponseDetail(company);
            saborListing.MLSNum = mlsNumber;
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).SendKeys(It.IsAny<string>()));
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.ClassName("fileupload-progress")),
                It.IsAny<CancellationToken>())).Returns(true);

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task PartialUploadWithExistingListing()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var company = new Mock<CompanyDetail>().Object;
            var listingSale = GetListingRequestDetailResponse();
            var saborListing = new SaborListingRequest(listingSale).CreateFromApiResponseDetail(company);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).SendKeys(It.IsAny<string>()));
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.ClassName("fileupload-progress")),
                It.IsAny<CancellationToken>())).Returns(true);

            // Act
            var sut = this.GetSut();
            var result = await sut.PartialUpload(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadFails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act && Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Upload((SaborListingRequest)null));
        }

        [Fact]
        public async Task LoginWithNoCredentialsUseDefaultSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany(null, null);

            // Act
            var sut = this.GetSut();
            var result = await sut.Login(Guid.NewGuid());

            // Assert
            Assert.Equal(LoginResult.Logged, result);
        }

        [Fact]
        public async Task LoginWithCredentialsSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();

            // Act
            var sut = this.GetSut();
            var result = await sut.Login(Guid.NewGuid());

            // Assert
            Assert.Equal(LoginResult.Logged, result);
        }

        [Fact]
        public async Task LoginInOtherElement()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.uploaderClient.Setup(x => x.WaitUntilElementIsDisplayed(
                It.Is<By>(i => i == By.Name("go")), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();

            // Act
            var sut = this.GetSut();
            var result = await sut.Login(Guid.NewGuid());

            // Assert
            Assert.Equal(LoginResult.Logged, result);
        }

        [Fact]
        public async Task UploadVirtualTourSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpVirtualTours();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse());
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTourNoVirtualToursSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse());
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ResidentialListingVirtualTour>())
            .Verifiable();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTourFails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UploadVirtualTour((SaborListingRequest)null));
        }

        [Fact]
        public async Task UpdateCompletionDateSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse())
            {
                MLSNum = "MLSNum",
                BuildCompletionDate = DateTime.UtcNow,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateCompletionDate(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateCompletionDateFails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdateCompletionDate((SaborListingRequest)null));
        }

        [Fact]
        public async Task UpdateStatusSoldSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse())
            {
                ListStatus = "SLD",
                ClosedDate = DateTime.UtcNow,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateStatus(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdatePriceSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse())
            {
                ListPrice = 100,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdatePrice(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateImagesSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse());
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.ClassName("fileupload-progress")),
                It.IsAny<CancellationToken>())).Returns(true);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateImages(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task EditListingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse());
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.Edit(saborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateOpenHouseSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();

            DateTime startDateMonday = AdjustStartDate(DayOfWeek.Monday, new TimeSpan(14, 0, 0));
            DateTime startDateThursday = AdjustStartDate(DayOfWeek.Thursday, new TimeSpan(10, 0, 0));

            var openHouses = new List<OpenHouseRequest>()
            {
                new OpenHouseRequest()
                {
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Date = OpenHouseExtensions.GetNextWeekday(startDateMonday, DayOfWeek.Monday),
                    Active = true,
                    Refreshments = "Y",
                    Lunch = "Y",
                },
                new OpenHouseRequest()
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    Date = OpenHouseExtensions.GetNextWeekday(startDateThursday, DayOfWeek.Thursday),
                    Active = true,
                    Refreshments = "Y",
                    Lunch = "N",
                },
            };
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse())
            {
                MLSNum = "mlsNum",
                OpenHouse = openHouses,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateOpenHouse(saborListing);
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public void FillOpenHouseInfo_CreatesOpenHouseEntries()
        {
            var expectedCount = 2;
            var saborListing = new SaborListingRequest(new ListingSaleRequestDetailResponse())
            {
                MLSNum = "mlsNum",
            };
            var openHouseResponses = new List<OpenHouseResponse>
            {
                new OpenHouseResponse
                {
                    Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Monday,
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Lunch = true,
                    Refreshments = false,
                },
                new OpenHouseResponse
                {
                    Type = Quicklister.Extensions.Domain.Enums.OpenHouseType.Wednesday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    Lunch = false,
                    Refreshments = true,
                },
            };

            saborListing.FillOpenHouseInfo(openHouseResponses, saborListing.OpenHouse);

            Assert.Equal(expectedCount, saborListing.OpenHouse.Count);
        }

        [Fact]
        public void TestGetOpenHouseHours()
        {
            var result = OpenHouseExtensions.GetOpenHouseHours("10:30");

            Assert.Equal("10", result);
        }

        [Fact]
        public async Task UpdateLotPrice_NotImplementedException_Fails()
        {
            // Arrange
            var lotListingRequest = new Mock<LotListingRequest>();

            // Act && Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await this.GetSut().UpdateLotPrice(lotListingRequest.Object));
        }

        private static ResidentialListingVirtualTour GetResidentialListingVirtualTour()
        {
            var id = Guid.NewGuid();
            return new()
            {
                Id = id,
                MediaUri = new Uri("https://test.org/" + id.ToString()),
            };
        }

        private static DateTime AdjustStartDate(DayOfWeek day, TimeSpan startTime)
        {
            DateTime currentDateTime = DateTime.Now;
            var openHouseDate = OpenHouseExtensions.GetNextWeekday(DateTime.Today, day);
            if (currentDateTime.Date.ToString("MM/dd/yyyy") == openHouseDate && TimeSpan.FromHours(currentDateTime.Hour) > startTime)
            {
                return DateTime.Today.AddDays(1);
            }
            else
            {
                return DateTime.Today;
            }
        }

        private static ListingSaleRequestDetailResponse GetListingRequestDetailResponse()
        {
            var spacesDimensionsInfo = new Mock<SpacesDimensionsResponse>();
            var addressInfo = new AddressInfoResponse()
            {
                City = Quicklister.Sabor.Domain.Enums.Domain.Cities.Abilene,
                County = Quicklister.Sabor.Domain.Enums.Domain.Counties.Atascosa,
                State = States.Texas,
            };
            var propertyInfo = new PropertyInfoResponse()
            {
                ConstructionStage = Quicklister.Sabor.Domain.Enums.ConstructionStage.Incomplete,
                ConstructionCompletionDate = DateTime.Now,
            };
            var featuresInfo = new Mock<FeaturesResponse>();
            var financialInfo = new FinancialResponse()
            {
                TaxRate = 1000,
                TaxYear = 2018,
                BuyersAgentCommission = 10,
                BuyersAgentCommissionType = Quicklister.Extensions.Domain.Enums.CommissionType.Amount,
            };
            var schoolsInfo = new SchoolsResponse()
            {
                ElementarySchool = Quicklister.Sabor.Domain.Enums.Domain.ElementarySchool.Adams,
                HighSchool = Quicklister.Sabor.Domain.Enums.Domain.HighSchool.Johnson,
                MiddleSchool = Quicklister.Sabor.Domain.Enums.Domain.MiddleSchool.CalallenMiddleSchool,
                SchoolDistrict = Quicklister.Sabor.Domain.Enums.Domain.SchoolDistrict.AlamoHeightsISD,
            };
            var showingInfo = new Mock<ShowingResponse>();
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
                Level = Quicklister.Sabor.Domain.Enums.Domain.RoomLevel.MainLevel,
                RoomType = RoomType.Office,
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
                ShowingInfo = showingInfo.Object,
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
            };

            return listingSale;
        }

        private void SetUpCredentials()
        {
            this.serviceSubscriptionClient
                .Setup(x => x.Corporation.GetMarketReverseProspectInformation(It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReverseProspectInfoResponse()
                {
                    UserName = "UserName",
                    Password = "password",
                })
            .Verifiable();
        }

        private void SetUpCompany(string username = "username", string password = "password")
        {
            var company = new CompanyDetail()
            {
                BrokerInfo = new BrokerInfoResponse()
                {
                    SiteUsername = username,
                    SitePassword = password,
                },
            };
            this.serviceSubscriptionClient
                .Setup(x => x.Company.GetCompany(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
        }

        private void SetUpVirtualTours()
        {
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[] { GetResidentialListingVirtualTour(), GetResidentialListingVirtualTour() })
            .Verifiable();
        }

        private SaborUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);
    }
}
