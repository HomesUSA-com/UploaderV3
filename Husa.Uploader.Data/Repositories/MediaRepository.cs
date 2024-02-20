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

        public MediaRepository(
            HttpClient httpClient,
            IMediaServiceClient mediaServiceClient,
            ILogger<MediaRepository> logger)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mediaServiceClient = mediaServiceClient ?? throw new ArgumentNullException(nameof(mediaServiceClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<IListingMedia>> GetListingMedia(Guid residentialListingRequestId, MarketCode market, CancellationToken token)
        {
            var listingMedia = await this.mediaServiceClient.GetResources(entityId: residentialListingRequestId, type: MediaType.ListingRequest, token);
            var consolidatedMedia = ConsolidateImages(listingMedia.Media);
            var result = new List<IListingMedia>();
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
                    Caption = !string.IsNullOrWhiteSpace(mediaDetail.Description) ? mediaDetail.Description : string.Empty,
                    MediaUri = mediaDetail.Uri,
                    Order = mediaDetail.Order ?? count,
                    IsPrimary = mediaDetail.IsPrimary,
                    ExternalUrl = mediaDetail.Uri.ToString(),
                };

                result.Add(residentialListingMedia);
                count++;
            }

            await this.PrepareImages(
                requestMedia: result.OfType<ResidentialListingMedia>(),
                market,
                token);

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

        public async Task<IEnumerable<ResidentialListingMedia>> GetListingImages(Guid residentialListingRequestId, MarketCode market, CancellationToken token)
        {
            var listingMedia = await this.mediaServiceClient.GetResources(entityId: residentialListingRequestId, type: MediaType.ListingRequest, token);
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

            await this.PrepareImages(
                requestMedia: result,
                market,
                token);

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

        private Task<string> DownloadImageAsync(ResidentialListingMedia media, string savePath, CancellationToken token)
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

        private async Task PrepareImages(IEnumerable<ResidentialListingMedia> requestMedia, MarketCode marketName, CancellationToken token)
        {
            var folder = Path.Combine(Path.GetTempPath(), MediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);

            foreach (var image in requestMedia)
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
                    if (marketName == MarketCode.Houston)
                    {
                        var modifiedImage = this.ChangeSize(filePath, image);
                        image.Extension = modifiedImage.Extension;
                        image.PathOnDisk = modifiedImage.PathOnDisk;
                    }
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
        }

        private ResidentialListingMedia ChangeSize(string pathFile, ResidentialListingMedia img)
        {
            string newFileName = $"{pathFile}_modified_{DateTime.Now.Ticks}";
            File.Move(pathFile + img.Extension, newFileName + img.Extension);

            var newImage = Image.FromFile(newFileName + img.Extension);
            int newImageWidth = newImage.Width;
            int newImageHeight = newImage.Height;
            if ((newImageWidth * newImageHeight) >= MaxImageArea)
            {
                int percentage = (100 - int.Parse(Math.Abs((15000000 * 100) / (newImageWidth * newImageHeight)).ToString())) / 2;
                newImageWidth -= (newImageWidth * percentage) / 100;
                newImageHeight -= (newImageHeight * percentage) / 100;

                var newFileSize = new Size(newImageWidth, newImageHeight);
                var photoFile = new Bitmap(newImage, newFileSize);
                photoFile.SetResolution(newImage.HorizontalResolution, newImage.VerticalResolution);
                using (var imageGraphics = Graphics.FromImage(photoFile))
                {
                    imageGraphics.Clear(Color.White);
                    imageGraphics.DrawImageUnscaled(newImage, x: 0, y: 0);
                }

                var imageEncoder = this.GetEncoder(ImageFormat.Jpeg);
                var myEncoderParameters = new EncoderParameters(count: EncoderParametersCount);
                var myEncoderParameter = new EncoderParameter(encoder: Encoder.Quality, value: BitmapQualityLevel);
                myEncoderParameters.Param[0] = myEncoderParameter;

                photoFile.Save(
                    filename: $"{@pathFile}{ManagedFileExtensions.Jpg}",
                    encoder: imageEncoder,
                    encoderParams: myEncoderParameters);

                img.Extension = ManagedFileExtensions.Jpg;
                img.PathOnDisk = $"{@pathFile}{ManagedFileExtensions.Jpg}";

                photoFile.Dispose();
                newImage.Dispose();

                // Delete the original file
                File.Delete(newFileName + img.Extension);
            }
            else
            {
                newImage.Dispose();
                img.PathOnDisk = newFileName + img.Extension;
            }

            return img;
        }

        private void ConvertFilePngToJpg(string pathFile, string actualExt) => this.ConvertImageFormat(pathFile, actualExt, ManagedFileExtensions.Jpg, ImageFormat.Jpeg);

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
