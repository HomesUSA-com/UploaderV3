namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NeighborhoodAmenitiesDescription
    {
        [EnumMember(Value = "BBQGR")]
        [Description("BBQ/Grill")]
        BBQGrill,
        [EnumMember(Value = "BIKET")]
        [Description("Bike Trails")]
        BikeTrails,
        [EnumMember(Value = "BOATD")]
        [Description("Boat Dock")]
        BoatDock,
        [EnumMember(Value = "BOATR")]
        [Description("Boat Ramp")]
        BoatRamp,
        [EnumMember(Value = "CABANA")]
        [Description("Cabana")]
        Cabana,
        [EnumMember(Value = "CLUBH")]
        [Description("Club House")]
        ClubHouse,
        [EnumMember(Value = "CONTR")]
        [Description("Controlled Access")]
        ControlledAccess,
        [EnumMember(Value = "DOGPRK")]
        [Description("Dog Park")]
        DogPark,
        [EnumMember(Value = "EXTLIGHT")]
        [Description("Exterior Lights")]
        ExteriorLights,
        [EnumMember(Value = "FISHI")]
        [Description("Fishing Pier")]
        FishingPier,
        [EnumMember(Value = "FITCNT")]
        [Description("Fitness Center")]
        FitnessCenter,
        [EnumMember(Value = "GOLFC")]
        [Description("Golf Course Access")]
        GolfCourseAccess,
        [EnumMember(Value = "LAKER")]
        [Description("Lake/River/Park")]
        LakeRiverPark,
        [EnumMember(Value = "LIVINGAREA")]
        [Description("Living Area")]
        LivingArea,
        [EnumMember(Value = "PARKA")]
        [Description("Park Access")]
        ParkAccess,
        [EnumMember(Value = "PLAYGRND")]
        [Description("Playground")]
        Playground,
        [EnumMember(Value = "PUBRESTROOM")]
        [Description("Public Restroom")]
        PublicRestroom,
        [EnumMember(Value = "RECROOM")]
        [Description("Recreation Room")]
        RecreationRoom,
        [EnumMember(Value = "SECURITYSYS")]
        [Description("Security System")]
        SecuritySystem,
        [EnumMember(Value = "SPORTCOURT")]
        [Description("Sport Court(s)")]
        SportCourt,
        [EnumMember(Value = "TENNI")]
        [Description("Tennis Courts")]
        TennisCourts,
        [EnumMember(Value = "VOLLE")]
        [Description("Volleyball Court")]
        VolleyballCourt,
        [EnumMember(Value = "WALKI")]
        [Description("Walking/Jogging/Bike Trails")]
        WalkingJoggingBikeTrails,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
