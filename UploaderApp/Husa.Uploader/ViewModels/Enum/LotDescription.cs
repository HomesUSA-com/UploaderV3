using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LotDescription
    {
        [EnumMember(Value = "-.5AC")]
        [Description("1/4 - 1/2 Acre")]
        QuarterToHalfAcre,
        [EnumMember(Value = "-1AC")]
        [Description("1/2-1 Acre")]
        HalfToOneAcre,
        [EnumMember(Value = "1-2A")]
        [Description("1 - 2 Acres")]
        OneToTwoAcres,
        [EnumMember(Value = "BLUFF")]
        [Description("Bluff View")]
        BluffView,
        [EnumMember(Value = "CITY")]
        [Description("City View")]
        CityView,
        [EnumMember(Value = "CLDSC")]
        [Description("Cul-de-Sac/Dead End")]
        CulDeSac,
        [EnumMember(Value = "CNTY")]
        [Description("County VIew")]
        CountyView,
        [EnumMember(Value = "CREEK")]
        [Description("Creek")]
        Creek,
        [EnumMember(Value = "CRNR")]
        [Description("Corner")]
        Corner,
        [EnumMember(Value = "GENTLYROLL")]
        [Description("Gently Rolling")]
        GentlyRolling,
        [EnumMember(Value = "GLFCR")]
        [Description("On Golf Course")]
        OnGolfCourse,
        [EnumMember(Value = "GRNBL")]
        [Description("On Greenbelt")]
        OnGreenbelt,
        [EnumMember(Value = "IRR")]
        [Description("Irregular")]
        Irregular,
        [EnumMember(Value = "LEVEL")]
        [Description("Level")]
        Level,
        [EnumMember(Value = "MATURETREES")]
        [Description("Mature Trees (ext feat)")]
        MatureTrees,
        [EnumMember(Value = "PARTWOODED")]
        [Description("Partially Wooded")]
        PartiallyWooded,
        [EnumMember(Value = "SECLUDED")]
        [Description("Secluded")]
        Secluded,
        [EnumMember(Value = "SLOPING")]
        [Description("Sloping")]
        Sloping,
        [EnumMember(Value = "WOODED")]
        [Description("Wooded")]
        Wooded,
        [EnumMember(Value = "XERISCAPED")]
        [Description("Xeriscaped")]
        Xeriscaped,
        [EnumMember(Value = "ZLL")]
        [Description("Zero Lot Line")]
        ZeroLotLine,
    }
}
