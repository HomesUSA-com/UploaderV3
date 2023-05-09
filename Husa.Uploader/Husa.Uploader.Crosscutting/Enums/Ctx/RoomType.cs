namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomType
    {
        [EnumMember(Value = "BATH")]
        [Description("Bathroom")]
        Bathroom,
        [EnumMember(Value = "BEDRO")]
        [Description("Bedroom")]
        Bedroom,
        [EnumMember(Value = "Bedroom_II")]
        [Description("Bedroom II")]
        BedroomII,
        [EnumMember(Value = "Bedroom_III")]
        [Description("Bedroom III")]
        BedroomIII,
        [EnumMember(Value = "Bedroom_IV")]
        [Description("Bedroom IV")]
        BedroomIV,
        [EnumMember(Value = "BonusRoom")]
        [Description("Bonus")]
        Bonus,
        [EnumMember(Value = "BKFRO")]
        [Description("Breakfast")]
        Breakfast,
        [EnumMember(Value = "DININ")]
        [Description("Dining")]
        Dining,
        [EnumMember(Value = "Entry_Forer")]
        [Description("Entry/Foyer")]
        EntryFoyer,
        [EnumMember(Value = "FAMIL")]
        [Description("Family")]
        Family,
        [EnumMember(Value = "GAMER")]
        [Description("Game")]
        Game,
        [EnumMember(Value = "GreatRoom")]
        [Description("Great")]
        Great,
        [EnumMember(Value = "GuestHse")]
        [Description("Guest House")]
        GuestHouse,
        [EnumMember(Value = "Gym")]
        [Description("Gym/Exercise")]
        GymExercise,
        [EnumMember(Value = "JACKNJILL")]
        [Description("Jack-N-Jill")]
        JackNJill,
        [EnumMember(Value = "KITCH")]
        [Description("Kitchen")]
        Kitchen,
        [EnumMember(Value = "Library")]
        [Description("Library/Den")]
        LibraryDen,
        [EnumMember(Value = "LIVIN")]
        [Description("Living Room")]
        LivingRoom,
        [EnumMember(Value = "LIVIN_II")]
        [Description("Living Room II")]
        LivingRoomII,
        [EnumMember(Value = "Loft")]
        [Description("Loft")]
        Loft,
        [EnumMember(Value = "Master_Bath")]
        [Description("Master Bath")]
        MasterBath,
        [EnumMember(Value = "Master_Bath_II")]
        [Description("Master Bath II")]
        MasterBathII,
        [EnumMember(Value = "MSTRB")]
        [Description("Master Bedroom")]
        MasterBedroom,
        [EnumMember(Value = "Master_Bedroom_II")]
        [Description("Master Bedroom II")]
        MasterBedroomII,
        [EnumMember(Value = "MediaRoom")]
        [Description("Media/Home Theatre")]
        MediaHomeTheatre,
        [EnumMember(Value = "OFFIC")]
        [Description("Office/Study")]
        OfficeStudy,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "Other_Room")]
        [Description("Other Room")]
        OtherRoom,
        [EnumMember(Value = "Other_Room_II")]
        [Description("Other Room II")]
        OtherRoomII,
        [EnumMember(Value = "Other_Room_III")]
        [Description("Other Room III")]
        OtherRoomIII,
        [EnumMember(Value = "SaunaRoom")]
        [Description("Sauna Room")]
        SaunaRoom,
        [EnumMember(Value = "UTILI")]
        [Description("Utility/Laundry ")]
        UtilityLaundry,
        [EnumMember(Value = "Wine")]
        [Description("Wine/Cellar")]
        WineCellar,
    }
}
