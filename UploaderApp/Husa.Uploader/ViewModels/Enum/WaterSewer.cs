using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WaterSewer
    {
        [EnumMember(Value = "NONEWATER")]
        [Description("None - Water")]
        NoneWater,
        [EnumMember(Value = "WTRSY")]
        [Description("Water System")]
        WaterSystem,
        [EnumMember(Value = "SWRSY")]
        [Description("Sewer System")]
        SewerSystem,
        [EnumMember(Value = "SPTIC")]
        [Description("Septic")]
        Septic,
        [EnumMember(Value = "CITY")]
        [Description("City")]
        City,
        [EnumMember(Value = "PRVT")]
        [Description("Private Well")]
        PrivateWell,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "NONESEWER")]
        [Description("None - Sewer")]
        NoneSewer,
        [EnumMember(Value = "WTRST")]
        [Description("Water Storage")]
        WaterStorage,
        [EnumMember(Value = "COOPWATER")]
        [Description("Co-op Water")]
        CoOpWater,
        [EnumMember(Value = "ARBSP")]
        [Description("Aerobic Septic")]
        AerobicSeptic,
    }
}
