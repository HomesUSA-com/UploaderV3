namespace Husa.Uploader.Data.Repositories
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.Media.Constants;
    using Husa.MediaService.Api.Contracts.Response;
    using Husa.MediaService.Client;
    using Husa.MediaService.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Interfaces;
    using ImageMagick;
    using Microsoft.Extensions.Logging;

    public class MediaRepository : IMediaRepository
    {
        public const string MediaFolderName = "Husa.Core.Uploader";

        private const int EncoderParametersCount = 1;
        private const long BitmapQualityLevel = 100L;
        private const int MaxImageArea = 16000000;

        private readonly HttpClient httpClient;
        private readonly IMediaServiceClient mediaServiceClient;
        private readonly ILogger<MediaRepository> logger;
        private int maxWidth;
        private int maxHeight;

        public MediaRepository(
            HttpClient httpClient,
            IMediaServiceClient mediaServiceClient,
            ILogger<MediaRepository> logger)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mediaServiceClient = mediaServiceClient ?? throw new ArgumentNullException(nameof(mediaServiceClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ResidentialListingMedia>> GetListingImages(Guid residentialListingRequestId, MarketCode market, CancellationToken token, MediaType mediaType = MediaType.ListingRequest)
        {
            var listingMedia = await this.mediaServiceClient.GetResources(entityId: residentialListingRequestId, type: mediaType, token);
            var consolidatedMedia = ConsolidateImages(listingMedia.Media);
            var result = new List<ResidentialListingMedia>();
            int count = 0;

            foreach (var mediaDetail in consolidatedMedia)
            {
                if (mediaDetail.Uri == null)
                {
                    this.logger.LogWarning("Skipping image {mediaId}", mediaDetail.Id);
                    continue;
                }

                var residentialListingMedia = new ResidentialListingMedia
                {
                    Id = mediaDetail.Id,
                    Caption = GetCaption(mediaDetail.Title, mediaDetail.Description),
                    MediaUri = mediaDetail.Uri,
                    Order = mediaDetail.Order ?? count,
                    IsPrimary = mediaDetail.IsPrimary,
                    ExternalUrl = mediaDetail.Uri.ToString(),
                };

                result.Add(residentialListingMedia);
                count++;
            }

            switch (market)
            {
                case MarketCode.CTX:
                    this.SetMaxDimensions(1024, 768);
                    break;
                case MarketCode.SanAntonio:
                    this.SetMaxDimensions(1280, 853);
                    break;
                default:
                    this.SetMaxDimensions(2048, 1536);
                    break;
            }

            return result.Where(m => !m.IsBrokenLink).OrderBy(m => m.Order);
        }

        public void SetMaxDimensions(int width, int height)
        {
            this.maxWidth = width;
            this.maxHeight = height;
        }

        public async Task<IEnumerable<ResidentialListingVirtualTour>> GetListingVirtualTours(Guid residentialListingRequestId, MarketCode market, CancellationToken token)
        {
            var listingMedia = await this.mediaServiceClient.GetResources(entityId: residentialListingRequestId, type: MediaType.ListingRequest, token);
            var result = new List<ResidentialListingVirtualTour>();
            foreach (var virtualTourDetail in listingMedia.VirtualTour)
            {
                var virtualTour = new ResidentialListingVirtualTour
                {
                    Id = virtualTourDetail.Id,
                    Caption = !string.IsNullOrWhiteSpace(virtualTourDetail.Description) ? virtualTourDetail.Description : string.Empty,
                    MediaUri = virtualTourDetail.Uri,
                };

                result.Add(virtualTour);
            }

            return result;
        }

        public Task<string> DownloadImageAsync(ResidentialListingMedia media, string savePath, CancellationToken token)
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            return DownloadImages();

            async Task<string> DownloadImages()
            {
                var response = await this.httpClient.GetAsync(media.MediaUri.ToString(), token);

                response.EnsureSuccessStatusCode();
                media.MediaType = response.Content.Headers.ContentType.MediaType;

                using var contentStream = await response.Content.ReadAsStreamAsync(token);
                using var imageAsFileStream = File.Create(path: $"{savePath}{media.Extension}");
                contentStream.Seek(0, SeekOrigin.Begin);
                await contentStream.CopyToAsync(imageAsFileStream, token);

                return response.Content.Headers.ContentType.MediaType;
            }
        }

        public void ConvertFilePngToJpg(string pathFile, string actualExt) => this.ConvertImageFormat(pathFile, actualExt, ManagedFileExtensions.Jpg);

        public async Task PrepareImage(ResidentialListingMedia image, MarketCode marketName, CancellationToken token, string folder)
        {
            var filePath = Path.Combine(folder, image.Id.ToString("N"));
            try
            {
                await this.DownloadImageAsync(
                    media: image,
                    savePath: filePath,
                    token);

                this.ConvertFilePngToJpg(filePath, image.Extension);
                image.Extension = ManagedFileExtensions.Jpg;

                image.PathOnDisk = $"{filePath}{image.Extension}";
                var modifiedImage = this.ChangeSize(filePath, image);
                image.Extension = modifiedImage.Extension;
                image.PathOnDisk = modifiedImage.PathOnDisk;
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogWarning(ex, "Failed to retrieve the image {imageId} in this {mediaUri}", image.Id, image.MediaUri.ToString());
                image.IsBrokenLink = true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to create the on-disk file for the image with {imageId} in this {filePath}", image.Id, folder);
                throw;
            }
        }

        private static string GetCaption(string title = null, string description = null)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return Path.GetFileNameWithoutExtension(title);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                return description;
            }

            return string.Empty;
        }

        private static List<MediaDetail> ConsolidateImages(IEnumerable<MediaDetail> resources)
        {
            var mlsResToReturn = new List<MediaDetail>();
            var mlsResInconsistent = new List<MediaDetail>();

            foreach (var res in resources)
            {
                if (!res.Order.HasValue)
                {
                    mlsResInconsistent.Add(res);
                }
                else
                {
                    mlsResToReturn.Add(res);
                }
            }

            if (mlsResInconsistent.Count > 0)
            {
                mlsResToReturn.AddRange(mlsResInconsistent);
            }

            return mlsResToReturn;
        }

        private ResidentialListingMedia ChangeSize(string pathFile, ResidentialListingMedia img)
        {
            string newFileName = $"{pathFile}_modified_{DateTime.Now.Ticks}";
            File.Move(pathFile + img.Extension, newFileName + img.Extension);

            using (var newImage = Image.FromFile(newFileName + img.Extension))
            {
                int newImageWidth = newImage.Width;
                int newImageHeight = newImage.Height;

                int maxImageArea = this.maxWidth * this.maxHeight;

                if ((newImageWidth * newImageHeight) >= maxImageArea)
                {
                    double percentage = Math.Sqrt((double)maxImageArea / (newImageWidth * newImageHeight));
                    newImageWidth = (int)(newImageWidth * percentage);
                    newImageHeight = (int)(newImageHeight * percentage);

                    using (var resizedImage = new Bitmap(newImage, newImageWidth, newImageHeight))
                    {
                        resizedImage.Save($"{pathFile}{ManagedFileExtensions.Jpg}", ImageFormat.Jpeg);
                        img.Extension = ManagedFileExtensions.Jpg;
                        img.PathOnDisk = $"{pathFile}{ManagedFileExtensions.Jpg}";
                    }
                }
            }

            GC.WaitForPendingFinalizers();

            var destinationFilePath = $"{pathFile}{ManagedFileExtensions.Jpg}";
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }

            File.Move(newFileName + img.Extension, destinationFilePath);
            img.Extension = ManagedFileExtensions.Jpg;
            img.PathOnDisk = destinationFilePath;

            if (File.Exists(newFileName + img.Extension))
            {
                File.Delete(newFileName + img.Extension);
            }

            return img;
        }

        private void ConvertImageFormat(string filePath, string actualExt, string newExtension)
        {
            string newFileName = $"{filePath}_backup_{DateTime.Now.Ticks}";
            File.Move(filePath + actualExt, newFileName + actualExt);

            using (var image = new MagickImage(newFileName + actualExt))
            {
                image.Format = this.GetMagickFormat(newExtension);
                image.Quality = (uint)BitmapQualityLevel;
                image.Write($"{filePath}{newExtension}");
            }

            File.Delete(newFileName + actualExt);
        }

        private MagickFormat GetMagickFormat(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => MagickFormat.Jpeg,
                ".png" => MagickFormat.Png,
                ".webp" => MagickFormat.WebP,
                ".bmp" => MagickFormat.Bmp,
                ".gif" => MagickFormat.Gif,
                _ => throw new NotSupportedException($"format {extension} is not supported."),
            };
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    this.logger.LogInformation("Codec {codecName} found for the image", codec.CodecName);
                    return codec;
                }
            }

            return null;
        }
    }
}
