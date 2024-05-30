namespace Husa.Uploader.Core.Tests
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services.BulkUpload;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class CtxBulkUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ICtxUploadService> uploadService = new();
        private readonly Mock<ILogger<CtxBulkUploadService>> logger = new();

        public CtxBulkUploadServiceTests()
        {
            this.uploaderClient.SetupAllProperties();
        }

        [Fact]
        public void CancelOperation_Success()
        {
            // Act
            var sut = this.GetSut();
            sut.CancelOperation();

            // Assert
            this.uploaderClient.Verify(r => r.CloseDriver(), Times.Once);
        }

        [Theory]
        [InlineData(RequestFieldChange.FullUpload)]
        [InlineData(RequestFieldChange.CompletionDate)]
        [InlineData(RequestFieldChange.ListPrice)]
        [InlineData(RequestFieldChange.ConstructionStage)]
        public void SetRequestFieldChange_Success(RequestFieldChange requestFieldChange)
        {
            // Act
            var sut = this.GetSut();
            sut.SetRequestFieldChange(requestFieldChange);

            // Assert
            Assert.Equal(requestFieldChange, sut.RequestFieldChange);
        }

        [Fact]
        public async Task Upload_Fails()
        {
            // Arrange
            var sut = this.GetSut();

            // Act
            var result = await sut.Upload();

            // Assert
            Assert.Equal(UploadResult.Failure, result);
        }

        [Theory]
        [InlineData(RequestFieldChange.FullUpload)]
        [InlineData(RequestFieldChange.ListPrice)]
        [InlineData(RequestFieldChange.CompletionDate)]
        [InlineData(RequestFieldChange.ConstructionStage)]
        public async Task Upload_Success(RequestFieldChange requestFieldChange)
        {
            // Arrange
            var sut = this.GetSut();
            sut.SetRequestFieldChange(requestFieldChange);
            var bulkListings = this.GetBulkListings();
            sut.SetBulkListings(bulkListings);

            // Act
            var result = await sut.Upload();

            // Assert
            Assert.Equal(UploadResult.Success, result);
        }

        private CtxBulkUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.uploadService.Object,
                this.logger.Object);

        private List<UploadListingItem> GetBulkListings()
        {
            return new List<UploadListingItem>()
            {
                new UploadListingItem()
                {
                    RequestId = Guid.NewGuid(),
                    MlsNumber = "New Listing 1",
                    Address = string.Empty,
                    Status = string.Empty,
                    Market = MarketCode.CTX.ToStringFromEnumMember(),
                    CompanyName = "Company 1",
                },
                new UploadListingItem()
                {
                    RequestId = Guid.NewGuid(),
                    MlsNumber = "New Listing 2",
                    Address = string.Empty,
                    Status = string.Empty,
                    Market = MarketCode.CTX.ToStringFromEnumMember(),
                    CompanyName = "Company 1",
                },
            };
        }
    }
}
