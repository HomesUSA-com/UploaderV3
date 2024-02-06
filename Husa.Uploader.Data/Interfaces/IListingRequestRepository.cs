namespace Husa.Uploader.Data.Interfaces
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IListingRequestRepository
    {
        Task<IEnumerable<ResidentialListingRequest>> GetListingRequests(CancellationToken token = default);

        Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token = default);

        Task<string> GetListingMlsNumber(Guid residentialListingId, MarketCode marketCode, CancellationToken token = default);
    }
}
