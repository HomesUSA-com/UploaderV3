namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MLSStatusType
    {
        [EnumMember(Value = "active")]
        [Description("Active")]
        Active,
        [EnumMember(Value = "activeOption")]
        [Description("Active Option")]
        ActiveOption,
        [EnumMember(Value = "backOnMarket")]
        [Description("Back On Market")]
        BackOnMarket,
        [EnumMember(Value = "cancelled")]
        [Description("Cancelled")]
        Cancelled,
        [EnumMember(Value = "priceChange")]
        [Description("Price Change")]
        PriceChange,
        [EnumMember(Value = "pendingSB")]
        [Description("Pending SB")]
        PendingSB,
        [EnumMember(Value = "pending")]
        [Description("Pending")]
        Pending,
        [EnumMember(Value = "activeRFR")]
        [Description("Active RFR")]
        ActiveRFR,
        [EnumMember(Value = "sold")]
        [Description("Sold")]
        Sold,
        [EnumMember(Value = "withdrawn")]
        [Description("Withdrawn")]
        Withdrawn,
    }
}
