using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HousingStyle
    {
        [EnumMember(Value = "1STRY")]
        [Description("One Story")]
        OneStory,
        [EnumMember(Value = "2STRY")]
        [Description("Two Story")]
        TwoStory,
        [EnumMember(Value = "COLONIAL")]
        [Description("Colonial")]
        Colonial,
        [EnumMember(Value = "CONT")]
        [Description("Contemporary")]
        Contemporary,
        [EnumMember(Value = "CRAFTSMAN")]
        [Description("Craftsman")]
        Craftsman,
        [EnumMember(Value = "MDTRN")]
        [Description("Mediterranean")]
        Mediterranean,
        [EnumMember(Value = "OTHER")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "RANCH")]
        [Description("Ranch")]
        Ranch,
        [EnumMember(Value = "SPLIT")]
        [Description("Split Level")]
        SplitLevel,
        [EnumMember(Value = "SPNSH")]
        [Description("Spanish")]
        Spanish,
        [EnumMember(Value = "TEXAS")]
        [Description("Texas Hill Country")]
        TexasHillCountry,
        [EnumMember(Value = "TRDNL")]
        [Description("Traditional")]
        Traditional,
        [EnumMember(Value = "TUDOR")]
        [Description("Tudor")]
        Tudor,
    }
}
