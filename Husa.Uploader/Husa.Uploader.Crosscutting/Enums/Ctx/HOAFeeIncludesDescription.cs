namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HOAFeeIncludesDescription
    {
        [EnumMember(Value = "CABLE")]
        [Description("Cable")]
        Cable,
        [EnumMember(Value = "MAINT")]
        [Description("Exterior Maintenance")]
        ExteriorMaintenance,
        [EnumMember(Value = "GAS")]
        [Description("Gas")]
        Gas,
        [EnumMember(Value = "INTERNET")]
        [Description("Internet")]
        Internet,
        [EnumMember(Value = "LANDSCAPING")]
        [Description("Landscaping")]
        Landscaping,
        [EnumMember(Value = "TRASH")]
        [Description("Trash Collection")]
        TrashCollection,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
