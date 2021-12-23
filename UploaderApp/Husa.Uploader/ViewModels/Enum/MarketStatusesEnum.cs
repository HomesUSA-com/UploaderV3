namespace Husa.Cargador.ViewModels.Enum
{
    using System.ComponentModel;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketStatusesEnum
    {
        [Description("Active")]
        [EnumMember(Value = "A")]
        Active,
        [Description("Active Contingent")]
        [EnumMember(Value = "AC")]
        ActiveContingent,
        [Description("Active Kick Out")]
        [EnumMember(Value = "AKO")]
        ActiveKickOut,
        [Description("Active Option Contract")]
        [EnumMember(Value = "AOC")]
        ActiveOptionContract,
        [Description("Cancelled")]
        [EnumMember(Value = "C")]
        Cancelled,
        [Description("Leased")]
        [EnumMember(Value = "L")]
        Leased,
        [Description("Pending")]
        [EnumMember(Value = "P")]
        Pending,
        [Description("Sold")]
        [EnumMember(Value = "S")]
        Sold,
        [Description("TempOffMarket")]
        [EnumMember(Value = "T")]
        TempOffMarket
    }
}
