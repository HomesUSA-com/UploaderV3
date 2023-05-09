using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;

namespace Husa.Uploader.Core.Interfaces
{
    public interface IPriceUploader : IUploader
    {
        UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing);
    }
}