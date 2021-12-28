namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    using System;
    using System.Collections.Generic;

    public class SaleProperty
    {
        public Guid SalePropertyId { get; set; }

        public string OwnerName { get; set; }

        public Guid? PlanId { get; set; }

        public Guid? CommunityId { get; set; }

        public Guid CompanyId { get; set; }

        public AddressInfo AddressInfo { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
        
        public SpacesDimensionsInfo SpacesDimensionsInfo { get; set; }
        
        public FeaturesInfo FeaturesInfo { get; set; }

        public FinancialInfo FinancialInfo { get; set; }

        public ShowingInfo ShowingInfo { get; set; }

        public SchoolsInfo SchoolsInfo { get; set; }

        public ICollection<Rooms> Rooms { get; set; }

        public ICollection<ListingSaleHoa> ListingSaleHoas { get; set; }

        public ICollection<ListingSaleOpenHouse> OpenHouses { get; set; }
    }
}
