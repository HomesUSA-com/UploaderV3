using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Core.Interfaces.Services
{
    public interface IUploadListing
    {
        bool CanUpload(ResidentialListingRequest listing);

        UploadResult Upload(ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media);

        LoginResult Login();

        UploadResult Logout();

        void CancelOperation();

        bool IsFlashRequired { get; }
    }
}