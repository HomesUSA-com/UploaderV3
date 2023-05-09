namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentsAvailableDescription
    {
        [EnumMember(Value = "BLDGPLANS")]
        [Description("Building Plans")]
        BuildingPlans,
        [EnumMember(Value = "MUDDIST")]
        [Description("MUD/Water District")]
        MUDWaterDistrict,
        [EnumMember(Value = "PLANSS")]
        [Description("Plans and Specs")]
        PlansAndSpecs,
        [EnumMember(Value = "PLAT")]
        [Description("Plat")]
        Plat,
        [EnumMember(Value = "SBDPL")]
        [Description("Subdivision Plat")]
        SubdivisionPlat,
        [EnumMember(Value = "SBDRE")]
        [Description("Subdivision Restrictions")]
        SubdivisionRestrictions,
        [EnumMember(Value = "SURVE")]
        [Description("Survey")]
        Survey,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
