namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomLevel
    {
        [EnumMember(Value = "L")]
        [Description("Lower")]
        Lower,
        [EnumMember(Value = "M")]
        [Description("Main")]
        Main,
        [EnumMember(Value = "U")]
        [Description("Upper")]
        Upper,
    }
}
