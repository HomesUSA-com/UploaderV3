namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public enum YearBuiltSource
    {
        [EnumMember(Value = "APPRD")]
        [Description("Appraisal District")]
        AppraisalDistrict,
        [EnumMember(Value = "APPRA")]
        [Description("Appraiser")]
        Appraiser,
        [EnumMember(Value = "BUILDER")]
        [Description("Builder")]
        Builder,
        [EnumMember(Value = "ESTIMATED")]
        [Description("Estimated")]
        Estimated,
        [EnumMember(Value = "OWNSE")]
        [Description("Owner/Seller")]
        OwnerSeller,
        [EnumMember(Value = "PUBLI")]
        [Description("Public Records")]
        PublicRecords,
        [EnumMember(Value = "UNKNO")]
        [Description("Unknown")]
        Unknown,
        [EnumMember(Value = "OTH")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
