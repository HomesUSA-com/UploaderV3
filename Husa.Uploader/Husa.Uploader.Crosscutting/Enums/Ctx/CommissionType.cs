namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommissionType
    {
        [EnumMember(Value = "$")]
        [Description("$")]
        Amount,
        [EnumMember(Value = "%")]
        [Description("%")]
        Percent,
    }
}
