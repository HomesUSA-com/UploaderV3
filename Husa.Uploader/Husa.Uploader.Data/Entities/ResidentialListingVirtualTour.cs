namespace Husa.Uploader.Data.Entities
{
    using Husa.Uploader.Data.Interfaces;

    public class ResidentialListingVirtualTour : IListingMedia
    {
        public Guid Id { get; set; }
        public Uri MediaUri { get; set; }
        public string Caption { get; set; }

        public string GetUnbrandedUrl()
        {
            if (this.MediaUri == null)
            {
                return string.Empty;
            }

            var mediaUrl = this.MediaUri.ToString();

            return mediaUrl
                .Replace("http://", newValue: string.Empty)
                .Replace("https://", newValue: string.Empty);
        }
    }
}
