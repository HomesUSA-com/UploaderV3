namespace Husa.Uploader.Core.Tests
{
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AborResponse = Husa.Quicklister.Abor.Api.Contracts.Response;

    [Collection(nameof(ApplicationServicesFixture))]
    public class AborUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<IMediaRepository> mediaRepository = new();
        private readonly Mock<IListingRequestRepository> sqlDataLoader = new();
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();
        private readonly Mock<ILogger<AborUploadService>> logger = new();
        private readonly ApplicationServicesFixture fixture;

        public AborUploadServiceTests(ApplicationServicesFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.uploaderClient.SetupAllProperties();
        }

        [Fact]
        public async Task Upload_ReturnSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpVirtualTours();

            var listingSale = GetListingRequestDetailResponse();
            var aborListing = new AborListingRequest(listingSale).CreateFromApiResponseDetail();

            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborListing);

            // Act
            var sut = this.GetSut();
            var result = await sut.Upload(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task Upload_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Upload((AborListingRequest)null));
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccess()
        {
            this.SetUpCredentials();
            this.SetUpVirtualTours();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccessWithoutVirtualTours()
        {
            // Arrange
            this.SetUpCredentials();
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[0])
            .Verifiable();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            var sut = this.GetSut();

            // Act
            var result = await sut.UploadVirtualTour(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UploadVirtualTour((AborListingRequest)null));
        }

        [Fact]
        public async Task UpdateCompletionDate_Success()
        {
            // Arrange
            this.SetUpCredentials();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.MLSNum = "MLSNum";
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateCompletionDate(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UpdateStatus_HoldSuccess()
        {
            // Arrange
            this.SetUpCredentials();
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Hold";
            aborListing.BackOnMarketDate = DateTime.Now;
            aborListing.OffMarketDate = DateTime.Now;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborListing);
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
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Pending";
            aborListing.PendingDate = DateTime.Now;
            aborListing.EstClosedDate = DateTime.Now;
            aborListing.ExpiredDate = DateTime.Now;
            aborListing.HasContingencyInfo = false;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborListing);
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
            var aborListing = new AborListingRequest(new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse());
            aborListing.ListStatus = "Closed";
            aborListing.PendingDate = DateTime.Now;
            aborListing.ClosedDate = DateTime.Now;
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aborListing);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateStatus(aborListing);

            // Assert
            Assert.Equal(UploadResult.Success, result);
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

        private static AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse GetListingRequestDetailResponse()
        {
            var spacesDimensionsInfo = new Mock<AborResponse.SalePropertyDetail.SpacesDimensionsResponse>();
            var addressInfo = new Mock<AborResponse.SalePropertyDetail.AddressInfoResponse>();
            var propertyInfo = new AborResponse.SalePropertyDetail.PropertyInfoResponse()
            {
                ConstructionStage = ConstructionStage.Incomplete,
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
                RealtorContactEmail = "RealtorContactEmail@tst.com",
                OccupantPhone = "8888888888",
            };
            var salePropertyInfo = new AborResponse.SalePropertyDetail.SalePropertyResponse()
            {
                OwnerName = "OwnerName",
                PlanName = "PlanName",
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
            };
            var statusFields = new Mock<AborResponse.ListingSaleStatusFieldsResponse>();

            var listingSale = new AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse()
            {
                SaleProperty = saleProperty,
                ListPrice = 127738,
                StatusFieldsInfo = statusFields.Object,
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

        private void SetUpVirtualTours()
        {
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[] { GetResidentialListingVirtualTour(), GetResidentialListingVirtualTour() })
            .Verifiable();
        }

        private AborUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.fixture.ApplicationOptions,
                this.mediaRepository.Object,
                this.sqlDataLoader.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);
    }
}
