namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HOATermType
    {
        [EnumMember(Value = "ANNL")]
        [Description("Annual")]
        Annual,
        [EnumMember(Value = "MNTH")]
        [Description("Monthly")]
        Monthly,
        [EnumMember(Value = "QTR")]
        [Description("Quarter")]
        Quarter,
        [EnumMember(Value = "SMIAN")]
        [Description("Semi Annual")]
        SemiAnnual,
    }
}
