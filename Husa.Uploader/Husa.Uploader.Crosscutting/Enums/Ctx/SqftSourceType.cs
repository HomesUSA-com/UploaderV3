namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SqftSourceType
    {
        [EnumMember(Value = "APPRD")]
        [Description("Appraisal District")]
        AppraisalDistrict,
        [EnumMember(Value = "APPRA")]
        [Description("Appraiser")]
        Appraiser,
        [EnumMember(Value = "ARCHI")]
        [Description("Architect")]
        Architect,
        [EnumMember(Value = "BUILD")]
        [Description("Builder")]
        Builder,
        [EnumMember(Value = "OWNER")]
        [Description("Owner")]
        Owner,
        [EnumMember(Value = "PLANS")]
        [Description("Plans")]
        Plans,
        [EnumMember(Value = "SURVE")]
        [Description("Survey")]
        Survey,
        [EnumMember(Value = "UNKNOWN")]
        [Description("Unknown")]
        Unknown,
    }
}
