namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccessRoadSurfaceDescription
    {
        [EnumMember(Value = "ALLEY")]
        [Description("Alley")]
        Alley,
        [EnumMember(Value = "ASPHA")]
        [Description("Asphalt")]
        Asphalt,
        [EnumMember(Value = "CITYS")]
        [Description("City Street")]
        CityStreet,
        [EnumMember(Value = "CONCR")]
        [Description("Concrete")]
        Concrete,
        [EnumMember(Value = "COUNT")]
        [Description("County Road")]
        CountyRoad,
        [EnumMember(Value = "CURBS")]
        [Description("Curbs")]
        Curbs,
        [EnumMember(Value = "PMROAD")]
        [Description("Public Maintained Road")]
        PublicMaintainedRoad,
        [EnumMember(Value = "SIDEW")]
        [Description("Sidewalks")]
        Sidewalks,
        [EnumMember(Value = "STGUTTERS")]
        [Description("Street Gutters")]
        StreetGutters,
        [EnumMember(Value = "STLIGHTS")]
        [Description("Street Lights")]
        StreetLights,
    }
}
