namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SprinklerSystemDescription
    {
        [EnumMember(Value = "AUTOMATIC")]
        [Description("Automatic")]
        Automatic,
        [EnumMember(Value = "BACKYARD")]
        [Description("Backyard")]
        Backyard,
        [EnumMember(Value = "DRIPONLY")]
        [Description("Drip Only/Bubblers")]
        DripOnlyBubblers,
        [EnumMember(Value = "FRONTYARD")]
        [Description("Front Yard")]
        FrontYard,
        [EnumMember(Value = "INGROUND")]
        [Description("In-Ground")]
        InGround,
        [EnumMember(Value = "PARTIAL")]
        [Description("Partial")]
        Partial,
        [EnumMember(Value = "RAINSENSOR")]
        [Description("Rain Sensor")]
        RainSensor,
        [EnumMember(Value = "ZONED")]
        [Description("Zoned")]
        Zoned,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
