namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUploadListing
    {
        MarketCode CurrentMarket { get; }

        bool IsFlashRequired { get; }

        bool CanUpload(ResidentialListingRequest listing);

        Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        Task<UploadResult> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        Task<LoginResult> Login(Guid companyId);

        UploadResult Logout();

        void CancelOperation();

        Task<UploadResult> UploadLot(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
    }
}
