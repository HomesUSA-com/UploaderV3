namespace Husa.Uploader.Core.Models
{
    public class UploadCommandInfo
    {
        public UploadCommandInfo()
        {
            this.UploaderErrors = new List<UploaderError>();
        }

        public bool IsNewListing { get; set; }

        public Guid RequestId { get; set; }

        public List<UploaderError> UploaderErrors { get; set; }

        public string UserFullName { get; set; }

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
