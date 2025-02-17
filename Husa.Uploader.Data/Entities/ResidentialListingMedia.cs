namespace Husa.Uploader.Data.Entities
{
    using Husa.Extensions.Media.Constants;
    using Husa.Uploader.Data.Interfaces;

    public class ResidentialListingMedia : IListingMedia
    {
        private string extension = string.Empty;
        private string extensionFromBytes;

        public Guid Id { get; set; }

        public byte[] Data { get; set; }

        public Uri MediaUri { get; set; }

        public string Caption { get; set; }

        public bool IsPrimary { get; set; }

        public int Order { get; set; }

        public bool IsBrokenLink { get; set; } = false;

        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(this.extension))
                {
                    this.extension = GetImageExtension(this.MediaType);
                }

                return this.extension;
            }
            set => this.extension = value;
        }

        public string ExtensionFromBytes
        {
            get
            {
                if (string.IsNullOrEmpty(this.extensionFromBytes))
                {
                    this.extensionFromBytes = GetImageExtension(this.Data);
                }

                return this.extensionFromBytes;
            }
            set => this.extensionFromBytes = value;
        }

        public string PathOnDisk { get; set; }

        public string ExternalUrl { get; set; }

        public string MediaType { get; set; }

        private static string GetImageExtension(string mediaType) => mediaType switch
        {
            ManagedMediaTypes.Pjpeg => ManagedFileExtensions.Jpeg,
            ManagedMediaTypes.Jpeg => ManagedFileExtensions.Jpeg,
            ManagedMediaTypes.Jpg => ManagedFileExtensions.Jpg,
            ManagedMediaTypes.Gif => ManagedFileExtensions.Gif,
            ManagedMediaTypes.Xpng => ManagedFileExtensions.Png,
            ManagedMediaTypes.Png => ManagedFileExtensions.Png,
            _ => string.Empty,
        };

        private static string GetImageExtension(byte[] data)
        {
            if (IsPng(data))
            {
                return ManagedFileExtensions.Png;
            }

            if (IsJpg(data))
            {
                return ManagedFileExtensions.Jpg;
            }

            if (IsJpeg(data))
            {
                return ManagedFileExtensions.Jpeg;
            }

            if (IsGif(data))
            {
                return ManagedFileExtensions.Gif;
            }

            return string.Empty;
        }

        private static bool IsPng(byte[] bytes) => bytes != null && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

        private static bool IsJpg(byte[] bytes) => bytes != null && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;

        private static bool IsGif(byte[] bytes) => bytes != null && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;

        private static bool IsJpeg(byte[] bytes) => bytes != null && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF && (bytes[3] == 0xE0 || bytes[3] == 0xE1 || bytes[3] == 0xE2 || bytes[3] == 0xE3 || bytes[3] == 0xE8 || bytes[3] == 0xE9 || bytes[3] == 0xEE);
    }
}
