namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IEditListing
    {
        Task<UploadResult> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        Task<UploadResult> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn);
    }
}
