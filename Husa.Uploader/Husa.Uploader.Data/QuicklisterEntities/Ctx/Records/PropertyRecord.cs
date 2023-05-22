namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using System;
    using System.Collections.Generic;
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record PropertyRecord
    {
        public DateTime? ConstructionCompletionDate { get; set; }

        public ConstructionStage ConstructionStage { get; set; }

        public int? ConstructionStartYear { get; set; }

        public YearBuiltSource YearBuiltSource { get; set; }

        public string LegalDescription { get; set; }

        public string TaxId { get; set; }

        public PropertySubType TypeCategory { get; set; }

        public FrontFaces? FrontFaces { get; set; }

        public ListingType ListingType { get; set; }

        public string Occupancy { get; set; }

        public bool? UpdateGeocodes { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int? Lot { get; set; }

        public bool? IsBxlManaged { get; set; }

        public int? SqFtTotal { get; set; }

        public string PropertyId { get; set; }

        public SqftSourceType? SqFtSource { get; set; }

        public ICollection<DocumentsAvailableDescription> DocumentsAvailable { get; set; }
    }
}
