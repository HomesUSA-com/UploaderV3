using System.Collections.Generic;

namespace Husa.Core.UploaderBase
{
    public class WebDriverUploadInformation
    {
        public bool IsNewListing { get; set; }

        public System.Guid RequestId { get; set; }
        //public int RequestId { get; set; }

        public List<UploaderError> UploaderErrors { get; set; }

        public WebDriverUploadInformation()
        {
            UploaderErrors = new List<UploaderError>();
        }
    }
}
