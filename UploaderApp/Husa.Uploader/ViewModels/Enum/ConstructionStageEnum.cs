namespace Husa.Uploader.ViewModels.Enum
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConstructionStageEnum
    {
        [EnumMember(Value = "COMPL")]
        Complete,
        [EnumMember(Value = "INCOM")]
        Incomplete,
        [EnumMember(Value = "PREOWN")]
        Preowned,
        [EnumMember(Value = "PROPOS")]
        Proposed,
        [EnumMember(Value = "UNKNOW")]
        Unknown
    }
}
