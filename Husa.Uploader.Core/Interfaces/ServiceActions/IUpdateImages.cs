namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUpdateImages
    {
        Task<UploaderResponse> UpdateImages(ResidentialListingRequest listing, bool logIn = true, CancellationToken cancellationToken = default);
        Task<UploaderResponse> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default);
    }
}
