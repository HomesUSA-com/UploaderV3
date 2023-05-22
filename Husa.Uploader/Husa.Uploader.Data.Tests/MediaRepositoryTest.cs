namespace Husa.Uploader.Data.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.MediaService.Api.Contracts.Response;
    using Husa.MediaService.Client;
    using Husa.MediaService.Domain.Enums;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class MediaRepositoryTest
    {
        private const string BaseImageUrl = "https://homesusastoragedev.blob.core.windows.net/husamediastorage/";

        private readonly Mock<HttpClient> httpClient = new();
        private readonly Mock<IMediaServiceClient> mediaServiceClient = new();
        private readonly Mock<ILogger<MediaRepository>> logger = new();

        [Fact]
        public async Task GetRequestMediaSuccess()
        {
            // Arrange
            var mediaId = new Guid("6546a37c-c1c9-4a5d-afbc-bc157273891d");
            var listingRequestId = new Guid("5ffef194-2fec-4001-18a2-08db4dc1035f");

            var mediaDetail = new MediaDetail
            {
                Id = mediaId,
                IsPrimary = true,
                Title = "some-title",
                Description = "some-description",
                MimeType = MimeType.Image,
                Order = 0,
                Uri = new Uri($"{BaseImageUrl}media/{mediaId}"),
                UriMedium = new Uri($"{BaseImageUrl}thumbnail-md/{mediaId}"),
                UriSmall = new Uri($"{BaseImageUrl}thumbnail-sm/{mediaId}"),
            };
            var virtualTourDetail = new VirtualTourDetail
            {
                Id = new Guid("1c102703-4d9d-447c-983d-b921595f5e13"),
                Title = "some-title",
                Description = "some-description",
                Uri = new Uri("https://my.matterport.com/show/?m=yNSfLUX4Mce"),
                EntityId = listingRequestId,
            };
            this.mediaServiceClient
                .Setup(m => m.GetResources(It.Is<Guid>(id => id == listingRequestId), It.Is<MediaType>(type => type == MediaType.ListingRequest), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResourceResponse
                {
                    Media = new[] { mediaDetail },
                    VirtualTour = new[] { virtualTourDetail },
                });

            var repository = this.GetSut();

            // Act
            var result = await repository.GetListingMedia(residentialListingRequestId: listingRequestId, market: MarketCode.SanAntonio);

            // Assert
            Assert.NotEmpty(result);
        }

        private MediaRepository GetSut() => new(
            this.httpClient.Object,
            this.mediaServiceClient.Object,
            this.logger.Object);
    }
}
