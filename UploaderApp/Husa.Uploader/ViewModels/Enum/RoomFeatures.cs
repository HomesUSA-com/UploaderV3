using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomFeatures
    {
        [EnumMember(Value = "CLFAN")]
        [Description("Ceiling Fan")]
        CeilingFan,
        [EnumMember(Value = "DUAL")]
        [Description("Dual Masters")]
        DualMasters,
        [EnumMember(Value = "DWNST")]
        [Description("DownStairs")]
        DownStairs,
        [EnumMember(Value = "FLBT")]
        [Description("Full Bath")]
        FullBath,
        [EnumMember(Value = "HFBT")]
        [Description("Half Bath")]
        HalfBath,
        [EnumMember(Value = "MULTI")]
        [Description("Multi-Closets")]
        MultiClosets,
        [EnumMember(Value = "NA")]
        [Description("Not Applicable/None")]
        NotApplicable,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "OUTAC")]
        [Description("Outside Access")]
        OutsideAccess,
        [EnumMember(Value = "SITRM")]
        [Description("Sitting Room")]
        SittingRoom,
        [EnumMember(Value = "SPLIT")]
        [Description("Split")]
        Split,
        [EnumMember(Value = "UPSTR")]
        [Description("Upstairs")]
        Upstairs,
        [EnumMember(Value = "WLKIN")]
        [Description("Walk-In Closet")]
        WalkInCloset,
        [EnumMember(Value = "TWHRL")]
        [Description("Tub has Whirlpool")]
        TubHasWhirlpool,
        [EnumMember(Value = "TSSEP")]
        [Description("Tub/Shower Separate")]
        TubShowerSeparate,
        [EnumMember(Value = "TSCMB")]
        [Description("Tub/Shower Combo")]
        TubShowerCombo,
        [EnumMember(Value = "TONLY")]
        [Description("Tub Only")]
        TubOnly,
        [EnumMember(Value = "SONLY")]
        [Description("Shower Only")]
        ShowerOnly,
        [EnumMember(Value = "SNGVN")]
        [Description("Single Vanity")]
        SingleVanity,
        [EnumMember(Value = "SEPVN")]
        [Description("Separate Vanity")]
        SeparateVanity,
        [EnumMember(Value = "NT/S")]
        [Description("None/No Tub or Shower")]
        NoTubOrShower,
        [EnumMember(Value = "GRDTB")]
        [Description("Garden Tub")]
        GardenTub,
        [EnumMember(Value = "DBLVN")]
        [Description("Double Vanity")]
        DoubleVanity,
        [EnumMember(Value = "BIDET")]
        [Description("Bidet")]
        Bidet,
    }
}
