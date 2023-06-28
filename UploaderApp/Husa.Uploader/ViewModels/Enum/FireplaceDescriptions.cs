using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FireplaceDescription
    {
        [EnumMember(Value = "GAS")]
        [Description("Gas")]
        Gas,
        [EnumMember(Value = "GASSTARTER")]
        [Description("Gas Starter")]
        GasStarter,
        [EnumMember(Value = "GLASSENCLSCREEN")]
        [Description("Glass/Enclosed Screen")]
        GlassEnclosedScreen,
        [EnumMember(Value = "HEATILATOR")]
        [Description("Heatilator")]
        Heatilator,
        [EnumMember(Value = "LGINC")]
        [Description("Gas Logs Included")]
        GasLogsIncluded,
        [EnumMember(Value = "LIVRM")]
        [Description("Living Room")]
        LivingRoom,
        [EnumMember(Value = "MOCK")]
        [Description("Mock Fireplace")]
        MockFireplace,
        [EnumMember(Value = "NA")]
        [Description("Not Applicable")]
        NotApplicable,
        [EnumMember(Value = "ONE")]
        [Description("One")]
        One,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "STONEROCKBRICK")]
        [Description("Stone/Rock/Brick")]
        StoneRockBrick,
        [EnumMember(Value = "TWO")]
        [Description("Two")]
        Two,
        [EnumMember(Value = "WDBRN")]
        [Description("Wood Burning")]
        WoodBurning,
    }
}
