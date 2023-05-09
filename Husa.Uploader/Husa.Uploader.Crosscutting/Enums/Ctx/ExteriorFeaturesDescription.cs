namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExteriorFeaturesDescription
    {
        [EnumMember(Value = "AWNING")]
        [Description("Awning")]
        Awning,
        [EnumMember(Value = "CoveredBalcony")]
        [Description("Balcony - Covered")]
        BalconyCovered,
        [EnumMember(Value = "BalconyUnCovered")]
        [Description("Balcony - Uncovered")]
        BalconyUncovered,
        [EnumMember(Value = "BUILTIN")]
        [Description("Built-In Kitchen")]
        BuiltInKitchen,
        [EnumMember(Value = "COVER")]
        [Description("Covered Porch")]
        CoveredPorch,
        [EnumMember(Value = "DECK")]
        [Description("Deck")]
        Deck,
        [EnumMember(Value = "DOUBL")]
        [Description("Double Pane Windows")]
        DoublePaneWindows,
        [EnumMember(Value = "GRILLGAS")]
        [Description("Grill-Gas")]
        GrillGas,
        [EnumMember(Value = "GRILLOTHR")]
        [Description("Grill-Other")]
        GrillOther,
        [EnumMember(Value = "GUTTE")]
        [Description("Gutters-Full")]
        GuttersFull,
        [EnumMember(Value = "PARTIALGTER")]
        [Description("Gutters-Partial")]
        GuttersPartial,
        [EnumMember(Value = "OUTDORFIRE")]
        [Description("Outdoor Fireplace")]
        OutdoorFireplace,
        [EnumMember(Value = "OUTDORKITCHN")]
        [Description("Outdoor Kitchen")]
        OutdoorKitchen,
        [EnumMember(Value = "CPATIO")]
        [Description("Patio-Covered")]
        PatioCovered,
        [EnumMember(Value = "PATIO")]
        [Description("Patio-Enclosed")]
        PatioEnclosed,
        [EnumMember(Value = "UPAT")]
        [Description("Patio-Uncovered")]
        PatioUncovered,
        [EnumMember(Value = "BACKYARDPRIV")]
        [Description("Private Backyard")]
        PrivateBackyard,
        [EnumMember(Value = "GATEPRIV")]
        [Description("Private Gate")]
        PrivateGate,
        [EnumMember(Value = "SECURTYLIGHT")]
        [Description("Security Lighting")]
        SecurityLighting,
        [EnumMember(Value = "SPLYARDLIGHT")]
        [Description("Special Yard Lighting")]
        SpecialYardLighting,
        [EnumMember(Value = "ZEROLOTLINE")]
        [Description("Zero Lot Line")]
        ZeroLotLine,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
