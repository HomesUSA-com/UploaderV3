namespace Husa.Uploader.Core.Tests
{
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
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
        }

        [Fact]
        public async Task CurrentClickOnceVersion_ParseValidVersionString_ReturnsVersion()
        {
            // Arrange
            this.uploaderClient.SetupAllProperties();
            this.serviceSubscriptionClient
                .Setup(x => x.Corporation.GetMarketReverseProspectInformation(It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReverseProspectInfoResponse()
                {
                    UserName = "UserName",
                    Password = "password",
                })
            .Verifiable();

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

        private static AborResponse.ListingRequest.SaleRequest.ListingSaleRequestDetailResponse GetListingRequestDetailResponse()
        {
            var spacesDimensionsInfo = new Mock<AborResponse.SalePropertyDetail.SpacesDimensionsResponse>();
            var addressInfo = new Mock<AborResponse.SalePropertyDetail.AddressInfoResponse>();
            var propertyInfo = new Mock<AborResponse.SalePropertyDetail.PropertyInfoResponse>();
            var featuresInfo = new Mock<AborResponse.SalePropertyDetail.FeaturesResponse>();
            var financialInfo = new AborResponse.SalePropertyDetail.FinancialResponse()
            {
                HasBonusWithAmount = true,
                BuyersAgentCommissionType = CommissionType.Amount,
                AgentBonusAmountType = CommissionType.Percent,
            };
            var schoolsInfo = new Mock<AborResponse.SchoolsResponse>();
            var showingInfo = new Mock<AborResponse.SalePropertyDetail.ShowingResponse>();
            var salePropertyInfo = new Mock<AborResponse.SalePropertyDetail.SalePropertyResponse>();
            var saleProperty = new AborResponse.SalePropertyDetail.SalePropertyDetailResponse()
            {
                SpacesDimensionsInfo = spacesDimensionsInfo.Object,
                AddressInfo = addressInfo.Object,
                PropertyInfo = propertyInfo.Object,
                FeaturesInfo = featuresInfo.Object,
                FinancialInfo = financialInfo,
                SchoolsInfo = schoolsInfo.Object,
                ShowingInfo = showingInfo.Object,
                SalePropertyInfo = salePropertyInfo.Object,
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
