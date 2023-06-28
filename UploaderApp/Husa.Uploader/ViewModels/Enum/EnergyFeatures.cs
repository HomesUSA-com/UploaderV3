using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnergyFeatures
    {
        [EnumMember(Value = "TWH")]
        [Description("Tankless Water Heater")]
        TanklessWaterHeater,
        [EnumMember(Value = "CFANS")]
        [Description("Ceiling Fans")]
        CeilingFans,
        [EnumMember(Value = "ESAPP")]
        [Description("Energy Star Appliances")]
        EnergyStarAppliances,
        [EnumMember(Value = "SEERAC")]
        [Description("16+ SEER AC")]
        SixteenPlusSeerAC,
        [EnumMember(Value = "EFURN")]
        [Description("90% Efficient Furnace")]
        NinetyPercentEfficientFurnace,
        [EnumMember(Value = "DPWIN")]
        [Description("Double Pane Windows")]
        DoublePaneWindows,
        [EnumMember(Value = "SEM")]
        [Description("Smart Electric Meter")]
        SmartElectricMeter,
        [EnumMember(Value = "12+AI")]
        [Description("12\"+ Attic Insulation")]
        TwelvePlusAtticInsulation,
        [EnumMember(Value = "STWIN")]
        [Description("Storm Windows")]
        StormWindows,
        [EnumMember(Value = "SEERAX")]
        [Description("13-15 SEER AX")]
        ThirteenSeerAx,
        [EnumMember(Value = "RADBAR")]
        [Description("Radiant Barrier")]
        RadiantBarrier,
        [EnumMember(Value = "VSH")]
        [Description("Variable Speed HVAC")]
        VariableSpeedHVAC,
        [EnumMember(Value = "CELLI")]
        [Description("Cellulose Insulation")]
        CelluloseInsulation,
        [EnumMember(Value = "FOAMI")]
        [Description("Foam Insulation")]
        FoamInsulation,
        [EnumMember(Value = "LOEWIN")]
        [Description("Low E Windows")]
        LowEmissivityWindows,
        [EnumMember(Value = "PTHRM")]
        [Description("Programmable Thermostat")]
        ProgrammableThermostat,
        [EnumMember(Value = "HEWH")]
        [Description("High Efficiency Water Heater")]
        HighEfficiencyWaterHeater,
        [EnumMember(Value = "RHOTW")]
        [Description("Recirculating Hot Water")]
        RecirculatingHotWater,
    }
}
