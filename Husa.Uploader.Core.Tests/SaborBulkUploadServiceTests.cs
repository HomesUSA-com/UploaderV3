namespace Husa.Uploader.Core.Tests
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services.BulkUpload;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SaborBulkUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<SaborBulkUploadService>> logger = new();

        public SaborBulkUploadServiceTests()
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

            // Act and Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.Upload());
        }

        private SaborBulkUploadService GetSut()
            => new(
                this.uploaderClient.Object,
                this.logger.Object);
    }
}
