using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HomeFaces
    {
        [EnumMember(Value = "E")]
        [Description("East")]
        East,
        [EnumMember(Value = "N")]
        [Description("North")]
        North,
        [EnumMember(Value = "S")]
        [Description("South")]
        South,
        [EnumMember(Value = "W")]
        [Description("West")]
        West,
    }
}
