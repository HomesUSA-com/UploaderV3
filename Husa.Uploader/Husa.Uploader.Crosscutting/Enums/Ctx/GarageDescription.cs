namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GarageDescription
    {
        [EnumMember(Value = "ATTCHEDGRG")]
        [Description("Attached Carport")]
        AttachedCarport,
        [EnumMember(Value = "ATTAC")]
        [Description("Attached Garage")]
        AttachedGarage,
        [EnumMember(Value = "CIRCU")]
        [Description("Circular Drive")]
        CircularDrive,
        [EnumMember(Value = "DETCA")]
        [Description("Detached Carport")]
        DetachedCarport,
        [EnumMember(Value = "DETGA")]
        [Description("Detached Garage")]
        DetachedGarage,
        [EnumMember(Value = "DOORMULTI")]
        [Description("Door-Multi")]
        DoorMulti,
        [EnumMember(Value = "DOORSINGLE")]
        [Description("Door-Single")]
        DoorSingle,
        [EnumMember(Value = "ENFC")]
        [Description("Entry-Front Carport")]
        EntryFrontCarport,
        [EnumMember(Value = "ENFG")]
        [Description("Entry-Front Garage")]
        EntryFrontGarage,
        [EnumMember(Value = "REENC")]
        [Description("Entry-Rear Carport")]
        EntryRearCarport,
        [EnumMember(Value = "REENG")]
        [Description("Entry-Rear Garage")]
        EntryRearGarage,
        [EnumMember(Value = "SIENC")]
        [Description("Entry-Side Carport")]
        EntrySideCarport,
        [EnumMember(Value = "SIENG")]
        [Description("Entry-Side Garage")]
        EntrySideGarage,
        [EnumMember(Value = "GARAG")]
        [Description("Garage Door Opener(s)")]
        GarageDoorOpener,
        [EnumMember(Value = "GOLFCART")]
        [Description("Golf Cart/Half Garage")]
        GolfCartHalfGarage,
        [EnumMember(Value = "OVRGA")]
        [Description("Oversized Garage")]
        OversizedGarage,
        [EnumMember(Value = "TANDEM")]
        [Description("Tandem")]
        Tandem,
        [EnumMember(Value = "NONEG")]
        [Description("None-Garage")]
        NoneGarage,
        [EnumMember(Value = "OTHGA")]
        [Description("Other Garage-See Remarks")]
        OtherGarageSeeRemarks,
    }
}
