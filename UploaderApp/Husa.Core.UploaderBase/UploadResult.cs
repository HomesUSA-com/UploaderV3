using System;

namespace Husa.Core.UploaderBase
{
    public enum UploadResult
    {
        Success,
        SuccessWithErrors,
        Failure
    }

    public enum LoginResult 
    { 
        Logged,
        Failure
    }

    public class UploadResponse
    {
        public ResidentialListingRequest Listing { get; set; }
        public CoreWebDriver Driver { get; set; }
        public UploadResult Result { get; set; }
        public IUploader Uploader { get; set; }
        public Exception Exception { get; set; }
    }
}
