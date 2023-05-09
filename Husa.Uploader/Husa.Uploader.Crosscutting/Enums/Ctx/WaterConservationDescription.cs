namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WaterConservationDescription
    {
        [EnumMember(Value = "EHOTWTR")]
        [Description("Efficient Hot Water Distribution")]
        EfficientHotWaterDistribution,
        [EnumMember(Value = "LOWFLOW")]
        [Description("Low-Flow Fixtures")]
        LowFlowFixtures,
        [EnumMember(Value = "WTRSMRTLND")]
        [Description("Water-Smart Landscaping")]
        WaterSmartLandscaping,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
