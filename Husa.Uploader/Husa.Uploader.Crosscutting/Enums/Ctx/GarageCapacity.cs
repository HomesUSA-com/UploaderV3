namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GarageCapacity
    {
        [EnumMember(Value = "ONE")]
        [Description("1")]
        One,
        [EnumMember(Value = "TWO")]
        [Description("2")]
        Two,
        [EnumMember(Value = "THREEPLUS")]
        [Description("3+")]
        Three,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
