namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IEditListing
    {
        Task<UploaderResponse> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        Task<UploaderResponse> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn);
    }
}
