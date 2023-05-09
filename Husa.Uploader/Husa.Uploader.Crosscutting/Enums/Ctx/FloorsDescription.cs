namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FloorsDescription
    {
        [EnumMember(Value = "ADOBE")]
        [Description("Adobe")]
        Adobe,
        [EnumMember(Value = "BAMBOO")]
        [Description("Bamboo")]
        Bamboo,
        [EnumMember(Value = "BRICK")]
        [Description("Brick")]
        Brick,
        [EnumMember(Value = "CARPE")]
        [Description("Carpet")]
        Carpet,
        [EnumMember(Value = "CERAMIC")]
        [Description("Ceramic Tile")]
        CeramicTile,
        [EnumMember(Value = "CONCR")]
        [Description("Concrete")]
        Concrete,
        [EnumMember(Value = "CONCSTAINED")]
        [Description("Concrete-Stained")]
        ConcreteStained,
        [EnumMember(Value = "CRICERTIFIED")]
        [Description("CRI Green Label Plus Certified Carpet")]
        CRIGreenLabelPlusCertifiedCarpet,
        [EnumMember(Value = "FloorScoreCERTIFIED")]
        [Description("FloorScore(r) Certified Flooring")]
        FloorScoreCertifiedFlooring,
        [EnumMember(Value = "FSCCERTIFIED")]
        [Description("FSC or SFI Certified Source Hardwood")]
        FSCOrSFICertifiedSourceHardwood,
        [EnumMember(Value = "HARDWOOD")]
        [Description("Hardwood")]
        Hardwood,
        [EnumMember(Value = "LAMIN")]
        [Description("Laminate")]
        Laminate,
        [EnumMember(Value = "LINOLEUM")]
        [Description("Linoleum")]
        Linoleum,
        [EnumMember(Value = "NOCCARPET")]
        [Description("No Carpet")]
        NoCarpet,
        [EnumMember(Value = "SLATE")]
        [Description("Slate")]
        Slate,
        [EnumMember(Value = "STONE")]
        [Description("Stone")]
        Stone,
        [EnumMember(Value = "TERRA")]
        [Description("Terrazzo")]
        Terrazzo,
        [EnumMember(Value = "TILE")]
        [Description("Tile")]
        Tile,
        [EnumMember(Value = "VINYL")]
        [Description("Vinyl")]
        Vinyl,
        [EnumMember(Value = "WOOD")]
        [Description("Wood")]
        Wood,
    }
}
