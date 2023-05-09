using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Core.Interfaces
{
    public interface IUploader
    {
        bool CanUpload(ResidentialListingRequest listing);

        UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media);

        LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing);

        UploadResult Logout(CoreWebDriver driver);

        bool IsFlashRequired { get; }
    }
}