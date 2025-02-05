namespace Husa.Uploader.Core.Tests
{
    using System.Collections.ObjectModel;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.Amarillo.Domain.Enums;
    using Husa.Quicklister.Amarillo.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
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
    using AmarilloResponse = Husa.Quicklister.Amarillo.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class AmarilloUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<IMediaRepository> mediaRepository = new();
        private readonly Mock<IListingRequestRepository> sqlDataLoader = new();
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();
        private readonly Mock<ILogger<AmarilloUploadService>> logger = new();
        private readonly Mock<Models.UploadCommandInfo> uploadCommandInfo = new();
        private readonly ApplicationServicesFixture fixture;

        public AmarilloUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
        }

        [Fact]
        public async Task UploadNewListingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpVirtualTours();
            var company = new Mock<CompanyDetail>().Object;
            company.Market = MarketCode.Amarillo;
            var listingSale = GetListingRequestDetailResponse();
            var amarilloListing = new AmarilloListingRequest(listingSale).CreateFromApiResponseDetail(company);
            this.SetUpUploaderClient();
            this.uploaderClient.Setup(x => x.ExecuteScript(It.Is<string>(y => y == "return jQuery('a[id^=remove_button_]').length;"), It.IsAny<bool>())).Returns("0");

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadExistingListingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpVirtualTours();
            var company = new Mock<CompanyDetail>().Object;
            company.Market = MarketCode.Amarillo;
            var listingSale = GetListingRequestDetailResponse();
            var amarilloListing = new AmarilloListingRequest(listingSale).CreateFromApiResponseDetail(company);
            amarilloListing.MLSNum = "123-4567";
            this.SetUpUploaderClient();
            this.uploaderClient.Setup(x => x.ExecuteScript(It.Is<string>(y => y == "return jQuery('a[id^=remove_button_]').length;"), It.IsAny<bool>())).Returns("0");

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task PartialUploadWithExistingListing()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
            };
            this.SetUpUploaderClient();

            // Act
            var sut = this.GetSut();
            var result = await sut.PartialUpload(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadFails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act && Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Upload(null));
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
        public Task LogoutSuccess()
        {
            this.SetUpUploaderClient();
            var sut = this.GetSut();
            var result = sut.Logout();

            Assert.Equal(UploadResult.Success, result);
            return Task.CompletedTask;
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
            this.SetUpUploaderClient();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTourNoVirtualToursSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
            };
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), MarketCode.Amarillo, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ResidentialListingVirtualTour>())
            .Verifiable();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTourFails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UploadVirtualTour(null));
        }

        [Fact]
        public async Task UpdateCompletionDateSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
                BuildCompletionDate = DateTime.UtcNow,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateCompletionDate(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateCompletionDateFails()
        {
            this.SetUpUploaderClient();

            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdateCompletionDate(null));
        }

        [Theory]
        [InlineData("Cancelled")] // UpdateStatus_Cancelled
        [InlineData("Closed")] // UpdateStatus_Closed
        [InlineData("Pending")] // UpdateStatus_Pending
        [InlineData("Under Contract W/Contingency")] // UpdateStatus_Under_Contract_W_Contingency
        [InlineData("Withdrawn")] // UpdateStatus_Withdrawn
        public async Task UpdateStatus_Success(string status)
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();

            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
                ListStatus = status,
            };

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateStatus(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_Failed()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();

            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
                ListStatus = "Anything",
            };

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateStatus(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Failure, result);
        }

        [Fact]
        public async Task UpdatePriceSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
                ListPrice = 100,
            };
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdatePrice(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateImagesSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
            };
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.ClassName("fileupload-progress")),
                It.IsAny<CancellationToken>())).Returns(true);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateImages(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task EditExistingListinListingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
            };
            this.SetUpUploaderClient();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.Edit(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task EditNewListinListingSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse());
            this.SetUpUploaderClient();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);

            // Act
            var sut = this.GetSut();
            var result = await sut.Edit(amarilloListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateOpenHouseSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            this.SetUpUploaderClient();
            this.uploaderClient.Setup(x => x.ExecuteScript(It.Is<string>(y => y == "return $('.open-house-checkbox .openHouseCheckbox').length;"), It.IsAny<bool>())).Returns("0");
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
            var amarilloListing = new AmarilloListingRequest(new SaleListingRequestDetailResponse())
            {
                MLSNum = "123-4567",
                OpenHouse = openHouses,
            };

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateOpenHouse(amarilloListing);
            Assert.Equal(UploadResult.Success, result);
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

        private static SaleListingRequestDetailResponse GetListingRequestDetailResponse()
        {
            var spacesDimensionsInfo = new Mock<SpacesDimensionsResponse>();
            var addressInfo = new AmarilloResponse.SalePropertyDetail.AddressInfoResponse()
            {
                City = Quicklister.Amarillo.Domain.Enums.Domain.Cities.Amarillo,
                County = Quicklister.Amarillo.Domain.Enums.Domain.Counties.Randall,
                State = States.Texas,
            };
            var propertyInfo = new PropertyInfoResponse()
            {
                ConstructionStage = Quicklister.Amarillo.Domain.Enums.Domain.ConstructionStage.Incomplete,
                ConstructionCompletionDate = DateTime.Now,
            };
            var featuresInfo = new Mock<FeaturesResponse>();
            var financialInfo = new FinancialResponse()
            {
                BuyersAgentCommission = 10,
                BuyersAgentCommissionType = Quicklister.Extensions.Domain.Enums.CommissionType.Amount,
            };
            var schoolsInfo = new SchoolsResponse()
            {
                ElementarySchool = Quicklister.Amarillo.Domain.Enums.Domain.ElementarySchool.Gateway,
                HighSchool = Quicklister.Amarillo.Domain.Enums.Domain.HighSchool.Amarillo,
                IntermediateSchool = Quicklister.Amarillo.Domain.Enums.Domain.IntermediateSchool.Other,
                SchoolDistrict = Quicklister.Amarillo.Domain.Enums.Domain.SchoolDistrict.Amarillo,
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
                Level = Quicklister.Amarillo.Domain.Enums.Domain.RoomLevel.First,
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

            var statusFields = new Mock<SaleListingStatusFieldsResponse>();

            var listingSale = new SaleListingRequestDetailResponse()
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

        private void SetUpUploaderClient()
        {
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).SendKeys(It.IsAny<string>()));
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);
            this.uploaderClient.Setup(x => x.WaitUntilElementDisappears(
                It.Is<By>(x => x == By.ClassName("fileupload-progress")),
                It.IsAny<CancellationToken>())).Returns(true);
            this.uploaderClient.Setup(x => x.SwitchTo().Frame(It.IsAny<int>())).Returns(new Mock<IWebDriver>().Object);
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), false).ToString());
            this.uploaderClient.Setup(x => x.ExecuteScript(It.Is<string>(y => y == "return jQuery('a[id^=remove_button_]').length;"), It.IsAny<bool>())).Returns("0");
            this.uploaderClient.Setup(x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()));
            this.uploaderClient.Setup(x => x.FindElements(It.IsAny<By>(), It.IsAny<bool>())).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { new Mock<IWebElement>().Object })).Verifiable();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).Click());
            this.uploaderClient.Setup(x => x.ScrollToTop());
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

        private void SetUpCompany(string username = "amt.homesusa", string password = "E$Qz72^dpKC5@H4#^7vm78")
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
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), MarketCode.Amarillo, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[] { GetResidentialListingVirtualTour(), GetResidentialListingVirtualTour() })
            .Verifiable();
        }

        private AmarilloUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);
    }
}
