namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using System.Collections.Generic;
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record FeaturesRecord
    {
        public ICollection<HousingStyleDescription> HousingStyle { get; set; }

        public ICollection<FoundationDescription> Foundation { get; set; }

        public StoriesDescription? Stories { get; set; }

        public ICollection<SpecialtyRoomsDescription> SpecialtyRooms { get; set; }

        public ICollection<RoofDescription> RoofDescription { get; set; }

        public ICollection<ExteriorDescription> ConstructionExterior { get; set; }

        public ICollection<FireplaceDescription> FireplaceDescription { get; set; }

        public ICollection<FloorsDescription> Floors { get; set; }

        public ICollection<PoolTypeDescription> Pool { get; set; }

        public ICollection<KitchenDescription> Kitchen { get; set; }

        public ICollection<LaundryDescription> Laundry { get; set; }

        public ICollection<MasterBedroomDescription> MasterBedroom { get; set; }

        public GarageCapacity? Garage { get; set; }

        public CarportCapacity? Carport { get; set; }

        public ICollection<GarageDescription> GarageDescription { get; set; }

        public ICollection<InclusionsDescription> Inclusions { get; set; }

        public ICollection<AppliancesEquipmentDescription> AppliancesEquipment { get; set; }

        public string LotDimension { get; set; }

        public string LotSize { get; set; }

        public ICollection<FencingDescription> Fencing { get; set; }

        public bool? WaterAccess { get; set; }

        public ICollection<WaterAccessTypeDescription> WaterAccessType { get; set; }

        public bool? WaterFront { get; set; }

        public ICollection<WaterDescription> WaterFeatures { get; set; }

        public bool? GatedCommunity { get; set; }

        public bool? SprinklerSystem { get; set; }

        public ICollection<SprinklerSystemDescription> SprinklerSystemDescription { get; set; }

        public ICollection<RestrictionsDescription> RestrictionsType { get; set; }

        public ICollection<ExteriorFeaturesDescription> ExteriorFeatures { get; set; }

        public ICollection<TopoLandDescription> TopoLandDescription { get; set; }

        public ICollection<NeighborhoodAmenitiesDescription> NeighborhoodAmenities { get; set; }

        public string LotImprovements { get; set; }

        public ICollection<HeatSystemDescription> HeatSystem { get; set; }

        public ICollection<CoolingSystemDescription> CoolingSystem { get; set; }

        public ICollection<GreenBuildingVerificationDescription> GreenBuildingVerification { get; set; }

        public ICollection<EnergyDescription> EnergyFeatures { get; set; }

        public ICollection<GreenVerificationSourceDescription> GreenVerificationSource { get; set; }

        public ICollection<WaterSewerDescription> WaterSewer { get; set; }
        public bool? IsNewConstruction { get; set; }

        public ICollection<AccessRoadSurfaceDescription> AccessRoadSurface { get; set; }

        public bool? UpgradedEnergyFeatures { get; set; }

        public bool? EESFeatures { get; set; }

        public ICollection<GreenBuildingVerificationDescription> GreenCertification { get; set; }

        public ICollection<AirQualityDescription> AirQuality { get; set; }

        public ICollection<WaterConservationDescription> WaterConservation { get; set; }

        public ICollection<SupplierOtherDescription> SupplierOther { get; set; }
    }
}
