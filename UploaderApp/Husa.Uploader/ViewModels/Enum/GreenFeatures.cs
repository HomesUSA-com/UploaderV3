using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GreenFeatures
    {
        [EnumMember(Value = "DTP")]
        [Description("Drought Tolerant Plants")]
        DroughtTolerantPlants,
        [EnumMember(Value = "EAF")]
        [Description("Enhanced Air Filtration")]
        EnhancedAirFiltration,
        [EnumMember(Value = "EFIC")]
        [Description("EF Irrigation Control")]
        EFIrrigationControl,
        [EnumMember(Value = "ERV")]
        [Description("Energy Recovery Ventilator")]
        EnergyRecoveryVentilator,
        [EnumMember(Value = "LFC")]
        [Description("Low Flow Commode")]
        LowFlowCommode,
        [EnumMember(Value = "LFF")]
        [Description("Low Flow Fixture")]
        LowFlowFixture,
        [EnumMember(Value = "MFA")]
        [Description("Mechanical Fresh Air")]
        MechanicalFreshAir,
        [EnumMember(Value = "RFS")]
        [Description("Rain/Freeze Sensors")]
        RainFreezeSensors,
        [EnumMember(Value = "RWC")]
        [Description("Rain Water Catchment")]
        RainWaterCatchment,
    }
}
