namespace Husa.Uploader.ViewModels.Enum
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GarageDescription
    {
        [EnumMember(Value = "NONE")]
        NotApplicable,
        [EnumMember(Value = "1GAR")]
        OneCarGarage,
        [EnumMember(Value = "2GAR")]
        TwoCarGarage,
        [EnumMember(Value = "3GAR")]
        ThreeCarGarage,
        [EnumMember(Value = "4+GAR")]
        FourPlusCarGarage,
        [EnumMember(Value = "ATT")]
        Attached,
        [EnumMember(Value = "DTCHD")]
        Detached,
        [EnumMember(Value = "OVRSZ")]
        Oversized,
        [EnumMember(Value = "REAR")]
        RearEntry,
        [EnumMember(Value = "SIDE")]
        SideEntry,
        [EnumMember(Value = "TANDEM")]
        Tandem
    }
}