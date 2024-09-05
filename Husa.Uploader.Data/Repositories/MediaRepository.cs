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
                    this.maxWidth = 1024;
                    this.maxHeight = 768;
                    break;
                case MarketCode.SanAntonio:
                    this.maxWidth = 1280;
                    this.maxHeight = 853;
                    break;
                default:
                    this.maxWidth = 2048;
                    this.maxHeight = 1536;
                    break;
            }

            return result.Where(m => !m.IsBrokenLink).OrderBy(m => m.Order);
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

        public void ConvertFilePngToJpg(string pathFile, string actualExt) => this.ConvertImageFormat(pathFile, actualExt, ManagedFileExtensions.Jpg, ImageFormat.Jpeg);

        public async Task PrepareImage(ResidentialListingMedia image, MarketCode marketName, CancellationToken token, string folder)
        {
            var filePath = Path.Combine(folder, image.Id.ToString("N"));
            try
            {
                await this.DownloadImageAsync(
                    media: image,
                    savePath: filePath,
                    token);

                if (image.MediaType == ManagedMediaTypes.Gif || image.MediaType == ManagedMediaTypes.Png || image.MediaType == ManagedMediaTypes.Jpeg)
                {
                    this.ConvertFilePngToJpg(filePath, image.Extension);
                    image.Extension = ManagedFileExtensions.Jpg;
                }

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

            var newImage = Image.FromFile(newFileName + img.Extension);
            int newImageWidth = newImage.Width;
            int newImageHeight = newImage.Height;

            int maxImageArea = this.maxWidth * this.maxHeight;

            if ((newImageWidth * newImageHeight) >= maxImageArea)
            {
                double percentage = (double)maxImageArea / (newImageWidth * newImageHeight);
                percentage = (Math.Sqrt(percentage) * 100) - 100;

                newImageWidth = (int)(newImageWidth * (1 + (percentage / 100)));
                newImageHeight = (int)(newImageHeight * (1 + (percentage / 100)));

                var resizedImage = new Bitmap(newImage, newImageWidth, newImageHeight);

                resizedImage.Save($"{pathFile}{ManagedFileExtensions.Jpg}", ImageFormat.Jpeg);

                img.Extension = ManagedFileExtensions.Jpg;
                img.PathOnDisk = $"{pathFile}{ManagedFileExtensions.Jpg}";

                resizedImage.Dispose();
                newImage.Dispose();

                File.Delete(newFileName + img.Extension);
            }
            else
            {
                newImage.Dispose();
                img.PathOnDisk = newFileName + img.Extension;
            }

            return img;
        }

        private void ConvertFileJpgToPng(string pathFile, string actualExt) => this.ConvertImageFormat(pathFile, actualExt, ManagedFileExtensions.Png, ImageFormat.Png);

        private void ConvertImageFormat(string filePath, string actualExt, string newExtension, ImageFormat imageFormat)
        {
            string newFileName = $"{filePath}_backup_{DateTime.Now.Ticks}";
            File.Move(filePath + actualExt, newFileName + actualExt);

            var newImage = Image.FromFile(newFileName + actualExt);
            var newFileSize = new Size(newImage.Width, newImage.Height);
            var photoFile = new Bitmap(newImage, newFileSize);
            photoFile.SetResolution(newImage.HorizontalResolution, newImage.VerticalResolution);
            using (var imageGraphics = Graphics.FromImage(photoFile))
            {
                imageGraphics.Clear(Color.White);
                imageGraphics.DrawImageUnscaled(newImage, x: 0, y: 0);
            }

            var imageEncoder = this.GetEncoder(imageFormat);
            var myEncoderParameters = new EncoderParameters(count: EncoderParametersCount);
            var myEncoderParameter = new EncoderParameter(
                encoder: Encoder.Quality,
                value: BitmapQualityLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;

            photoFile.Save(
                filename: $"{filePath}{newExtension}",
                encoder: imageEncoder,
                encoderParams: myEncoderParameters);

            photoFile.Dispose();
            newImage.Dispose();

            // Delete the original file
            File.Delete(newFileName + actualExt);
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
