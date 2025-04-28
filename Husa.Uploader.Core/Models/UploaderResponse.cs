namespace Husa.Uploader.Core.Models
{
    using Husa.Quicklister.Extensions.Domain.Enums;

    public class UploaderResponse
    {
        public UploadResult UploadResult { get; set; }

        public UploadCommandInfo UploadInformation { get; set; }
    }
}
