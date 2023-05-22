namespace Husa.Uploader.Crosscutting.Enums
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConstructionStage
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
        Unknown,
    }
}
