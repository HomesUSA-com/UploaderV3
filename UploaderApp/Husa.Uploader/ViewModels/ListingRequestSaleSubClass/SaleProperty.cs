namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    using System;

    public class SaleProperty
    {
        public Guid SalePropertyId { get; set; }

        public Guid CompanyId { get; set; }

        public PropertyTab PropertyTab { get; set; }
        
        public SpaceAndDimensionsTab SpaceAndDimensionsTab { get; set; }
        
        public FeatureTab FeatureTab { get; set; }

        public FinancialSchoolTab FinancialSchoolTab { get; set; }

        public ShowingTab ShowingTab { get; set; }
    }
}
