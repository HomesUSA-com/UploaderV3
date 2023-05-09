namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnergyDescription
    {
        [EnumMember(Value = "APPLIANCES")]
        [Description("Appliances")]
        Appliances,
        [EnumMember(Value = "CONSTRUCTION")]
        [Description("Construction")]
        Construction,
        [EnumMember(Value = "DOORS")]
        [Description("Doors")]
        Doors,
        [EnumMember(Value = "SHADE")]
        [Description("Exposure/Shade")]
        ExposureShade,
        [EnumMember(Value = "HVAC")]
        [Description("HVAC")]
        HVAC,
        [EnumMember(Value = "INCENTIVE")]
        [Description("Incentives")]
        Incentives,
        [EnumMember(Value = "INSULATION")]
        [Description("Insulation")]
        Insulation,
        [EnumMember(Value = "LIGHTING")]
        [Description("Lighting")]
        Lighting,
        [EnumMember(Value = "ROOF")]
        [Description("Roof")]
        Roof,
        [EnumMember(Value = "THERMOSTAT")]
        [Description("Thermostat")]
        Thermostat,
        [EnumMember(Value = "WTRHEATER")]
        [Description("Water Heater")]
        WaterHeater,
        [EnumMember(Value = "WINDOW")]
        [Description("Windows")]
        Windows,
    }
}
