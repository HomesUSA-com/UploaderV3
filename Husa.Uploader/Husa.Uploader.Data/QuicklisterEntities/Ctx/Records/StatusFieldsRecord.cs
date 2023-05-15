namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Uploader.Crosscutting.Enums.Ctx;
    using System;

    public record StatusFieldsRecord
    {
        public DateTime? ContractDate { get; set; }

        public string SellConcess { get; set; }

        public SellerConcessionDescription? SellerConcessionDescription { get; set; }

        public decimal? ClosePrice { get; set; }

        public DateTime? EstimatedClosedDate { get; set; }

        public Guid? AgentId { get; set; }

        public bool? HasBuyerAgent { get; set; }

        public DateTime? BackOnMarketDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public DateTime? OffMarketDate { get; set; }

        public DateTime? WithdrawalDate { get; set; }

        public string WithdrawalReason { get; set; }

        public bool? WithdrawalActiveListingAgreement { get; set; }
    }
}
