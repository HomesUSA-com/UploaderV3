using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Floors
    {
        [Description("Brick")]
        [EnumMember(Value = "BRICK")]
        Brick,
        [Description("Carpeting")]
        [EnumMember(Value = "CRPT")]
        Carpeting,
        [Description("Ceramic Tile")]
        [EnumMember(Value = "CTILE")]
        CeramicTile,
        [Description("Laminate")]
        [EnumMember(Value = "LMNAT")]
        Laminate,
        [Description("Other")]
        [EnumMember(Value = "OTHER")]
        Other,
        [Description("Saltillo Tile")]
        [EnumMember(Value = "STILE")]
        SaltilloTile,
        [Description("Stone")]
        [EnumMember(Value = "STONE")]
        Stone,
        [Description("Vinyl")]
        [EnumMember(Value = "VINYL")]
        Vinyl,
        [Description("Wood")]
        [EnumMember(Value = "WOOD")]
        Wood,
    }
}
