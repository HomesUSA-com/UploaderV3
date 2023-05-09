namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoundationDescription
    {
        [EnumMember(Value = "COMBINATION")]
        [Description("Combination")]
        Combination,
        [EnumMember(Value = "PERMANENT")]
        [Description("Permanent")]
        Permanent,
        [EnumMember(Value = "SLAB")]
        [Description("Slab")]
        Slab,
    }
}
