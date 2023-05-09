namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LockboxType
    {
        [EnumMember(Value = "CDEBX")]
        [Description("Codebox")]
        Codebox,
        [EnumMember(Value = "COMBO")]
        [Description("Combo")]
        Combo,
        [EnumMember(Value = "KEYOFFICE")]
        [Description("Key w/office")]
        KeyWoffice,
        [EnumMember(Value = "SUPRA")]
        [Description("Supra")]
        Supra,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
