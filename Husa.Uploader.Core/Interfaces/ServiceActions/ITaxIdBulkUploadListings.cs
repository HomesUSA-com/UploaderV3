namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities.BulkUpload;

    public interface ITaxIdBulkUploadListings : IBulkUpload
    {
        void SetBulkListings(List<TaxIdBulkUploadListingItem> bulkListings);
        Task<UploaderResponse> TaxIdUpdate(TaxIdBulkUploadListingItem listing);
    }
}
