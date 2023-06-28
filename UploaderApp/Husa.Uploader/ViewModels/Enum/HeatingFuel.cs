using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HeatingFuel
    {
        [EnumMember(Value = "ELEC")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "PPLNS")]
        [Description("Propane Leased")]
        PropaneLeased,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "NTGAS")]
        [Description("Natural Gas")]
        NaturalGas,
    }
}
