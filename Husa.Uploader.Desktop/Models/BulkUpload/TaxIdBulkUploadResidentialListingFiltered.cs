namespace Husa.Uploader.Desktop.Models.BulkUpload
{
    using Husa.Uploader.Data.Entities.BulkUpload;

    public class TaxIdBulkUploadResidentialListingFiltered
    {
        public bool Selected { get; set; }

        public TaxIdBulkUploadListingItem ResidentialListing { get; set; }
    }
}
