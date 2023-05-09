using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Core.Interfaces;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;

namespace Husa.Uploader.Core.Models
{
    public class UploadResponse
    {
        public ResidentialListingRequest Listing { get; set; }
        public CoreWebDriver Driver { get; set; }
        public UploadResult Result { get; set; }
        public IUploader Uploader { get; set; }
        public Exception Exception { get; set; }
    }
}
