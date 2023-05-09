namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TopoLandDescription
    {
        [EnumMember(Value = "025AC")]
        [Description("0-.25 Acres")]
        Acres025,
        [EnumMember(Value = "255AC")]
        [Description(".25-.5 Acres")]
        Acres255,
        [EnumMember(Value = "51ACR")]
        [Description(".5-1 Acres")]
        Acres51,
        [EnumMember(Value = "13ACR")]
        [Description("1-3 Acres")]
        Acres13,
        [EnumMember(Value = "ALLEYACCESS")]
        [Description("Alley Access")]
        AlleyAccess,
        [EnumMember(Value = "BACKSTOGOLFCOURSE")]
        [Description("Backs to Golf Course")]
        BackstoGolfCourse,
        [EnumMember(Value = "BACKSTOGREENBELT")]
        [Description("Backs to Greenbelt")]
        BackstoGreenbelt,
        [EnumMember(Value = "CANAL")]
        [Description("Canal")]
        Canal,
        [EnumMember(Value = "CORNE")]
        [Description("Corner Lot")]
        CornerLot,
        [EnumMember(Value = "CULDE")]
        [Description("Cul-de-Sac")]
        CuldeSac,
        [EnumMember(Value = "EXCEP")]
        [Description("Exceptional View")]
        ExceptionalView,
        [EnumMember(Value = "GREEN")]
        [Description("Greenbelt")]
        Greenbelt,
        [EnumMember(Value = "IRREGULAR")]
        [Description("Irregular")]
        Irregular,
        [EnumMember(Value = "LEV")]
        [Description("Level")]
        Level,
        [EnumMember(Value = "MATUR")]
        [Description("Mature Trees")]
        MatureTrees,
        [EnumMember(Value = "ONGOL")]
        [Description("On Golf Course")]
        OnGolfCourse,
        [EnumMember(Value = "OPEN")]
        [Description("Open")]
        Open,
        [EnumMember(Value = "SECLU")]
        [Description("Secluded")]
        Secluded,
        [EnumMember(Value = "SLOPI")]
        [Description("Sloping")]
        Sloping,
        [EnumMember(Value = "WOODE")]
        [Description("Wooded-Partial")]
        WoodedPartial,
        [EnumMember(Value = "XEROSCAPE")]
        [Description("Xeriscape")]
        Xeriscape,
        [EnumMember(Value = "ZEROLOTLINE")]
        [Description("Zero Lot Line")]
        ZeroLotLine,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
