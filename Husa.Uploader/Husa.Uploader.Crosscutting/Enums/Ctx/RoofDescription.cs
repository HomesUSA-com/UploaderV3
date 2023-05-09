namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoofDescription
    {
        [EnumMember(Value = "FIBARCEMENT")]
        [Description("Fiber Cement")]
        FiberCement,
        [EnumMember(Value = "FLAT")]
        [Description("Flat")]
        Flat,
        [EnumMember(Value = "OVERLAY")]
        [Description("Overlay")]
        Overlay,
        [EnumMember(Value = "SHNGC")]
        [Description("Shingle-Composition")]
        ShingleComposition,
        [EnumMember(Value = "SLATE")]
        [Description("Slate/Imitation Slate")]
        SlateImitationSlate,
        [EnumMember(Value = "TILE")]
        [Description("Tile")]
        Tile,
    }
}
