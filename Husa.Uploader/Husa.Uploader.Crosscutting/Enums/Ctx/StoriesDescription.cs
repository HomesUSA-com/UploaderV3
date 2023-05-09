namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoriesDescription
    {
        [EnumMember(Value = "ONE")]
        [Description("One")]
        One,
        [EnumMember(Value = "TWO")]
        [Description("Two")]
        Two,
        [EnumMember(Value = "THREE")]
        [Description("Three+")]
        Three,
        [EnumMember(Value = "SPLIT")]
        [Description("Split/Multi Level")]
        SplitMultiLevel,
    }
}
