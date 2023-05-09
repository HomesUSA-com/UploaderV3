namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoolingSystemDescription
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
        [EnumMember(Value = "ATTIC")]
        [Description("AtticFan")]
        AtticFan,
        [EnumMember(Value = "CENTR")]
        [Description("Central")]
        Central,
        [EnumMember(Value = "ELECT")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "HEATP")]
        [Description("HeatPump")]
        HeatPump,
        [EnumMember(Value = "SOLAR")]
        [Description("Solar")]
        Solar,
        [EnumMember(Value = "ZONED")]
        [Description("Zoned")]
        Zoned,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
