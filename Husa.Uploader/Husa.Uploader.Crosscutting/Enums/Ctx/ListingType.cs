namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public enum ListingType
    {
        [EnumMember(Value = "AGENCY")]
        [Description("ExclusiveAgency")]
        ExclusiveAgency,
        [EnumMember(Value = "RIGHTTOLEASE")]
        [Description("ExclusiveRightToLease")]
        ExclusiveRightToLease,
        [EnumMember(Value = "RIGHTTOSELL")]
        [Description("ExclusiveRightToSell")]
        ExclusiveRightToSell,
        [EnumMember(Value = "EXCEPTION")]
        [Description("ExclusiveRightWException")]
        ExclusiveRightWException,
        [EnumMember(Value = "LIMITEDSERVC")]
        [Description("LimitedServiceListing")]
        LimitedServiceListing,
        [EnumMember(Value = "MLSONLY")]
        [Description("MLSOnly")]
        MLSOnly,
        [EnumMember(Value = "PROPMGMTAGREEMENT")]
        [Description("PropertyManagementAgreement")]
        PropertyManagementAgreement,
    }
}
