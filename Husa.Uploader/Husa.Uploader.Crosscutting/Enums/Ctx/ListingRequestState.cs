namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ListingRequestState
    {
        [EnumMember(Value = "All")]
        All = 0,
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Completed")]
        Completed = 2,
        [EnumMember(Value = "Returned")]
        Returned = 3,
        [EnumMember(Value = "Deleted")]
        Deleted = 4,
        [EnumMember(Value = "Processing")]
        Processing = 5,
        [EnumMember(Value = "Approved")]
        Approved = 6,
    }
}
