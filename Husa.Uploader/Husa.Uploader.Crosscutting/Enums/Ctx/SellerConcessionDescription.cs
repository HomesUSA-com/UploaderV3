namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SellerConcessionDescription
    {
        [EnumMember(Value = "CASH")]
        [Description("Cash")]
        Cash,
        [EnumMember(Value = "CONTR")]
        [Description("Contract for Deed")]
        ContractForDeed,
        [EnumMember(Value = "CONVE")]
        [Description("Conventional")]
        Conventional,
        [EnumMember(Value = "FHA")]
        [Description("FHA")]
        Fha,
        [EnumMember(Value = "HOUSV")]
        [Description("Housing Voucher")]
        HousingVoucher,
        [EnumMember(Value = "OTH")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "OWNFI")]
        [Description("Owner Financing")]
        OwnerFinancing,
        [EnumMember(Value = "VA")]
        [Description("VA")]
        Va,
    }
}
