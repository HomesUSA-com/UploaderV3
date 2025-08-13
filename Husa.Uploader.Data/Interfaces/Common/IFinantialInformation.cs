namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IFinantialInformation
    {
        string ProposedTerms { get; }
        string Exemptions { get; }
        string TaxYear { get; }
        string TaxRate { get; }
        string HOA { get; }
        string AssocName { get; }
        int? AssocFee { get; }
        string ManagementCompany { get; }
        string AssocPhone { get; }
        string HoaWebsite { get; }
        int? AssocTransferFee { get; }
        string AssocFeeIncludes { get; }
        string AssocFeeFrequency { get; }
    }
}
