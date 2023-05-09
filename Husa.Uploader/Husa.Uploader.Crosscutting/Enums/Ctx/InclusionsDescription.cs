namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InclusionsDescription
    {
        [EnumMember(Value = "BEDRMDOWN")]
        [Description("All Bedrooms Down")]
        AllBedroomsDown,
        [EnumMember(Value = "BEDRMUP")]
        [Description("All Bedrooms Up")]
        AllBedroomsUp,
        [EnumMember(Value = "WETBA")]
        [Description("Bar-Wet")]
        BarWet,
        [EnumMember(Value = "BEAMCEILNG")]
        [Description("Beamed Ceilings")]
        BeamedCeilings,
        [EnumMember(Value = "BRKFSTBAR")]
        [Description("Breakfast Bar")]
        BreakfastBar,
        [EnumMember(Value = "BOOKCASE")]
        [Description("Built-in Bookcase")]
        BuiltinBookcase,
        [EnumMember(Value = "ENTERTAINMENT")]
        [Description("Built-in Entertainment")]
        BuiltinEntertainment,
        [EnumMember(Value = "CARBO")]
        [Description("Carbon Monoxide Detector")]
        CarbonMonoxideDetector,
        [EnumMember(Value = "CATHEDRALCS")]
        [Description("Cathedral Ceiling(s)")]
        CathedralCeiling,
        [EnumMember(Value = "CNTRD")]
        [Description("Central Distribution Plumbing System")]
        CentralDistributionPlumbingSystem,
        [EnumMember(Value = "CHANDELIER")]
        [Description("Chandelier")]
        Chandelier,
        [EnumMember(Value = "COFFEREDCS")]
        [Description("Coffered Ceiling(s)")]
        CofferedCeiling,
        [EnumMember(Value = "CROWNMOLD")]
        [Description("Crown Molding")]
        CrownMolding,
        [EnumMember(Value = "DBLVANITY")]
        [Description("Double Vanity")]
        DoubleVanity,
        [EnumMember(Value = "DUMBWAITER")]
        [Description("Dumbwaiter")]
        Dumbwaiter,
        [EnumMember(Value = "EATINKITCH")]
        [Description("Eat-in Kitchen")]
        EatinKitchen,
        [EnumMember(Value = "ELEVA")]
        [Description("Elevator")]
        Elevator,
        [EnumMember(Value = "ENTRFOYER")]
        [Description("Entrance Foyer")]
        EntranceFoyer,
        [EnumMember(Value = "FIREALARM")]
        [Description("Fire Alarm System")]
        FireAlarmSystem,
        [EnumMember(Value = "OPENPLAN")]
        [Description("Floor Plan-Open")]
        FloorPlanOpen,
        [EnumMember(Value = "SPLITPLAN")]
        [Description("Floor Plan-Split")]
        FloorPlanSplit,
        [EnumMember(Value = "FORMALDIN")]
        [Description("Formal Dining")]
        FormalDining,
        [EnumMember(Value = "GAMER")]
        [Description("Game Room")]
        GameRoom,
        [EnumMember(Value = "GRANITE")]
        [Description("Granite Counters")]
        GraniteCounters,
        [EnumMember(Value = "HIGHC")]
        [Description("High Ceilings")]
        HighCeilings,
        [EnumMember(Value = "CLOSETS")]
        [Description("His and Hers Closets")]
        HisandHersClosets,
        [EnumMember(Value = "INLAW")]
        [Description("In-Law Suites")]
        InLawSuites,
        [EnumMember(Value = "KITDIN")]
        [Description("Kit/Din Combo")]
        KitDinCombo,
        [EnumMember(Value = "LAMINATED")]
        [Description("Laminate Counters")]
        LaminateCounters,
        [EnumMember(Value = "LIVDIN")]
        [Description("Liv/Din Combo")]
        LivDinCombo,
        [EnumMember(Value = "LOWFLOWPLUMB")]
        [Description("Low Flow Plumbing Fixtures")]
        LowFlowPlumbingFixtures,
        [EnumMember(Value = "MSTRD")]
        [Description("Master Down")]
        MasterDown,
        [EnumMember(Value = "MSTRU")]
        [Description("Master Up")]
        MasterUp,
        [EnumMember(Value = "MLTDI")]
        [Description("Multiple Dining Areas")]
        MultipleDiningAreas,
        [EnumMember(Value = "MLTLI")]
        [Description("Multiple Living Areas")]
        MultipleLivingAreas,
        [EnumMember(Value = "NATURWOODWK")]
        [Description("Natural Woodwork")]
        NaturalWoodwork,
        [EnumMember(Value = "NODINING")]
        [Description("No Dining")]
        NoDining,
        [EnumMember(Value = "OFFIC")]
        [Description("Office")]
        Office,
        [EnumMember(Value = "PANTRY")]
        [Description("Pantry")]
        Pantry,
        [EnumMember(Value = "RECESSED")]
        [Description("Recessed Lighting")]
        RecessedLighting,
        [EnumMember(Value = "SECSYSOWN")]
        [Description("Security System-Owned")]
        SecuritySystemOwned,
        [EnumMember(Value = "SEPAR")]
        [Description("Separate Shower")]
        SeparateShower,
        [EnumMember(Value = "SHOWO")]
        [Description("Shower Only")]
        ShowerOnly,
        [EnumMember(Value = "SHOWT")]
        [Description("Shower/Tub Combo")]
        ShowerTubCombo,
        [EnumMember(Value = "SHUTTERS")]
        [Description("Shutters")]
        Shutters,
        [EnumMember(Value = "SHUTTERSPLANT")]
        [Description("Shutters-Plantation")]
        ShuttersPlantation,
        [EnumMember(Value = "SMARTHOME")]
        [Description("Smart Home")]
        SmartHome,
        [EnumMember(Value = "SMARTTHERMO")]
        [Description("Smart Thermostat")]
        SmartThermostat,
        [EnumMember(Value = "SMOKE")]
        [Description("Smoke Detector")]
        SmokeDetector,
        [EnumMember(Value = "SPLIT")]
        [Description("Split Bedroom")]
        SplitBedroom,
        [EnumMember(Value = "STONECOUNTR")]
        [Description("Stone Counters")]
        StoneCounters,
        [EnumMember(Value = "STORAGE")]
        [Description("Storage")]
        Storage,
        [EnumMember(Value = "TILECONTERS")]
        [Description("Tile Counters")]
        TileCounters,
        [EnumMember(Value = "TRACKLIGHT")]
        [Description("Track Lighting")]
        TrackLighting,
        [EnumMember(Value = "TRAYCEILINGS")]
        [Description("Tray Ceiling(s)")]
        TrayCeiling,
        [EnumMember(Value = "GARDE")]
        [Description("Tub - Garden")]
        TubGarden,
        [EnumMember(Value = "TUBJACUZZI")]
        [Description("Tub - Jacuzzi")]
        TubJacuzzi,
        [EnumMember(Value = "TUBSOAKING")]
        [Description("Tub-Soaking")]
        TubSoaking,
        [EnumMember(Value = "VAULTEDCS")]
        [Description("Vaulted Ceiling(s)")]
        VaultedCeiling,
        [EnumMember(Value = "WALKI")]
        [Description("Walk-In Closet(s)")]
        WalkInCloset,
        [EnumMember(Value = "WTRSENSE")]
        [Description("WaterSense Fixture(s)")]
        WaterSenseFixture,
        [EnumMember(Value = "WIREDDATA")]
        [Description("Wired for Data")]
        WiredForData,
        [EnumMember(Value = "WIREDSECUR")]
        [Description("Wired for Security")]
        WiredForSecurity,
        [EnumMember(Value = "WSPEAK")]
        [Description("Wired for Speakers")]
        WiredForSpeakers,
    }
}
