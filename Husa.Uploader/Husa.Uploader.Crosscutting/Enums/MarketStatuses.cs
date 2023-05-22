namespace Husa.Uploader.Crosscutting.Enums
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketStatuses
    {
        [Description("Active")]
        [EnumMember(Value = "ACT")]
        Active,
        [Description("Active Option")]
        [EnumMember(Value = "AO")]
        ActiveOption,
        [Description("Active RFR")]
        [EnumMember(Value = "RFR")]
        ActiveRFR,
        [Description("Back on Market")]
        [EnumMember(Value = "BOM")]
        BackOnMarket,
        [Description("Cancelled")]
        [EnumMember(Value = "CAN")]
        Cancelled,
        [Description("Pending")]
        [EnumMember(Value = "PND")]
        Pending,
        [Description("Pending SB")]
        [EnumMember(Value = "PDB")]
        PendingSB,
        [Description("Price Change")]
        [EnumMember(Value = "PCH")]
        PriceChange,
        [Description("Sold")]
        [EnumMember(Value = "SLD")]
        Sold,
        [Description("Withdrawn")]
        [EnumMember(Value = "WDN")]
        Withdrawn,
    }
}
