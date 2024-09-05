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
    public class MediaRepositoryTests
    {
        private const string BaseImageUrl = "https://homesusastoragedev.blob.core.windows.net/husamediastorage/";
        private const string VirtualTourUrl = "https://my.matterport.com/show/?m=yNSfLUX4Mce";

        private readonly Mock<HttpClient> httpClient = new();
        private readonly Mock<IMediaServiceClient> mediaServiceClient = new();
        private readonly Mock<ILogger<MediaRepository>> logger = new();

        [Theory]
        [InlineData("some-title", "some-description")]
        [InlineData("", "some-description")]
        [InlineData("", "")]
        public async Task GetListingImages_ReturnsImages(string title, string description)
        {
            // Arrange
            var mediaId = new Guid("6546a37c-c1c9-4a5d-afbc-bc157273891d");
            var listingRequestId = new Guid("5ffef194-2fec-4001-18a2-08db4dc1035f");

            var mediaDetail = new MediaDetail
            {
                Id = mediaId,
                IsPrimary = true,
                Title = title,
                Description = description,
                MimeType = MimeType.Image,
                Order = 0,
                Uri = new Uri($"{BaseImageUrl}media/{mediaId}"),
                UriMedium = new Uri($"{BaseImageUrl}thumbnail-md/{mediaId}"),
                UriSmall = new Uri($"{BaseImageUrl}thumbnail-sm/{mediaId}"),
            };
            this.mediaServiceClient
                .Setup(m => m.GetResources(It.Is<Guid>(id => id == listingRequestId), It.Is<MediaType>(type => type == MediaType.ListingRequest), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResourceResponse
                {
                    Media = new[] { mediaDetail },
                });

            var repository = this.GetSut();

            // Act
            var result = await repository.GetListingImages(listingRequestId, MarketCode.SanAntonio, default, MediaType.ListingRequest);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(mediaId, result.First().Id);
        }

        [Fact]
        public async Task GetListingImages_NoUri_ReturnsEmpty()
        {
            // Arrange
            var mediaId = Guid.NewGuid();
            var listingRequestId = Guid.NewGuid();

            var mediaDetail = new MediaDetail
            {
                Id = mediaId,
            };
            this.mediaServiceClient
                .Setup(m => m.GetResources(It.Is<Guid>(id => id == listingRequestId), It.Is<MediaType>(type => type == MediaType.ListingRequest), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResourceResponse
                {
                    Media = new[] { mediaDetail },
                });

            var repository = this.GetSut();

            // Act
            var result = await repository.GetListingImages(listingRequestId, MarketCode.SanAntonio, default, MediaType.ListingRequest);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetListingVirtualTours_ReturnsVirtualTours()
        {
            // Arrange
            var virtualTourId = new Guid("6546a37c-c1c9-4a5d-afbc-bc157273891d");
            var listingRequestId = new Guid("5ffef194-2fec-4001-18a2-08db4dc1035f");

            var virtualTourDetail = new VirtualTourDetail
            {
                Id = virtualTourId,
                Title = "some-title",
                Description = "some-description",
                Uri = new Uri(VirtualTourUrl),
            };
            this.mediaServiceClient
                .Setup(m => m.GetResources(It.Is<Guid>(id => id == listingRequestId), It.Is<MediaType>(type => type == MediaType.ListingRequest), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResourceResponse
                {
                    VirtualTour = new[] { virtualTourDetail },
                });

            var repository = this.GetSut();

            // Act
            var result = await repository.GetListingVirtualTours(listingRequestId, MarketCode.SanAntonio, default);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(virtualTourId, result.First().Id);
        }

        private MediaRepository GetSut() => new(
            this.httpClient.Object,
            this.mediaServiceClient.Object,
            this.logger.Object);
    }
}
