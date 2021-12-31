using Husa.Core.UploaderBase;

namespace Husa.Uploader.Support
{
    public class UploadListingItem
    {
        public System.Guid RequestId { get; set; }
        public int InternalLotRequestId { get; set; }
        public string Market { get; set; }
        public string MlsNumber { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public string BuilderName { get; set; }
        public string BrokerOffice { get; set; }
        public ResidentialListingRequest FullListing { get; set; }
        public string IsLeasing { get; set; }
        public string IsLot { get; set; }
        public string WorkingBy { get; set; }
        public string WorkingStatus { get; set; }
        public string UnitNumber { get; set; }
    }

    public enum Entity
    {
        Listing,
        Lot
    }
}