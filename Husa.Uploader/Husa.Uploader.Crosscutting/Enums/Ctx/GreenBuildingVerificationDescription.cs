namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GreenBuildingVerificationDescription
    {
        [EnumMember(Value = "ESCHOME")]
        [Description("ENERGY STAR Certified Homes")]
        ENERGYSTARCertifiedHomes,
        [EnumMember(Value = "HERSSCORE")]
        [Description("HERS Index Score")]
        HERSIndexScore,
        [EnumMember(Value = "HOMEBULD")]
        [Description("Home Energy Score")]
        HomeEnergyScore,
        [EnumMember(Value = "HPESTAR")]
        [Description("Home Performance with ENERGY STAR")]
        HomePerformanceWithENERGYSTAR,
        [EnumMember(Value = "LEEDHOMES")]
        [Description("LEED For Homes")]
        LEEDForHomes,
        [EnumMember(Value = "NGBSCONSTRUCTION")]
        [Description("NGBS New Construction")]
        NGBSNewConstruction,
        [EnumMember(Value = "NGBSREMODEL")]
        [Description("NGBS Small Projects Remodel")]
        NGBSSmallProjectsRemodel,
        [EnumMember(Value = "WATERSENSE")]
        [Description("WaterSense")]
        WaterSense,
    }
}
