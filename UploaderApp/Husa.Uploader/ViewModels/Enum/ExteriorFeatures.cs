using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExteriorFeatures
    {
        [EnumMember(Value = "CVPAT")]
        [Description("Covered Patio")]
        CoveredPatio,
        [EnumMember(Value = "DBLPN")]
        [Description("Double Pane Windows")]
        DoublePaneWindows,
        [EnumMember(Value = "DK/BL")]
        [Description("Deck/Balcony")]
        DeckBalcony,
        [EnumMember(Value = "GTTRS")]
        [Description("Has Gutters")]
        HasGutters,
        [EnumMember(Value = "OTKT")]
        [Description("Outdoor Kitchen")]
        OutdoorKitchen,
        [EnumMember(Value = "PRSPR")]
        [Description("Partial Sprinkler System")]
        PartialSprinklerSystem,
        [EnumMember(Value = "PTSLB")]
        [Description("Patio Slab")]
        PatioSlab,
        [EnumMember(Value = "PVFNC")]
        [Description("Privacy Fence")]
        PrivacyFence,
        [EnumMember(Value = "SPSYS")]
        [Description("Sprinkler System")]
        SprinklerSystem,
        [EnumMember(Value = "STONE")]
        [Description("Stone/Masonry Fence")]
        StoneMasonryFence,
        [EnumMember(Value = "STRWN")]
        [Description("Storm Windows")]
        StormWindows,
        [EnumMember(Value = "TREES")]
        [Description("Mature Trees")]
        MatureTrees,
        [EnumMember(Value = "WRGHT")]
        [Description("Wrought Iron Fence")]
        WroughtIronFence,
    }
}
