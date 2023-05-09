namespace Husa.Uploader.Data.QuicklisterEntities.Sabor.ValueObjects
{
    using System;

    public class FinancialInfo
    {
        public int? TaxYear { get; set; }

        public bool HasMultipleHOA { get; set; }

        public int? NumHOA { get; set; }

        public bool HasAgentBonus { get; set; }

        public bool HasBonusWithAmount { get; set; }

        public decimal? AgentBonusAmount { get; set; }

        public DateTime? BonusExpirationDate { get; set; }

        public bool HasBuyerIncentive { get; set; }

        public bool IsMultipleTaxed { get; set; }

        public decimal? TaxRate { get; set; }

        public string TitleCompany { get; set; }

        public string ProposedTerms { get; set; }

        public string HOARequirement { get; set; }

        public string BuyersAgentCommission { get; set; }
    }
}
