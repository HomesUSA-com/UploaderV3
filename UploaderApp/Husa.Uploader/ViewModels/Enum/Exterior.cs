using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Exterior
    {
        [EnumMember(Value = "1SIDEMASONRY")]
        [Description("1 Side Masonry")]
        OneSideMasonry,
        [EnumMember(Value = "2SIDEMASONRY")]
        [Description("2 Sides Masonry")]
        TwoSidesMasonry,
        [EnumMember(Value = "3SDMS")]
        [Description("3 Sides Masonry")]
        ThreeSidesMasonry,
        [EnumMember(Value = "4SDMS")]
        [Description("4 Sides Masonry")]
        FourSidesMasonry,
        [EnumMember(Value = "ALUMN")]
        [Description("Aluminum")]
        Aluminum,
        [EnumMember(Value = "BRICK")]
        [Description("Brick")]
        Brick,
        [EnumMember(Value = "CMTFB")]
        [Description("Cement Fiber")]
        CementFiber,
        [EnumMember(Value = "ROCKSTONE")]
        [Description("Rock/Stone Veneer")]
        RockStoneVeneer,
        [EnumMember(Value = "SDING")]
        [Description("Siding")]
        Siding,
        [EnumMember(Value = "STONE")]
        [Description("Stone/Rock")]
        StoneRock,
        [EnumMember(Value = "STCCO")]
        [Description("Stucco")]
        Stucco,
        [EnumMember(Value = "VINYL")]
        [Description("Vinyl")]
        Vinyl,
        [EnumMember(Value = "WOOD")]
        [Description("Wood")]
        Wood,
    }
}
