namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AirQualityDescription
    {
        [EnumMember(Value = "CONTMIN")]
        [Description("Contaminant Control")]
        ContaminantControl,
        [EnumMember(Value = "PESTMGMT")]
        [Description("Integrated Pest Management")]
        IntegratedPestManagement,
        [EnumMember(Value = "MOISTUR")]
        [Description("Moisture Control")]
        MoistureControl,
        [EnumMember(Value = "VENTILATION")]
        [Description("Ventilation")]
        Ventilation,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
