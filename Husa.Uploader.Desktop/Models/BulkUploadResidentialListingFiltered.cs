namespace Husa.Uploader.Desktop.Models
{
    using Husa.Uploader.Data.Entities;

    public class BulkUploadResidentialListingFiltered
    {
        public bool Selected { get; set; }

        public UploadListingItem ResidentialListingRequest { get; set; }
    }
}
