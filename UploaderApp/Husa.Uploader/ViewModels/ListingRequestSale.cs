namespace Husa.Cargador.ViewModels
{
    using Husa.Cargador.ViewModels.Enum;
    using Husa.Cargador.ViewModels.ListingRequestSaleSubClass;
    using System;

    public class ListingRequestSale
    {
        public string id { get; set; }

        public SaleProperty SaleProperty { get; set; }

        public Guid ListingSaleId { get; set; }

        public string ContingencyInfo { get; set; }

        public DateTime? ContractDate { get; set; }

        public DateTime? ExpiredDateOption { get; set; }

        public string Financing { get; set; }

        public string KickOutInformation { get; set; }

        public string MortgageCompany { get; set; }

        public string SellerPaid { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public ListingRequestState ListingRequestState { get; set; }

        public decimal? ListPrice { get; set; }

        public string MlsArea { get; set; }

        public string MlsSubArea { get; set; }

        public string MlsNumber { get; set; }

        public MarketStatusesEnum MlsStatus { get; set; }

        #region Status Fields 

        public Guid? AgentID { get; set; }

        public DateTime? BackOnMarketDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public string CancelledOption { get; set; } // CancelledOptionsEnum

        public string CancelledReason { get; set; }

        public DateTime? ClosedDate { get; set; }

        public decimal? ClosePrice { get; set; }

        public DateTime? EstimatedClosedDate { get; set; }

        public DateTime? OffMarketDate { get; set; }

        public DateTime? PendingDate { get; set; }

        #endregion

        //Entity Base Data
        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public char SysState { get; set; }
    }
}
