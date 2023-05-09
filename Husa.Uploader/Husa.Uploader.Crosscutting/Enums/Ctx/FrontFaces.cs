namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FrontFaces
    {
        [EnumMember(Value = "E")]
        [Description("East")]
        East,
        [EnumMember(Value = "N")]
        [Description("North")]
        North,
        [EnumMember(Value = "NE")]
        [Description("North-East")]
        NorthEast,
        [EnumMember(Value = "NW")]
        [Description("North-West")]
        NorthWest,
        [EnumMember(Value = "S")]
        [Description("South")]
        South,
        [EnumMember(Value = "SE")]
        [Description("South-East")]
        SouthEast,
        [EnumMember(Value = "SW")]
        [Description("South-West")]
        SouthWest,
        [EnumMember(Value = "W")]
        [Description("West")]
        West,
    }
}
