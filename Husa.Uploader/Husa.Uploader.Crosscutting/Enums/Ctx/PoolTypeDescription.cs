namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PoolTypeDescription
    {
        [EnumMember(Value = "COMM")]
        [Description("Community")]
        Community,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None = 1,
    }
}
