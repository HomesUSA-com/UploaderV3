namespace Husa.Uploader.Data.Interfaces.LotListing
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface ILotListingRequestRepository
    {
        Task<IEnumerable<LotListingRequest>> GetListingRequests(CancellationToken token = default);

        Task<IEnumerable<LotListingRequest>> GetListingRequestsByMarketAndAction(MarketCode marketCode, RequestFieldChange requestFieldChange, CancellationToken token = default);

        Task<LotListingRequest> GetListingRequest(Guid lotListingRequestId, MarketCode marketCode, CancellationToken token = default);

        Task<string> GetListingMlsNumber(Guid lotListingId, MarketCode marketCode, CancellationToken token = default);
    }
}
