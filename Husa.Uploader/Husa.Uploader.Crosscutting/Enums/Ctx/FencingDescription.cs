namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FencingDescription
    {
        [EnumMember(Value = "BACKYARD")]
        [Description("Back Yard")]
        BackYard,
        [EnumMember(Value = "Brick")]
        [Description("Brick")]
        Brick,
        [EnumMember(Value = "CEDAR")]
        [Description("Cedar")]
        Cedar,
        [EnumMember(Value = "ELECTRC")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "FULL")]
        [Description("Full")]
        Full,
        [EnumMember(Value = "PRIVA")]
        [Description("Privacy")]
        Privacy,
        [EnumMember(Value = "STONE")]
        [Description("Stone")]
        Stone,
        [EnumMember(Value = "VINYL")]
        [Description("Vinyl")]
        Vinyl,
        [EnumMember(Value = "WOODF")]
        [Description("Wood")]
        Wood,
        [EnumMember(Value = "WROUG")]
        [Description("Wrought Iron")]
        WroughtIron,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
