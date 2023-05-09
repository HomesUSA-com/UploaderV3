namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HeatSystemDescription
    {
        [EnumMember(Value = "1UNIT")]
        [Description("1 Unit")]
        Unit1,
        [EnumMember(Value = "2UNIT")]
        [Description("2 Units")]
        Units2,
        [EnumMember(Value = "3PLUN")]
        [Description("3+ Units")]
        Units3,
        [EnumMember(Value = "CENTR")]
        [Description("Central")]
        Central,
        [EnumMember(Value = "ELECT")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "Fireplace")]
        [Description("Fireplace")]
        Fireplace,
        [EnumMember(Value = "HEATP")]
        [Description("Heat Pump")]
        HeatPump,
        [EnumMember(Value = "GAS")]
        [Description("Natural Gas")]
        NaturalGas,
        [EnumMember(Value = "PROPA")]
        [Description("Propane/Butane")]
        PropaneButane,
        [EnumMember(Value = "RADIANT")]
        [Description("Radiant")]
        Radiant,
        [EnumMember(Value = "ZONED")]
        [Description("Zoned")]
        Zoned,
    }
}
