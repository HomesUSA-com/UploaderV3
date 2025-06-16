namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Data.Entities.BulkUpload;

    public interface ITaxIdBulkUploadListings : IBulkUpload
    {
        void SetBulkListings(List<TaxIdBulkUploadListingItem> bulkListings);
    }
}
