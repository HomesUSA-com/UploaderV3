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

        public virtual decimal? BuyersAgentCommission { get; set; }

        public virtual CommissionType BuyersAgentCommissionType { get; set; }

        public virtual LockboxType? LockboxType { get; set; }

        public virtual ICollection<LockboxLocationDescription> LockboxLocation { get; set; }

        public virtual ICollection<ShowingInstructionsDescription> ShowingInstrcutions { get; set; }

        public virtual string Directions { get; set; }

        public virtual string PropertyDescription { get; set; }

        public virtual bool EnableOpenHouses { get; set; }

        public virtual bool OpenHousesAgree { get; set; }

        public virtual bool ShowOpenHousesPending { get; set; }

        public bool HasAgentBonus { get; set; }

        public bool HasBonusWithAmount { get; set; }

        public decimal? AgentBonusAmount { get; set; }

        public CommissionType? AgentBonusAmountType { get; set; }

        public DateTime? BonusExpirationDate { get; set; }

        public bool HasBuyerIncentive { get; set; }
    }
}
