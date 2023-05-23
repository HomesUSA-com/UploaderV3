using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Accessibility
    {
        [EnumMember(Value = "1STBA")]
        [Description("First Floor Bath")]
        FirstFloorBath,
        [EnumMember(Value = "1STBD")]
        [Description("First Floor Bedroom")]
        FirstFloorBedroom,
        [EnumMember(Value = "1STFB")]
        [Description("Full Bath/Bed on 1st Flr")]
        FullBathBedFirstFloor,
        [EnumMember(Value = "2+ACX")]
        [Description("2+ Access Exits")]
        TwoPlusAccessExits,
        [EnumMember(Value = "36IND")]
        [Description("36 inch or more wide halls")]
        ThirtySixInchOrWideHalls,
        [EnumMember(Value = "42WID")]
        [Description("Hallways 42\" Wide")]
        HallwaysFortyTwoInchWide,
        [EnumMember(Value = "5/8TR")]
        [Description("Thresholds less than 5/8 of an inch")]
        ThresholdsLessThanFiveEightsOfInch,
        [EnumMember(Value = "DRLVH")]
        [Description("Doors w/Lever Handles")]
        DoorsWithLeverHandles,
        [EnumMember(Value = "DRSWI")]
        [Description("Doors-Swing-In")]
        DoorsSwingIn,
        [EnumMember(Value = "ENT1F")]
        [Description("Entry Slope less than 1 foot")]
        EntrySlopeLessThanOneFoot,
        [EnumMember(Value = "EXTDR")]
        [Description("Ext Door Opening 36\"+")]
        ExtDoorOpeningThirtySixInchesPlus,
        [EnumMember(Value = "FLRMO")]
        [Description("Flooring Modifications")]
        FlooringModifications,
        [EnumMember(Value = "HEAMO")]
        [Description("Hearing Modifications")]
        HearingModifications,
        [EnumMember(Value = "INTDR")]
        [Description("Int Door Opening 32\"+")]
        IntDoorOpeningThirtyTwoPlus,
        [EnumMember(Value = "LOBTM")]
        [Description("Low Bathroom Mirrors")]
        LowBathroomMirrors,
        [EnumMember(Value = "LOCLR")]
        [Description("Low Closet Rods")]
        LowClosetRods,
        [EnumMember(Value = "LOWPC")]
        [Description("Low Pile Carpet")]
        LowPileCarpet,
        [EnumMember(Value = "LVDRV")]
        [Description("Level Drive")]
        LevelDrive,
        [EnumMember(Value = "LVLOT")]
        [Description("Level Lot")]
        LevelLot,
        [EnumMember(Value = "NEAR")]
        [Description("Near Bus Line")]
        NearBusLine,
        [EnumMember(Value = "NOCRPT")]
        [Description("No Carpet")]
        NoCarpet,
        [EnumMember(Value = "NOSTP")]
        [Description("No Steps Down")]
        NoStepsDown,
        [EnumMember(Value = "NOSTR")]
        [Description("No Stairs")]
        NoStairs,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "STALL")]
        [Description("Stall Shower")]
        StallShower,
        [EnumMember(Value = "WHLAC")]
        [Description("Wheelchair Accessible")]
        WheelchairAccessible,
    }
}
