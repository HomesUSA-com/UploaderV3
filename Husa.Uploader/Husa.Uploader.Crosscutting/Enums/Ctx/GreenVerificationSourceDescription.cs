namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GreenVerificationSourceDescription
    {
        [EnumMember(Value = "ASSESOR")]
        [Description("Assessor")]
        Assessor,
        [EnumMember(Value = "BUILDER")]
        [Description("Builder")]
        Builder,
        [EnumMember(Value = "CONTRACTOR")]
        [Description("Contractor or Installer")]
        ContractorOrInstaller,
        [EnumMember(Value = "SPONSOR")]
        [Description("Program Sponsor")]
        ProgramSponsor,
        [EnumMember(Value = "VERIFIER")]
        [Description("Program Verifier")]
        ProgramVerifier,
    }
}
