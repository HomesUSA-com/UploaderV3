namespace Husa.Uploader.ViewModels.ListingRequestSaleSubClass
{
    using Husa.Uploader.ViewModels.Enum;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class PropertyInfo
    {
        public DateTime ConstructionCompletionDate { get; set; }

        public ConstructionStageEnum ConstructionStage { get; set; }

        public int? ConstructionStartYear { get; set; }

        public string LegalDescription { get; set; }

        public string TaxId { get; set; }

        public string MlsArea { get; set; }

        public string MapscoGrid { get; set; }

        public string LotDimension { get; set; }
        public string LotSize { get; set; }

        public ICollection<LotDescription> LotDescription { get; set; }

        public ICollection<Occupancy> Occupancy { get; set; }

        public bool UpdateGeocodes { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool IsBxlManaged { get; set; }
    }
}
