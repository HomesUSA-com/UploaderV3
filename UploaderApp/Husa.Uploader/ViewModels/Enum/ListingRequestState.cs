namespace Husa.Uploader.ViewModels.Enum
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public enum ListingRequestState
    {
        [Description("Pending")]
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Completed")]
        [Description("Completed")]
        Completed,
        [EnumMember(Value = "Returned")]
        [Description("Returned")]
        Returned,
        [EnumMember(Value = "Deleted")]
        [Description("Deleted")]
        Deleted,
        [EnumMember(Value = "Processing")]
        [Description("Processing")]
        Processing,
        [EnumMember(Value = "Approved")]
        [Description("Approved")]
        Approved
    }
}
