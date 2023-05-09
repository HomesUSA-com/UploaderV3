namespace Husa.Uploader.Crosscutting.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConstructionStageEnum
    {
        [EnumMember(Value = "NCC")]
        Complete,
        [EnumMember(Value = "NCI")]
        Incomplete,
        [EnumMember(Value = "PREOWN")]
        Preowned,
        [EnumMember(Value = "PROPOS")]
        Proposed,
        [EnumMember(Value = "UNKNOW")]
        Unknown
    }
}
