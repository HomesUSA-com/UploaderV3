namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Uploader.Crosscutting.Enums.Ctx;
    using System;
    using System.Collections.Generic;

    public record ShowingRecord
    {
        public string AgentPrivateRemarks { get; set; }

        public string PublicRemarks { get; set; }

        public string ShowingPhone { get; set; }

        public string SecondShowingPhone { get; set; }

        public decimal? BuyersAgentCommission { get; set; }

        public CommissionType BuyersAgentCommissionType { get; set; }

        public LockboxType? LockboxType { get; set; }

        public ICollection<LockboxLocationDescription> LockboxLocation { get; set; }

        public ICollection<ShowingInstructionsDescription> ShowingInstrcutions { get; set; }

        public string Directions { get; set; }

        public string PropertyDescription { get; set; }

        public bool? EnableOpenHouses { get; set; }

        public bool? OpenHousesAgree { get; set; }

        public bool? ShowOpenHousesPending { get; set; }

        public bool? HasAgentBonus { get; set; }

        public bool? HasBonusWithAmount { get; set; }

        public decimal? AgentBonusAmount { get; set; }

        public CommissionType? AgentBonusAmountType { get; set; }

        public DateTime? BonusExpirationDate { get; set; }

        public bool? HasBuyerIncentive { get; set; }
    }
}
