namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IListingInfo
    {
        string MLSNum { get; }
        string CompanyName { get; }
        Guid CompanyId { get; }
        Guid ListingRequestID { get; }
        Guid ListingID { get; }
        bool IsNewListing { get; }
    }
}
