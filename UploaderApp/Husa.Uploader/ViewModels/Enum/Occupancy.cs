using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Occupancy
    {
        [EnumMember(Value = "OWNER")]
        [Description("Owner")]
        Owner,
        [EnumMember(Value = "VACNT")]
        [Description("Vacant")]
        Vacant,
    }
}
