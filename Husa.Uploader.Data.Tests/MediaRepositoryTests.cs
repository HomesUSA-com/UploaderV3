namespace Husa.Uploader.Data.Tests
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Net;
    using System.Net.Http;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.Media.Constants;
    using Husa.MediaService.Api.Contracts.Response;
    using Husa.MediaService.Client;
    using Husa.MediaService.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class MediaRepositoryTests
    {
        private const string BaseImageUrl = "https://homesusastoragedev.blob.core.windows.net/husamediastorage/";
        private const string VirtualTourUrl = "https://my.matterport.com/show/?m=yNSfLUX4Mce";

        private readonly Mock<HttpClient> httpClient = new();
        private readonly Mock<IMediaServiceClient> mediaServiceClient = new();
        private readonly Mock<ILogger<MediaRepository>> logger = new();
        private readonly Mock<HttpMessageHandler> httpMessageHandler = new();

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

        [Fact]
        public async Task PrepareImage_ValidPngImage_ConvertsToJpg()
        {
            // Arrange
            var folder = Path.GetTempPath();
            var dummyFilePath = Path.Combine(folder, "dummy.png");

            using (var bitmap = new Bitmap(1282, 853))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Blue);
                }

                bitmap.Save(dummyFilePath, ImageFormat.Png);
            }

            var media = new ResidentialListingMedia
            {
                Id = Guid.NewGuid(),
                Extension = ".png",
                MediaUri = new Uri("https://example.com/image.png"),
                MediaType = "image/png",
                Data = File.ReadAllBytes(dummyFilePath),
            };

            var filePath = Path.Combine(folder, media.Id.ToString("N"));

            using (var bitmap = new Bitmap(1282, 853))
            {
                bitmap.Save($"{filePath}.png", ImageFormat.Png);
            }

            var repository = this.GetSut();

            repository.SetMaxDimensions(1280, 853);

            this.httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(File.ReadAllBytes($"{filePath}.png")))
                    {
                        Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png") },
                    },
                });

            try
            {
                // Act
                await repository.PrepareImage(media, MarketCode.SanAntonio, default, folder);

                // Assert
                Assert.Equal(ManagedFileExtensions.Jpg, media.Extension);
                Assert.Equal($"{filePath}{ManagedFileExtensions.Jpg}", media.PathOnDisk);
                Assert.True(File.Exists(media.PathOnDisk));
            }
            finally
            {
                if (File.Exists(dummyFilePath))
                {
                    File.Delete(dummyFilePath);
                }

                if (File.Exists($"{filePath}.png"))
                {
                    File.Delete($"{filePath}.png");
                }

                if (File.Exists($"{filePath}{ManagedFileExtensions.Jpg}"))
                {
                    File.Delete($"{filePath}{ManagedFileExtensions.Jpg}");
                }
            }
        }

        [Fact]
        public async Task PrepareImage_FailedDownload_MarksAsBrokenLink()
        {
            // Arrange
            var media = new ResidentialListingMedia
            {
                Id = Guid.NewGuid(),
                Extension = ".png",
                MediaUri = new Uri("https://example.com/image.png"),
            };

            var folder = Path.GetTempPath();
            var repository = this.GetSut();

            this.httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Failed to download"));

            // Act
            await repository.PrepareImage(media, MarketCode.SanAntonio, default, folder);

            // Assert
            Assert.True(media.IsBrokenLink);
            Assert.Null(media.PathOnDisk);
            Assert.Equal(".png", media.Extension);
        }

        [Fact]
        public async Task PrepareImage_ErrorDuringProcessing_LogsError()
        {
            // Arrange
            var media = new ResidentialListingMedia
            {
                Id = Guid.NewGuid(),
                Extension = ".png",
                MediaUri = new Uri("https://example.com/image.png"),
            };

            var folder = Path.GetTempPath();
            var repository = this.GetSut();

            this.httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(new byte[10])), // Datos corruptos
                });

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
                await repository.PrepareImage(media, MarketCode.SanAntonio, default, folder));

            this.logger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("Failed to create the on-disk file for the image with") &&
                        v.ToString().Contains(media.Id.ToString())),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task PrepareImage_HandlesMagickException_UsesFallback()
        {
            // Arrange
            var folder = Path.GetTempPath();
            var dummyFilePath = Path.Combine(folder, "dummy.png");

            using (var bitmap = new Bitmap(800, 600))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Blue);
                }

                bitmap.Save(dummyFilePath, ImageFormat.Png);
            }

            File.AppendAllText(dummyFilePath, "Corrupted data");

            var media = new ResidentialListingMedia
            {
                Id = Guid.NewGuid(),
                Extension = ".png",
                MediaUri = new Uri("https://example.com/image.png"),
                MediaType = "image/png",
                Data = File.ReadAllBytes(dummyFilePath),
            };

            var filePath = Path.Combine(folder, media.Id.ToString("N"));

            var repository = this.GetSut();
            repository.SetMaxDimensions(1280, 853);

            this.httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(File.ReadAllBytes(dummyFilePath)))
                    {
                        Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png") },
                    },
                });

            try
            {
                // Act
                await repository.PrepareImage(media, MarketCode.SanAntonio, default, folder);

                // Assert
                Assert.Equal(ManagedFileExtensions.Jpg, media.Extension);
                Assert.True(File.Exists($"{filePath}{ManagedFileExtensions.Jpg}"), "Fallback mechanism did not produce the output file.");
            }
            finally
            {
                if (File.Exists(dummyFilePath))
                {
                    File.Delete(dummyFilePath);
                }

                if (File.Exists($"{filePath}.png"))
                {
                    File.Delete($"{filePath}.png");
                }

                if (File.Exists($"{filePath}{ManagedFileExtensions.Jpg}"))
                {
                    File.Delete($"{filePath}{ManagedFileExtensions.Jpg}");
                }
            }
        }

        [Theory]
        [InlineData(".png", "image/png")]
        [InlineData(".bmp", "image/bmp")]
        [InlineData(".gif", "image/gif")]
        [InlineData(".jpg", "image/jpeg")]
        [InlineData(".webp", "image/webp")]
        public async Task PrepareImage_HandlesDifferentFormats_ConvertsToJpg(string extension, string mimeType)
        {
            // Arrange
            var folder = Path.GetTempPath();
            var dummyFilePath = Path.Combine(folder, $"dummy{extension}");

            using (var bitmap = new Bitmap(800, 600))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Red);
                }

                ImageFormat imageFormat = extension switch
                {
                    ".png" => ImageFormat.Png,
                    ".bmp" => ImageFormat.Bmp,
                    ".gif" => ImageFormat.Gif,
                    ".jpg" => ImageFormat.Jpeg,
                    ".webp" => ImageFormat.Png,
                    _ => throw new NotSupportedException($"Format {extension} is not supported in test setup."),
                };

                bitmap.Save(dummyFilePath, imageFormat);
            }

            var media = new ResidentialListingMedia
            {
                Id = Guid.NewGuid(),
                Extension = extension,
                MediaUri = new Uri($"https://example.com/dummy{extension}"),
                MediaType = mimeType,
                Data = File.ReadAllBytes(dummyFilePath),
            };

            var filePath = Path.Combine(folder, media.Id.ToString("N"));
            var repository = this.GetSut();
            repository.SetMaxDimensions(1280, 853);

            this.httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(File.ReadAllBytes(dummyFilePath)))
                    {
                        Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType) },
                    },
                });

            try
            {
                // Act
                await repository.PrepareImage(media, MarketCode.SanAntonio, default, folder);

                // Assert
                Assert.Equal(ManagedFileExtensions.Jpg, media.Extension);
                Assert.True(File.Exists($"{filePath}{ManagedFileExtensions.Jpg}"), $"Failed to generate JPG for format {extension}");
            }
            finally
            {
                if (File.Exists(dummyFilePath))
                {
                    File.Delete(dummyFilePath);
                }

                if (File.Exists($"{filePath}{ManagedFileExtensions.Jpg}"))
                {
                    File.Delete($"{filePath}{ManagedFileExtensions.Jpg}");
                }
            }
        }

        private MediaRepository GetSut()
        {
            var httpClient2 = new HttpClient(this.httpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com"),
            };

            return new MediaRepository(
                httpClient2,
                this.mediaServiceClient.Object,
                this.logger.Object);
        }
    }
}
