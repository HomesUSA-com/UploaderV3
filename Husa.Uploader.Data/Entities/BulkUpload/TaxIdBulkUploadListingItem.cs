namespace Husa.Uploader.Data.Entities.BulkUpload
{
    using System;

    public class TaxIdBulkUploadListingItem
    {
        public Guid Id { get; set; }
        public string MlsNumber { get; set; }
        public string Address { get; set; }
        public string UnitNumber { get; set; }
        public string OwnerName { get; set; }
        public Guid CompanyId { get; set; }
        public string TaxId { get; set; }
        public string Market { get; set; }
    }
}
