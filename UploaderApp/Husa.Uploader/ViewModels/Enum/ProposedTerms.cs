using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProposedTerms
    {
        [EnumMember(Value = "CASH")]
        [Description("Cash")]
        Cash,
        [EnumMember(Value = "CONV")]
        [Description("Conventional")]
        Conventional,
        [EnumMember(Value = "FHA")]
        [Description("FHA")]
        FHA,
        [EnumMember(Value = "INVOK")]
        [Description("Investors OK")]
        InvestorsOK,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "TXVET")]
        [Description("TX Vet")]
        TxVet,
        [EnumMember(Value = "USDA")]
        [Description("USDA")]
        USDA,
        [EnumMember(Value = "VA")]
        [Description("VA")]
        VA,
    }
}
