namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppliancesEquipmentDescription
    {
        [EnumMember(Value = "CTELECT")]
        [Description("Cook Top Electric")]
        CookTopElectric,
        [EnumMember(Value = "CTGAS")]
        [Description("Cook Top Gas")]
        CookTopGas,
        [EnumMember(Value = "DISHW")]
        [Description("Dishwasher")]
        Dishwasher,
        [EnumMember(Value = "DISPO")]
        [Description("Disposal")]
        Disposal,
        [EnumMember(Value = "DRYER")]
        [Description("Dryer")]
        Dryer,
        [EnumMember(Value = "EFRECIR")]
        [Description("Exhaust Fan Recirculated")]
        ExhaustFanRecirculated,
        [EnumMember(Value = "EFVNTED")]
        [Description("Exhaust Fan Vented")]
        ExhaustFanVented,
        [EnumMember(Value = "ICEM")]
        [Description("Ice Maker")]
        IceMaker,
        [EnumMember(Value = "ICEMC")]
        [Description("Ice Maker Connection")]
        IceMakerConnection,
        [EnumMember(Value = "INDOORGRILL")]
        [Description("Indoor Grill")]
        IndoorGrill,
        [EnumMember(Value = "IHOTWTR")]
        [Description("Instant Hot Water")]
        InstantHotWater,
        [EnumMember(Value = "MICRO")]
        [Description("Microwave")]
        Microwave,
        [EnumMember(Value = "OVENCON")]
        [Description("Oven-Convection")]
        OvenConvection,
        [EnumMember(Value = "DOUBL")]
        [Description("Oven-Double")]
        OvenDouble,
        [EnumMember(Value = "OVENSNGL")]
        [Description("Oven-Single")]
        OvenSingle,
        [EnumMember(Value = "ELETR")]
        [Description("Range-Electric")]
        RangeElectric,
        [EnumMember(Value = "GASRA")]
        [Description("Range-Gas")]
        RangeGas,
        [EnumMember(Value = "REFZEROTYPE")]
        [Description("Refigerator Sub-Zero Type R")]
        RefigeratorSubZeroTypeR,
        [EnumMember(Value = "REFRI")]
        [Description("Refrigerator")]
        Refrigerator,
        [EnumMember(Value = "REFWINE")]
        [Description("Refrigerator - Wine")]
        RefrigeratorWine,
        [EnumMember(Value = "WASHE")]
        [Description("Washer")]
        Washer,
        [EnumMember(Value = "WHTNKLS")]
        [Description("Water Heater -Tankless")]
        WaterHeaterTankless,
        [EnumMember(Value = "WTRHEATER")]
        [Description("Water Heater 1 Unit")]
        WaterHeaterOneUnit,
        [EnumMember(Value = "WH2PLUSUNIT")]
        [Description("Water Heater 2+ Units")]
        WaterHeaterTwoUnits,
        [EnumMember(Value = "WHELECT")]
        [Description("Water Heater Electric")]
        WaterHeaterElectric,
        [EnumMember(Value = "WHGAS")]
        [Description("Water Heater Gas")]
        WaterHeaterGas,
        [EnumMember(Value = "WHPROPN")]
        [Description("Water Heater Propane/Butane")]
        WaterHeaterPropaneButane,
        [EnumMember(Value = "WHSOLAR")]
        [Description("Water Heater Solar")]
        WaterHeaterSolar,
        [EnumMember(Value = "WHSCOM")]
        [Description("Water Heater Solar Combo")]
        WaterHeaterSolarCombo,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
