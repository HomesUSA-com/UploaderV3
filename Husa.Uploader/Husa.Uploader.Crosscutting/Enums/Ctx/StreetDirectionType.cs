namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StreetDirectionType
    {
        [EnumMember(Value = "E")]
        [Description("East")]
        East,
        [EnumMember(Value = "N")]
        [Description("North")]
        North,
        [EnumMember(Value = "NE")]
        [Description("Northeast")]
        Northeast,
        [EnumMember(Value = "NW")]
        [Description("Northwest")]
        Northwest,
        [EnumMember(Value = "S")]
        [Description("South")]
        South,
        [EnumMember(Value = "SE")]
        [Description("Southeast")]
        Southeast,
        [EnumMember(Value = "SW")]
        [Description("Southwest")]
        Southwest,
        [EnumMember(Value = "W")]
        [Description("West")]
        West = 7,
    }
}
