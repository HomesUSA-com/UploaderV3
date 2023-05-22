namespace Husa.Uploader.Data
{
    using Husa.Uploader.Data.Entities;

    public interface ISqlDataLoader
    {
        Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default);

        Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, CancellationToken token = default);
    }
}
