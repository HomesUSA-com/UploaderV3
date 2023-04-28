using Husa.Uploader.ViewModels.Enum;
using System.Collections;
using System.Collections.Generic;

namespace Husa.Uploader.ViewModels.ListingRequestSaleSubClass
{
    public class FeaturesInfo
    {
        public string PropertyDescription { get; set; }

        public int? Fireplaces { get; set; }

        public ICollection<FireplaceDescription> FireplaceDescription { get; set; }

        public string WindowCoverings { get; set; }

        public bool HasAccessibility { get; set; }

        public string Accessibility { get; set; }

        public string HousingStyle { get; set; }

        public string Exterior { get; set; }

        public bool HasPrivatePool { get; set; }

        public string PrivatePool { get; set; }

        public string HomeFaces { get; set; }

        //public string NeighborhoodAmenities { get; set; }

        public string LotImprovements { get; set; }

        public string Inclusions { get; set; }

        public string Floors { get; set; }

        public string ExteriorFeatures { get; set; }

        public string RoofDescription { get; set; }

        public string Foundation { get; set; }

        public string HeatSystem { get; set; }

        public string CoolingSystem { get; set; }

        public string GreenCertification { get; set; }

        public string EnergyFeatures { get; set; }

        public string GreenFeatures { get; set; }

        public string WaterSewer { get; set; }

        public string SupplierElectricity { get; set; }

        public string SupplierWater { get; set; }

        public string SupplierSewer { get; set; }

        public string SupplierGarbage { get; set; }

        public string SupplierGas { get; set; }

        public string SupplierOther { get; set; }

        public string HeatingFuel { get; set; }

        public bool IsNewConstruction { get; set; }
    }
}
