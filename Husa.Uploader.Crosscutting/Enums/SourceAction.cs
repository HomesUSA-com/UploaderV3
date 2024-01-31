namespace Husa.Uploader.Crosscutting.Enums
{
    using System.ComponentModel;

    public enum SourceAction
    {
        [Description("View")]
        View,
        [Description("Upload")]
        Upload,
        [Description("Update Status")]
        UpdateStatus,
        [Description("Update Price")]
        UpdatePrice,
        [Description("Update Completion Date")]
        UpdateCompletionDate,
        [Description("Update Open House")]
        UpdateOpenHouse,
        [Description("Update Images")]
        UpdateImages,
        [Description("Upload Virtual Tour")]
        UploadVirtualTour,
    }
}
