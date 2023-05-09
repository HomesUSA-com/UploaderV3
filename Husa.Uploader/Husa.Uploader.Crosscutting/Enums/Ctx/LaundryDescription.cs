namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LaundryDescription
    {
        [EnumMember(Value = "CLOSE")]
        [Description("Closet")]
        Closet,
        [EnumMember(Value = "DRYER")]
        [Description("Dryer Connection Elec")]
        DryerConnectionElec,
        [EnumMember(Value = "DRYERGAS")]
        [Description("Dryer Connection Gas")]
        DryerConnectionGas,
        [EnumMember(Value = "ELECT")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "GAS")]
        [Description("Gas")]
        Gas,
        [EnumMember(Value = "INKIT")]
        [Description("In Kitchen")]
        InKitchen,
        [EnumMember(Value = "INSID")]
        [Description("Inside")]
        Inside,
        [EnumMember(Value = "LAUND")]
        [Description("Laundry Room")]
        LaundryRoom,
        [EnumMember(Value = "LOWER")]
        [Description("Lower Level")]
        LowerLevel,
        [EnumMember(Value = "MAINL")]
        [Description("Main Level")]
        MainLevel,
        [EnumMember(Value = "Sink")]
        [Description("Sink")]
        Sink,
        [EnumMember(Value = "STACKABLE")]
        [Description("Stackable Only")]
        StackableOnly,
        [EnumMember(Value = "UPPER")]
        [Description("Upper Level")]
        UpperLevel,
        [EnumMember(Value = "Utility_Laundry")]
        [Description("Utility/Laundry Room")]
        UtilityLaundryRoom,
        [EnumMember(Value = "WASHE")]
        [Description("Washer Connection")]
        WasherConnection,
    }
}
