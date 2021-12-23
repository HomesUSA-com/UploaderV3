namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    using Husa.Cargador.ViewModels.Enum;
    using System;

    public class PropertyTab
    {
        public DateTime? ConstCompletionDate { get; set; }

        public string LakeName { get; set; }

        public int? ConstructionStartYear { get; set; }

        public ConstructionStageEnum ConstructionStage { get; set; }

        public string MasterPlannedCommunity { get; set; }

        public string TaxId { get; set; }

        public decimal? Acres { get; set; }

        public string Block { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string Lot { get; set; }

        public string LotDescription { get; set; }

        public string LotDimensions { get; set; }

        public string LotSizeAcreage { get; set; }

        public string OwnerName { get; set; }

        public string PreDirection { get; set; }

        public string State { get; set; }

        public string StreetName { get; set; }

        public string StreetNum { get; set; }

        public string StreetType { get; set; }

        public string Subdivision { get; set; }

        public string UnitNumber { get; set; }

        public string ZipCode { get; set; }
    }
}
