namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using System.Collections.Generic;
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record FinancialRecord
    {
        public ICollection<AcceptableFinancingDescription> ProposedTerms { get; set; }

        public int? TaxYear { get; set; }

        public decimal? TaxRate { get; set; }

        public HOARequirement HoaRequirement { get; set; }

        public string TitleCompany { get; set; }

        public string HoaName { get; set; }

        public decimal? HoaTransferFeeAmount { get; set; }

        public string HoaWebsite { get; set; }

        public string HoaPhone { get; set; }

        public string HoaMgmtCo { get; set; }

        public HOATermType? HoaTerm { get; set; }

        public ICollection<HOAFeeIncludesDescription> HoaFeeIncludes { get; set; }

        public virtual decimal? HoaAmount { get; set; }
    }
}
