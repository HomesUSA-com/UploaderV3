namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUploadListing
    {
        MarketCode CurrentMarket { get; }

        bool IsFlashRequired { get; }

        bool CanUpload(ResidentialListingRequest listing);

        Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        Task<LoginResult> Login(Guid companyId);

        UploadResult Logout();

        void CancelOperation();
    }
}
