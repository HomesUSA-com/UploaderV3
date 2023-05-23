using Husa.Uploader.ViewModels.Enum;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;

namespace Husa.Uploader.ViewModels.ListingRequestSaleSubClass
{
    public class FeaturesInfo
    {
        public string PropertyDescription { get; set; }

        public int? Fireplaces { get; set; }

        public ICollection<FireplaceDescription> FireplaceDescription { get; set; }

        public ICollection<WindowCoverings> WindowCoverings { get; set; }

        public bool HasAccessibility { get; set; }

        public ICollection<Accessibility> Accessibility { get; set; }

        public ICollection<HousingStyle> HousingStyle { get; set; }

        public ICollection<Exterior> Exterior { get; set; }

        public bool HasPrivatePool { get; set; }

        public ICollection<PrivatePool> PrivatePool { get; set; }

        public ICollection<HomeFaces> HomeFaces { get; set; }

        //public string NeighborhoodAmenities { get; set; }

        public ICollection<LotImprovements> LotImprovements { get; set; }

        public ICollection<Inclusions> Inclusions { get; set; }

        public ICollection<Floors> Floors { get; set; }

        public ICollection<ExteriorFeatures> ExteriorFeatures { get; set; }

        public ICollection<RoofDescription> RoofDescription { get; set; }

        public ICollection<Foundation> Foundation { get; set; }

        public ICollection<HeatingSystem> HeatSystem { get; set; }

        public ICollection<CoolingSystem> CoolingSystem { get; set; }

        public ICollection<GreenCertification> GreenCertification { get; set; }

        public ICollection<EnergyFeatures> EnergyFeatures { get; set; }

        public ICollection<GreenFeatures> GreenFeatures { get; set; }

        public ICollection<WaterSewer> WaterSewer { get; set; }

        public string SupplierElectricity { get; set; }

        public string SupplierWater { get; set; }

        public string SupplierSewer { get; set; }

        public string SupplierGarbage { get; set; }

        public string SupplierGas { get; set; }

        public string SupplierOther { get; set; }

        public ICollection<HeatingFuel> HeatingFuel { get; set; }

        public bool IsNewConstruction { get; set; }
    }
}
