using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrivatePool
    {
        [EnumMember(Value = "PLSWP")]
        [Description("Pools Sweep")]
        PoolsSweep,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "ADJPL")]
        [Description("AdjoiningPool/Spa")]
        AdjoiningPoolSpa,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "FNCPL")]
        [Description("Fenced Pool")]
        FencedPool,
        [EnumMember(Value = "DVBRD")]
        [Description("Diving Board")]
        DivingBoard,
        [EnumMember(Value = "PLSLR")]
        [Description("Pool Solar Heated")]
        PoolSolarHeated,
        [EnumMember(Value = "INGRN")]
        [Description("In Ground Pool")]
        InGroundPool,
        [EnumMember(Value = "HOTTB")]
        [Description("Hot Tub")]
        HotTub,
        [EnumMember(Value = "PLHTD")]
        [Description("Pool is Heated")]
        PoolIsHeated,
        [EnumMember(Value = "ENCPL")]
        [Description("Enclosed Pool")]
        EnclosedPool,
        [EnumMember(Value = "ABVGR")]
        [Description("Above Ground Pool")]
        AboveGroundPool,
    }
}
