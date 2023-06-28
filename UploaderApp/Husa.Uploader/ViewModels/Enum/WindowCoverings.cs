using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WindowCoverings
    {
        [EnumMember(Value = "ALL")]
        [Description("All Remain")]
        AllRemain,
        [EnumMember(Value = "NONE")]
        [Description("None Remain")]
        NoneRemain,
        [EnumMember(Value = "SOME")]
        [Description("Some Remain")]
        SomeRemain,
    }
}
