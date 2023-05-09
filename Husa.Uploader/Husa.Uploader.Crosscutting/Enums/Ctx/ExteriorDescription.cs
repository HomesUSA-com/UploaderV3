namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExteriorDescription
    {
        [EnumMember(Value = "1SIDE")]
        [Description("1-Side Masonry")]
        SideMasonry1,
        [EnumMember(Value = "2SIDE")]
        [Description("2-Side Masonry")]
        SideMasonry2,
        [EnumMember(Value = "3SIDE")]
        [Description("3-Side Masonry")]
        SideMasonry3,
        [EnumMember(Value = "4SIDE")]
        [Description("4-Side Masonry")]
        SideMasonry4,
        [EnumMember(Value = "BATTS")]
        [Description("Batts Insulation")]
        BattsInsulation,
        [EnumMember(Value = "BLOWN")]
        [Description("Blown-In Insulation")]
        BlownInInsulation,
        [EnumMember(Value = "BOARD")]
        [Description("Board & Batten Siding")]
        BoardAndBattenSiding,
        [EnumMember(Value = "BRICK")]
        [Description("Brick")]
        Brick,
        [EnumMember(Value = "BRIVE")]
        [Description("Brick Veneer")]
        BrickVeneer,
        [EnumMember(Value = "DUCTP")]
        [Description("Ducts Professionally Air-Sealed")]
        DuctsProfessionallyAirSealed,
        [EnumMember(Value = "FOAM")]
        [Description("Foam Insulation")]
        FoamInsulation,
        [EnumMember(Value = "Frame")]
        [Description("Frame")]
        Frame,
        [EnumMember(Value = "FRMBRK")]
        [Description("Frame/Brick Veneer")]
        FrameBrickVeneer,
        [EnumMember(Value = "Frame_Stone")]
        [Description("Frame/Stone")]
        FrameStone,
        [EnumMember(Value = "GLASS")]
        [Description("Glass")]
        Glass,
        [EnumMember(Value = "HARDI")]
        [Description("Hardiplank")]
        Hardiplank,
        [EnumMember(Value = "ICAT")]
        [Description("ICAT Recessed Lighting")]
        ICATRecessedLighting,
        [EnumMember(Value = "RADIB")]
        [Description("Radiant Barrier")]
        RadiantBarrier,
        [EnumMember(Value = "ROCKS")]
        [Description("Rock/Stone/Veneer")]
        RockStoneVeneer,
        [EnumMember(Value = "VINYL")]
        [Description("Siding-Vinyl")]
        SidingVinyl,
        [EnumMember(Value = "SPRAYF")]
        [Description("Spray Foam Insulation")]
        SprayFoamInsulation,
        [EnumMember(Value = "STEEL")]
        [Description("Steel Frame")]
        SteelFrame,
        [EnumMember(Value = "STNVEN")]
        [Description("Stone Veneer")]
        StoneVeneer,
        [EnumMember(Value = "STUCC")]
        [Description("Stucco")]
        Stucco,
        [EnumMember(Value = "SYSTUC")]
        [Description("Synthetic Stucco")]
        SyntheticStucco,
        [EnumMember(Value = "WOOD")]
        [Description("Wood")]
        Wood,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
