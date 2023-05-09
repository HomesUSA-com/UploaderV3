using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Core.Interfaces
{
    public interface IUploadVirtualTourUploader : IUploader
    {
        UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media);
    }
}