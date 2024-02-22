namespace Husa.Uploader.Data.Interfaces
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IMediaRepository
    {
        Task<IEnumerable<ResidentialListingMedia>> GetListingImages(Guid residentialListingRequestId, MarketCode market, CancellationToken token);

        Task<IEnumerable<ResidentialListingVirtualTour>> GetListingVirtualTours(Guid residentialListingRequestId, MarketCode market, CancellationToken token);

        Task PrepareImage(ResidentialListingMedia image, MarketCode marketName, CancellationToken token, string folder);
    }
}
