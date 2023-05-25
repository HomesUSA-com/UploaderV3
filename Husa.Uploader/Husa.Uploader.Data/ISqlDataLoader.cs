namespace Husa.Uploader.Data
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Data.Entities;

    public interface ISqlDataLoader
    {
        Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default);

        Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token = default);
    }
}
