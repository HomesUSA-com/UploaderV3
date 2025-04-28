namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUpdateStatus
    {
        Task<UploaderResponse> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);
        Task<UploaderResponse> UpdateLotStatus(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
    }
}
