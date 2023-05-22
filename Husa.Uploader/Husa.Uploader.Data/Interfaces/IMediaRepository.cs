namespace Husa.Uploader.Data.Interfaces
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IMediaRepository
    {
        Task<IEnumerable<ResidentialListingMedia>> GetListingImages(Guid residentialListingRequestId, MarketCode market);

        Task<IEnumerable<ResidentialListingVirtualTour>> GetListingVirtualTours(Guid residentialListingRequestId, MarketCode market);

        Task<IEnumerable<IListingMedia>> GetListingMedia(Guid residentialListingRequestId, MarketCode market);
    }
}
