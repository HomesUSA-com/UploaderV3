namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUpdatePrice
    {
        Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);
        Task<UploadResult> UpdateLotPrice(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
    }
}
