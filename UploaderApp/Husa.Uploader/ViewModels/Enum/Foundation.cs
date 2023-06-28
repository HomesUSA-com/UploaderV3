using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Foundation
    {
        [EnumMember(Value = "BSMNT")]
        [Description("Basement")]
        Basement,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "SLAB")]
        [Description("Slab")]
        Slab,
    }
}
