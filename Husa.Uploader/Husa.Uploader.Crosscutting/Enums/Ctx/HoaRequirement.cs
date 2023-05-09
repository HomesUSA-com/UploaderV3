namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HOARequirement
    {
        [Description("Mandatory")]
        [EnumMember(Value = "MAND")]
        Mandatory,
        [Description("Voluntary")]
        [EnumMember(Value = "VOLNT")]
        Voluntary,
        [Description("None")]
        [EnumMember(Value = "NONE")]
        None,
    }
}
