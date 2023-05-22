namespace Husa.Uploader.Crosscutting.Enums
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ListingRequestState
    {
        [Description("All")]
        [EnumMember(Value = "All")]
        All,
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
        Approved,
    }
}
