namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShowingInstructionsDescription
    {
        [EnumMember(Value = "APPOI")]
        [Description("Appointment Only")]
        AppointmentOnly,
        [EnumMember(Value = "OFFICE")]
        [Description("Appointment w/Office")]
        AppointmentWOffice,
        [EnumMember(Value = "CallGo")]
        [Description("Call First-Go")]
        CallFirstGo,
        [EnumMember(Value = "Go")]
        [Description("Go")]
        Go,
        [EnumMember(Value = "ACCOMPANY")]
        [Description("Owner Must Accompany")]
        OwnerMustAccompany,
        [EnumMember(Value = "SHOWI")]
        [Description("Showing Service")]
        ShowingService,
        [EnumMember(Value = "SHOW2")]
        [Description("Showing Service App")]
        ShowingServiceApp,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
