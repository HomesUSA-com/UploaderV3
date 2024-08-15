namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUpdateImages
    {
        Task<UploadResult> UpdateImages(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
        Task<UploadResult> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default);
    }
}
