namespace Husa.Uploader.Crosscutting.Enums
{
    using System.ComponentModel;

    public enum UploaderState
    {
        [Description("Loaded")]
        Loaded,
        [Description("Ready")]
        Ready,
        [Description("Upload in progress")]
        UploadInProgress,
        [Description("Upload failed")]
        UploadFailed,
        [Description("Upload succeeded with errors")]
        UploadSucceededWithErrors,
        [Description("Upload Succeeded")]
        UploadSucceeded,
        [Description("Cancelled")]
        Cancelled,
        [Description("None")]
        None,
    }
}
