using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Data
{
    public interface ISqlDataLoader
    {
        Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default);
        
        IEnumerable<IListingMedia> GetListingMedia(Guid residentialListingRequestID, string marketName);

        Task<IEnumerable<ResidentialListingRequest>> GetListingRequest(string residentialListingRequestId, CancellationToken token = default);
    }
}