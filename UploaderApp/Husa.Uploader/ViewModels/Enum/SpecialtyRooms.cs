using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialtyRooms
    {
        [EnumMember(Value = "WLKIN")]
        [Description("Walk-In Pantry")]
        WalkInPantry,
        [EnumMember(Value = "WIC")]
        [Description("Walk in Closets")]
        WalkInClosets,
        [EnumMember(Value = "UTINS")]
        [Description("Utility Room Inside")]
        UtilityRoomInside,
        [EnumMember(Value = "TELEPHONE")]
        [Description("Telephone")]
        Telephone,
        [EnumMember(Value = "STUDY")]
        [Description("Study/Library")]
        StudyLibrary,
        [EnumMember(Value = "SPDIN")]
        [Description("Separate Dining Room")]
        SeparateDiningRoom,
        [EnumMember(Value = "SKYLT")]
        [Description("Skylights")]
        Skylights,
        [EnumMember(Value = "SCBED")]
        [Description("Secondary Bedroom Down")]
        SecondaryBedroomDown,
        [EnumMember(Value = "PLDWN")]
        [Description("Pull Down Storage")]
        PullDownStorage,
        [EnumMember(Value = "OPEN")]
        [Description("Open Floor Plan")]
        OpenFloorPlan,
        [EnumMember(Value = "MEDIA")]
        [Description("Media Room")]
        MediaRoom,
        [EnumMember(Value = "MDQTR")]
        [Description("Maid's Quarters")]
        MaidQuarters,
        [EnumMember(Value = "LOFT")]
        [Description("Loft")]
        Loft,
        [EnumMember(Value = "LDYUPPER")]
        [Description("Laundry Upper Level")]
        LaundryUpperLevel,
        [EnumMember(Value = "LDYRM")]
        [Description("Laundry Room")]
        LaundryRoom,
        [EnumMember(Value = "LDYMAIN")]
        [Description("Laundry Main Level")]
        LaundryMainLevel,
        [EnumMember(Value = "LDYLOWER")]
        [Description("Laundry Lower Level")]
        LaundryLowerLevel,
        [EnumMember(Value = "LDYKITCHEN")]
        [Description("Laundry in Kitchen")]
        LaundryInKitchen,
        [EnumMember(Value = "LDYCLOSET")]
        [Description("Laundry in Closet")]
        LaundryInCloset,
        [EnumMember(Value = "LDCMB")]
        [Description("Liv/Din Combo")]
        LivingDiningCombo,
        [EnumMember(Value = "ISLKT")]
        [Description("Island Kitchen")]
        IslandKitchen,
        [EnumMember(Value = "INTRN")]
        [Description("High Speed Internet")]
        HighSpeedInternet,
        [EnumMember(Value = "HGHCL")]
        [Description("High Ceilings")]
        HighCeilings,
        [EnumMember(Value = "GAMRM")]
        [Description("Game Room")]
        GameRoom,
        [EnumMember(Value = "FLARM")]
        [Description("Florida Room")]
        FloridaRoom,
        [EnumMember(Value = "EATIN")]
        [Description("Eat-In Kitchen")]
        EatInKitchen,
        [EnumMember(Value = "CABLE")]
        [Description("Cable TV Available")]
        CableTVAvailable,
        [EnumMember(Value = "BKFST")]
        [Description("Breakfast Bar")]
        BreakfastBar,
        [EnumMember(Value = "BEDUP")]
        [Description("All Bedrooms Upstairs")]
        AllBedroomsUpstairs,
        [EnumMember(Value = "ATCSTORAGE")]
        [Description("Attic - Storage Only")]
        AtticStorageOnly,
        [EnumMember(Value = "ATCRBDECK")]
        [Description("Attic - Radiant Barrier Decking")]
        AtticRadiantBarrierDecking,
        [EnumMember(Value = "ATCPULLSTAIRS")]
        [Description("Attic - Pull Down Stairs")]
        AtticPullDownStairs,
        [EnumMember(Value = "ATCPERMSTAIRS")]
        [Description("Attic - Permanent Stairs")]
        AtticPermanentStairs,
        [EnumMember(Value = "ATCPARTFLOORED")]
        [Description("Attic - Partially Floored")]
        AtticPartiallyFloored,
        [EnumMember(Value = "ATCFLOORED")]
        [Description("Attic - Floored")]
        AtticFloored,
        [EnumMember(Value = "ATCFAN")]
        [Description("Attic - Attic Fan")]
        AtticFan,
        [EnumMember(Value = "ATCACCESS")]
        [Description("Attic - Access only")]
        AtticAccessOnly,
        [EnumMember(Value = "ALLBEDSDOWN")]
        [Description("All Bedrooms Downstairs")]
        AllBedroomsDownstairs,
        [EnumMember(Value = "3LVAR")]
        [Description("Three Living Area")]
        ThreeLivingArea,
        [EnumMember(Value = "2LVAR")]
        [Description("Two Living Area")]
        TwoLivingArea,
        [EnumMember(Value = "2ETAR")]
        [Description("Two Eating Areas")]
        TwoEatingAreas,
        [EnumMember(Value = "1LVAR")]
        [Description("One Living Area")]
        OneLivingArea,
        [EnumMember(Value = "1FLRL")]
        [Description("1st Floor Lvl/No Steps")]
        FirstFloorLvlOrNoSteps,
    }
}
