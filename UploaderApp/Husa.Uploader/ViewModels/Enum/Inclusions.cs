using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Inclusions
    {
        [Description("2nd Floor Utility Room")]
        [EnumMember(Value = "2FLUT")]
        SecondFloorUtilityRoom,
        [Description("Attic Fan")]
        [EnumMember(Value = "ATCFN")]
        AtticFan,
        [Description("Built-In Oven")]
        [EnumMember(Value = "BIOVN")]
        BuiltInOven,
        [Description("Chandelier")]
        [EnumMember(Value = "CHNDL")]
        Chandelier,
        [Description("City Garbage service")]
        [EnumMember(Value = "CITYGARBAGE")]
        CityGarbageservice,
        [Description("Cook Top")]
        [EnumMember(Value = "CKTOP")]
        CookTop,
        [Description("Ceiling Fans")]
        [EnumMember(Value = "CLFNS")]
        CeilingFans,
        [Description("Central Vacuum")]
        [EnumMember(Value = "CNTVC")]
        CentralVacuum,
        [Description("Carbon Monoxide Detector")]
        [EnumMember(Value = "CODETECTOR")]
        CarbonMonoxideDetector,
        [Description("Custom Cabinets")]
        [EnumMember(Value = "CUSTOMCABINETS")]
        CustomCabinets,
        [Description("Double Ovens")]
        [EnumMember(Value = "DBLOVENS")]
        DoubleOvens,
        [Description("Disposal")]
        [EnumMember(Value = "DISPL")]
        Disposal,
        [Description("Down Draft")]
        [EnumMember(Value = "DNDT")]
        DownDraft,
        [Description("Dryer Connection")]
        [EnumMember(Value = "DRYCN")]
        DryerConnection,
        [Description("Dryer")]
        [EnumMember(Value = "DRYER")]
        Dryer,
        [Description("Dishwasher")]
        [EnumMember(Value = "DSHWR")]
        Dishwasher,
        [Description("Electric Water Heater")]
        [EnumMember(Value = "ELWTR")]
        ElectricWaterHeater,
        [Description("Garage Door Opener")]
        [EnumMember(Value = "GARDR")]
        GarageDoorOpener,
        [Description("Gas Grill")]
        [EnumMember(Value = "GRILL")]
        GasGrill,
        [Description("Gas Cooking")]
        [EnumMember(Value = "GSCK")]
        GasCooking,
        [Description("Gas Water Heater")]
        [EnumMember(Value = "GSWTR")]
        GasWaterHeater,
        [Description("Whole House Fan")]
        [EnumMember(Value = "HSEFN")]
        WholeHouseFan,
        [Description("Ice Maker Connection")]
        [EnumMember(Value = "ICMKR")]
        IceMakerConnection,
        [Description("Microwave Oven")]
        [EnumMember(Value = "MCOVN")]
        MicrowaveOven,
        [Description("Plumb for Water Softener")]
        [EnumMember(Value = "PLUMB")]
        PlumbingWaterSoftener,
        [Description("Private Garbage Service")]
        [EnumMember(Value = "PRIVATEGARBAGE")]
        PrivateGarbageService,
        [Description("Propane Water Heater")]
        [EnumMember(Value = "PROPANEWTRHTR")]
        PropaneWaterHeater,
        [Description("Pre-Wired for Security")]
        [EnumMember(Value = "PRWRD")]
        PreWiredSecurity,
        [Description("In Wall Pest Control")]
        [EnumMember(Value = "PSTCN")]
        InWallPestControl,
        [Description("Refrigerator")]
        [EnumMember(Value = "REFRI")]
        Refrigerator,
        [Description("Self-Cleaning Oven")]
        [EnumMember(Value = "SCOVN")]
        SelfCleaningOven,
        [Description("Solid Counter Tops")]
        [EnumMember(Value = "SLCNT")]
        SolidCounterTops,
        [Description("Smooth Cooktop")]
        [EnumMember(Value = "SMCK")]
        SmoothCooktop,
        [Description("Smoke Alarm")]
        [EnumMember(Value = "SMKAL")]
        SmokeAlarm,
        [Description("Security System (Leased)")]
        [EnumMember(Value = "SSLSD")]
        SecuritySystemLeased,
        [Description("Security System (Owned)")]
        [EnumMember(Value = "SSOWN")]
        SecuritySystemOwned,
        [Description("Stove/Range")]
        [EnumMember(Value = "STVRN")]
        StoveRange,
        [Description("2+ Water Heater Units")]
        [EnumMember(Value = "TWOPLUSWTRHTR")]
        TwoPlusWaterHeaterUnits,
        [Description("Vent Fan")]
        [EnumMember(Value = "VNTFN")]
        VentFan,
        [Description("Wet Bar")]
        [EnumMember(Value = "WETBR")]
        WetBar,
        [Description("Washer Connection")]
        [EnumMember(Value = "WSHCN")]
        WasherConnection,
        [Description("Washer")]
        [EnumMember(Value = "WSHER")]
        Washer,
    }
}
