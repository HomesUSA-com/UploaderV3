namespace Husa.Uploader.Data.Interfaces
{
    using Husa.Extensions.Common.Enums;
    using Husa.MediaService.Domain.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IMediaRepository
    {
        Task<IEnumerable<ResidentialListingMedia>> GetListingImages(Guid residentialListingRequestId, MarketCode market, CancellationToken token, MediaType mediaType = MediaType.ListingRequest);

        Task<IEnumerable<ResidentialListingVirtualTour>> GetListingVirtualTours(Guid residentialListingRequestId, MarketCode market, CancellationToken token);

        Task PrepareImage(ResidentialListingMedia image, MarketCode marketName, CancellationToken token, string folder);
    }
}
