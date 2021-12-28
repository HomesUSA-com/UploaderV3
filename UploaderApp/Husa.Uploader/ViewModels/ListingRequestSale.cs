namespace Husa.Cargador.ViewModels
{
    using Husa.Cargador.ViewModels.Enum;
    using Husa.Cargador.ViewModels.ListingRequestSaleSubClass;
    using System;

    public class ListingRequestSale
    {
        public string id { get; set; }

        public Guid ListingSaleId { get; set; }

        public SaleProperty SaleProperty { get; set; }

        public int? CDOM { get; set; }

        public int? DOM { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? ListDate { get; set; }

        public decimal? ListPrice { get; set; }

        public string ListType { get; protected set; }

        public DateTime? MarketModifiedOn { get; set; }

        public string MarketUniqueId { get; set; }

        public string MlsNumber { get; set; }

        public string MlsStatus { get; set; }

        public Guid PropertyId { get; set; }

        //Entity Base Data
        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public char SysState { get; set; }

        public ListingRequestState RequestState { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}
