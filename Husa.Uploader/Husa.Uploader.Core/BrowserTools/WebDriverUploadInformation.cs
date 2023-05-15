using Husa.Uploader.Core.Models;

namespace Husa.Uploader.Core.BrowserTools
{
    public class WebDriverUploadInformation
    {
        public bool IsNewListing { get; set; }

        public Guid RequestId { get; set; }

        public List<UploaderError> UploaderErrors { get; set; }

        public WebDriverUploadInformation()
        {
            UploaderErrors = new List<UploaderError>();
        }

        public void AddError(
            string fieldFindBy,
            string fieldLabel,
            string fieldSection,
            string friendlyErrorMessage,
            string errorMessage = null)
        {
            var error = new UploaderError(
                fieldFindBy,
                fieldLabel,
                fieldSection,
                friendlyErrorMessage,
                errorMessage: errorMessage ?? string.Empty);
            this.UploaderErrors.Add(error);
        }
            
    }
}
