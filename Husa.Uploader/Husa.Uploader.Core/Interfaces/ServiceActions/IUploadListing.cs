namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUploadListing
    {
        bool IsFlashRequired { get; }

        bool CanUpload(ResidentialListingRequest listing);

        Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        LoginResult Login();

        UploadResult Logout();

        void CancelOperation();
    }
}
