namespace Husa.Uploader.Core.Tests
{
    using System.Collections.ObjectModel;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.CTX.Domain.Enums;
    using Husa.Quicklister.CTX.Domain.Enums.Entities;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;
    using AddressInfoResponse = Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail.AddressInfoResponse;
    using UploadResult = Husa.Uploader.Crosscutting.Enums.UploadResult;

    [Collection(nameof(ApplicationServicesFixture))]
    public class CtxUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<IWebDriver> webDriver = new();
        private readonly Mock<IWebElement> webElement = new();
        private readonly Mock<IMediaRepository> mediaRepository = new();
        private readonly Mock<IListingRequestRepository> sqlDataLoader = new();
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();
        private readonly Mock<ILogger<CtxUploadService>> logger = new();
        private readonly Mock<Models.UploadCommandInfo> uploadCommandInfo = new();
        private readonly ApplicationServicesFixture fixture;

        public CtxUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
            this.webDriver.SetupAllProperties();
            this.webElement.SetupAllProperties();
        }

        [Fact]
        public async Task Upload_ReturnSuccess()
        {
            // Arrange
            var listingSale = GetListingRequestDetailResponse();
            var ctxListing = new CtxListingRequest(listingSale).CreateFromApiResponseDetail();

            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ctxListing);
            this.SetUpConfigs(setUpAdditionalUploaderConfig: true);

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task Upload_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Upload((CtxListingRequest)null));
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccess()
        {
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse());
            this.SetUpConfigs(ctxListing);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccessWithoutVirtualTours()
        {
            // Arrange
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ResidentialListingVirtualTour>())
            .Verifiable();
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse());
            this.SetUpConfigs(ctxListing, setUpVirtualTours: false);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UploadVirtualTour((CtxListingRequest)null));
        }

        [Fact]
        public async Task UpdateCompletionDate_Success()
        {
            // Arrange
            var ctxListing = new CtxListingRequest(new ListingSaleRequestDetailResponse())
            {
                MLSNum = "MLSNum",
            };
            this.SetUpConfigs(ctxListing, setUpVirtualTours: false);

            // Act
            var sut = this.GetSut();
            var result = await sut.UpdateCompletionDate(ctxListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
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

        [Fact]
        public async Task LoginWithNoCredentials_UseDefaultSuccess()
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

#nullable enable
        internal void SetUpConfigs(CtxListingRequest? ctxListing = null, bool setUpVirtualTours = true, bool setUpAdditionalUploaderConfig = false)
        {
            this.SetUpCredentials();
            this.SetUpCompany();
            if (setUpVirtualTours)
            {
                this.SetUpVirtualTours();
            }

            if (setUpAdditionalUploaderConfig)
            {
                this.SetUpAdditionalUploaderConfig();
            }

            if (ctxListing is null)
            {
                return;
            }

            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ctxListing);
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

        private static ListingSaleRequestDetailResponse GetListingRequestDetailResponse()
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
            };

            return listingSale;
        }

        private void SetUpAdditionalUploaderConfig()
        {
            var windowHandle = "current";
            var windowHandles = new ReadOnlyCollection<string>(new List<string>() { windowHandle });
            var findElementsValues = new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { this.webElement.Object });
            this.uploaderClient.SetupGet(x => x.WindowHandles).Returns(windowHandles);
            this.uploaderClient.SetupGet(x => x.CurrentWindowHandle).Returns(windowHandle);
            this.uploaderClient.Setup(x => x.FindElements(It.IsAny<By>(), false)).Returns(findElementsValues);
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), false, false).SendKeys(It.IsAny<string>()));
            this.uploaderClient.Setup(x => x.SwitchTo().Window(It.IsAny<string>())).Returns(this.webDriver.Object);
            this.uploaderClient.SetupGet(x => x.UploadInformation).Returns(this.uploadCommandInfo.Object);
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
                .Setup(x => x.Company.GetCompany(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
        }

        private void SetUpVirtualTours()
        {
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[] { GetResidentialListingVirtualTour(), GetResidentialListingVirtualTour() })
            .Verifiable();
        }

        private CtxUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.sqlDataLoader.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);
    }
}
