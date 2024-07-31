namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUpdateStatus
    {
        Task<UploadResult> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
        Task<UploadResult> UpdateLotStatus(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
    }
}
