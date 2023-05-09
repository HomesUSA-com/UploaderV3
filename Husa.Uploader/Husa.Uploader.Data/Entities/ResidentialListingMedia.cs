using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Data.Entities
{
    public class ResidentialListingMedia : IListingMedia
    {
        public byte[] Data { get; set; }
        public string Caption { get; set; }
        public bool IsPrimary { get; set; }
        public int Order { get; set; }
        public string Id { get; set; }
        public string Extension { get; set; }
        public string PathOnDisk { get; set; }
        public string ExternalUrl { get; set; }
    }
}
