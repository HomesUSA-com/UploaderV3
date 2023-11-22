namespace Husa.Uploader.Core.Tests
{
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Interfaces;
    using Moq;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public abstract class MarketUploadServiceTests<TUploadService, TResidentialListingRequest>
            where TUploadService : IMarketUploadService
            where TResidentialListingRequest : ResidentialListingRequest
    {
        protected Mock<IMediaRepository> mediaRepository = new();
        protected Mock<IListingRequestRepository> sqlDataLoader = new();
        protected Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();

        [Fact]
        public async Task Upload_ReturnSuccess()
        {
            // Arrange
            this.SetUpConfigs(setUpAdditionalUploaderConfig: true);

            var request = this.GetResidentialListingRequest();
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
        public async Task Upload_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Upload((TResidentialListingRequest)null));
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccess()
        {
            var request = this.GetEmptyListingRequest();
            this.SetUpConfigs(request);

            // Act
            var sut = this.GetSut();
            var result = await sut.UploadVirtualTour(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_ReturnSuccessWithoutVirtualTours()
        {
            // Arrange
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[0])
            .Verifiable();
            var request = this.GetEmptyListingRequest();
            this.SetUpConfigs(request, setUpVirtualTours: false);
            var sut = this.GetSut();

            // Act
            var result = await sut.UploadVirtualTour(request);

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        [Fact]
        public async Task UploadVirtualTour_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UploadVirtualTour((TResidentialListingRequest)null));
        }

        [Fact]
        public async Task UpdateCompletionDate_Success()
        {
            // Arrange
            this.SetUpCredentials();
            this.SetUpCompany();
            var request = this.GetEmptyListingRequest();
            request.MLSNum = "MLSNum";
            this.sqlDataLoader
                .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(request);
            var sut = this.GetSut();

            // Act
            var result = await sut.UpdateCompletionDate(request);

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

        protected abstract TUploadService GetSut();
        protected abstract TResidentialListingRequest GetEmptyListingRequest();
        protected abstract ResidentialListingRequest GetResidentialListingRequest();

        protected void SetUpCompany(string username = "username", string password = "password")
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

        protected void SetUpCredentials()
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

        protected void SetUpVirtualTours()
        {
            this.mediaRepository
                .Setup(x => x.GetListingVirtualTours(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResidentialListingVirtualTour[] { GetResidentialListingVirtualTour(), GetResidentialListingVirtualTour() })
            .Verifiable();
        }

        protected virtual void SetUpConfigs(TResidentialListingRequest request = null, bool setUpVirtualTours = true, bool setUpAdditionalUploaderConfig = false)
        {
            this.SetUpCredentials();
            this.SetUpCompany();
            if (setUpVirtualTours)
            {
                this.SetUpVirtualTours();
            }

            if (request != null)
            {
                this.sqlDataLoader
                    .Setup(x => x.GetListingRequest(It.IsAny<Guid>(), It.IsAny<MarketCode>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(request);
            }
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
    }
}
