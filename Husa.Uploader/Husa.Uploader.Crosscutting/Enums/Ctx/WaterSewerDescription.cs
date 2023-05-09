namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WaterSewerDescription
    {
        [EnumMember(Value = "AEROB")]
        [Description("Aerobic Septic")]
        AerobicSeptic,
        [EnumMember(Value = "CITYATSTREET")]
        [Description("City at Street")]
        CityAtStreet,
        [EnumMember(Value = "CITYATPROP")]
        [Description("City on Property")]
        CityOnProperty,
        [EnumMember(Value = "CITYW")]
        [Description("City Water")]
        CityWater,
        [EnumMember(Value = "COOPW")]
        [Description("Co-Op Water")]
        CoOpWater,
        [EnumMember(Value = "MUD")]
        [Description("Municipal Utility District")]
        MunicipalUtilityDistrict,
        [EnumMember(Value = "PUBLI")]
        [Description("Public Sewer")]
        PublicSewer,
        [EnumMember(Value = "SEPRS")]
        [Description("Separate Sewer Meters")]
        SeparateSewerMeters,
        [EnumMember(Value = "SEPRW")]
        [Description("Separate Water Meters")]
        SeparateWaterMeters,
        [EnumMember(Value = "SPTC")]
        [Description("Septic")]
        Septic,
        [EnumMember(Value = "SPTCR")]
        [Description("Septic Required")]
        SepticRequired,
        [EnumMember(Value = "OTHWT")]
        [Description("Other Water-See Remarks")]
        OtherWaterSeeRemarks,
    }
}
