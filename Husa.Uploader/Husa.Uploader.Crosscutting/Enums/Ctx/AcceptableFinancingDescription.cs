namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AcceptableFinancingDescription
    {
        [EnumMember(Value = "CASH")]
        [Description("Cash")]
        Cash,
        [EnumMember(Value = "CONVE")]
        [Description("Conventional")]
        Conventional,
        [EnumMember(Value = "FHA")]
        [Description("FHA")]
        FHA,
        [EnumMember(Value = "TEXAS")]
        [Description("Texas Vet")]
        TexasVet,
        [EnumMember(Value = "USDA")]
        [Description("USDA")]
        USDA,
        [EnumMember(Value = "VA")]
        [Description("VA")]
        VA,
    }
}
