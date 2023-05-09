namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WaterAccessTypeDescription
    {
        [EnumMember(Value = "BOATDOCK")]
        [Description("Boat Dock")]
        BoatDock,
        [EnumMember(Value = "BOATHOUSE")]
        [Description("Boat House")]
        BoatHouse,
        [EnumMember(Value = "BOATLIFT")]
        [Description("Boat Lift")]
        BoatLift,
        [EnumMember(Value = "BOATRAMP")]
        [Description("Boat Ramp")]
        BoatRamp,
        [EnumMember(Value = "BOATSLIP")]
        [Description("Boat Slip")]
        BoatSlip,
        [EnumMember(Value = "COMMONDOCK")]
        [Description("Common Dock")]
        CommonDock,
        [EnumMember(Value = "COMMONRAMP")]
        [Description("Common Ramp")]
        CommonRamp,
        [EnumMember(Value = "PUBLICDOCK")]
        [Description("Public Dock")]
        PublicDock,
        [EnumMember(Value = "PUBLICRAMP")]
        [Description("Public Ramp")]
        PublicRamp,
        [EnumMember(Value = "SHORE")]
        [Description("Shore")]
        Shore,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
