namespace Husa.Uploader.Core.Models
{
    public class UploaderError
    {
        public UploaderError(string fieldId, string fieldLabel, string fieldSection, string friendlyErrorMessage, string errorMessage)
        {
            this.FieldId = fieldId;
            this.FieldLabel = fieldLabel;
            this.FieldSection = fieldSection;
            this.FriendlyErrorMessage = friendlyErrorMessage;
            this.ErrorMessage = errorMessage;
            string fieldInfo = "Element " + (!string.IsNullOrEmpty(this.FieldId) ? this.FieldId : "Unknown");
            if (!string.IsNullOrEmpty(fieldLabel))
            {
                fieldInfo += ". Label: " + fieldLabel;
            }

            if (!string.IsNullOrEmpty(fieldSection))
            {
                fieldInfo += ", located in the Tab/Section: " + fieldSection;
            }

            this.FieldInfo = fieldInfo + ". " + friendlyErrorMessage;
        }

        public string FieldId { get; set; }

        public string FieldLabel { get; set; }

        public string FieldSection { get; set; }

        public string FriendlyErrorMessage { get; set; }

        public string ErrorMessage { get; set; }

        public string FieldInfo { get; set; }
    }
}
