namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConstructionStage
    {
        [EnumMember(Value = "COMPL")]
        [Description("CompleteConstruction")]
        CompleteConstruction,
        [EnumMember(Value = "TOBEB")]
        [Description("ToBeBuilt")]
        ToBeBuilt,
        [EnumMember(Value = "UNDER")]
        [Description("UnderConstruction")]
        UnderConstruction,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
