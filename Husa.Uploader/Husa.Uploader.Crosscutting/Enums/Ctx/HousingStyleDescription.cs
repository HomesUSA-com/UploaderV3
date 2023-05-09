namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HousingStyleDescription
    {
        [EnumMember(Value = "COLONIAL")]
        [Description("Colonial")]
        Colonial,
        [EnumMember(Value = "CONDO")]
        [Description("Condo")]
        Condo,
        [EnumMember(Value = "CONTE")]
        [Description("Contemporary/Modern")]
        ContemporaryModern,
        [EnumMember(Value = "CRAFT")]
        [Description("Craftsman")]
        Craftsman,
        [EnumMember(Value = "English_Tudor")]
        [Description("English/Tudor")]
        EnglishTudor,
        [EnumMember(Value = "French")]
        [Description("French")]
        French,
        [EnumMember(Value = "GARDE")]
        [Description("Garden Home")]
        GardenHome,
        [EnumMember(Value = "HILLC")]
        [Description("Hill Country")]
        HillCountry,
        [EnumMember(Value = "MEDIT")]
        [Description("Mediterranean/Spanish")]
        MediterraneanSpanish,
        [EnumMember(Value = "RANCH")]
        [Description("Ranch")]
        Ranch,
        [EnumMember(Value = "SPANISH")]
        [Description("Spanish")]
        Spanish,
        [EnumMember(Value = "Split_Level")]
        [Description("Split Level")]
        SplitLevel,
        [EnumMember(Value = "TOWN")]
        [Description("Townhome")]
        Townhome,
        [EnumMember(Value = "TRADI")]
        [Description("Traditional")]
        Traditional,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
