namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KitchenDescription
    {
        [EnumMember(Value = "BKFAR")]
        [Description("Breakfast Area")]
        BreakfastArea,
        [EnumMember(Value = "BKFBA")]
        [Description("Breakfast Bar")]
        BreakfastBar,
        [EnumMember(Value = "BLINO")]
        [Description("Built-In Oven")]
        BuiltInOven,
        [EnumMember(Value = "Center_Island")]
        [Description("Center Island")]
        CenterIsland,
        [EnumMember(Value = "COOKT")]
        [Description("Cook Top")]
        CookTop,
        [EnumMember(Value = "CUSTO")]
        [Description("Custom Cabinets")]
        CustomCabinets,
        [EnumMember(Value = "Eat_in_Kitchen")]
        [Description("Eat in Kitchen")]
        EatInKitchen,
        [EnumMember(Value = "ELECT")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "GAS")]
        [Description("Gas")]
        Gas,
        [EnumMember(Value = "GRANITECT")]
        [Description("Granite Counter Top")]
        GraniteCounterTop,
        [EnumMember(Value = "ISLAN")]
        [Description("Island")]
        Island,
        [EnumMember(Value = "OPENDIN")]
        [Description("Open To Dining")]
        OpenToDining,
        [EnumMember(Value = "Open_to_Fam_Rm")]
        [Description("Open to Family Room")]
        OpenToFamilyRoom,
        [EnumMember(Value = "PANTR")]
        [Description("Pantry")]
        Pantry,
        [EnumMember(Value = "Walk_in_Pantry")]
        [Description("Pantry-Walk In")]
        PantryWalkIn,
        [EnumMember(Value = "POTFLTR")]
        [Description("Pot Filler")]
        PotFiller,
        [EnumMember(Value = "RANGE")]
        [Description("Range")]
        Range,
        [EnumMember(Value = "CNTOP")]
        [Description("Solid Counter Tops")]
        SolidCounterTops,
        [EnumMember(Value = "BLTMICW")]
        [Description("Built-In Microwave")]
        BuiltInMicrowave,
    }
}
