namespace Husa.Uploader.Data.QuicklisterEntities.Ctx
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Enums.Ctx;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.QuicklisterEntities.Ctx.Records;
    using Husa.Uploader.Data.QuicklisterEntities.Sabor;
    using Newtonsoft.Json;
    using System;

    public class CtxListingRequest : IConvertToUploaderRequest
    {
        [JsonProperty(PropertyName = "id")]
        public virtual Guid Id { get; set; }

        public virtual DateTime SysCreatedOn { get; set; }

        public virtual DateTime? SysModifiedOn { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual Guid? SysModifiedBy { get; set; }

        public virtual Guid? SysCreatedBy { get; set; }

        public virtual DateTime SysTimestamp { get; set; }

        public virtual Guid CompanyId { get; set; }

        public virtual int? CDOM { get; set; }

        public virtual int? DOM { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }

        public virtual DateTime? ListDate { get; set; }

        public virtual decimal ListPrice { get; set; }

        public virtual DateTime? MarketModifiedOn { get; }

        public virtual string MarketUniqueId { get; }

        public virtual string MlsNumber { get; set; }

        public virtual Guid ListingSaleId { get; set; }

        public virtual MarketStatuses MlsStatus { get; set; }

        public virtual ListingRequestState RequestState { get; set; }

        public virtual SalePropertyRecord SaleProperty { get; set; }

        public virtual StatusFieldsRecord StatusFieldsInfo { get; set; }

        public virtual PublishFieldsRecord PublishInfo { get; set; }

        //TODO: Complete the transformation for CTX
        public ResidentialListingRequest ConvertFromCosmos(string marketName, string marketUser, string marketPassword)
        {
            var residentialListingRequest = new ResidentialListingRequest
            {
                ResidentialListingRequestID = ListingSaleId,
                MarketUsername = marketUser,
                MarketPassword = marketPassword,
                MarketID = 8,
                MarketName = marketName,
                MarketCode = MarketCode.CTX,
                ListPrice = ListPrice,
                MLSNum = ""/*this.MlsNumber*/,
                MlsStatus = MlsStatus.ToString(),
                SysCreatedOn = SysCreatedOn,
                SysCreatedBy = SysCreatedBy,
                SysModifiedOn = SysModifiedOn,
                SysModifiedBy = SysModifiedBy,
                BuilderName = SaleProperty.OwnerName,
                CompanyName = SaleProperty.OwnerName,
                OwnerName = SaleProperty.OwnerName,
                PlanProfileID = SaleProperty.PlanId,
                CommunityProfileID = SaleProperty.CommunityId,
                StreetNum = SaleProperty.AddressInfo.StreetNumber,
                StreetName = SaleProperty.AddressInfo.StreetName,
                CityCode = SaleProperty.AddressInfo.City.ToString(),
                State = SaleProperty.AddressInfo.State.ToString(),
                Zip = SaleProperty.AddressInfo.ZipCode,
                County = SaleProperty.AddressInfo.County.ToString(),
                LotNum = SaleProperty.AddressInfo.LotNum,
                Block = SaleProperty.AddressInfo.Block,
                Subdivision = SaleProperty.AddressInfo.Subdivision,
                BuildCompletionDate = SaleProperty.PropertyInfo.ConstructionCompletionDate,// check
                YearBuiltDesc = SaleProperty.PropertyInfo.ConstructionStage.ToString(),
                YearBuilt = SaleProperty.PropertyInfo.ConstructionStartYear,
                //ConstructionStage
                //ConstructionYear
                Legal = SaleProperty.PropertyInfo.LegalDescription, //check
                TaxID = SaleProperty.PropertyInfo.TaxId,
                //MLSArea = SaleProperty.PropertyInfo.MlsArea,
                //MapscoMapBook = SaleProperty.PropertyInfo.MapscoGrid, // check
                //LotDim = SaleProperty.PropertyInfo.LotDimension,
                //LotSize = SaleProperty.PropertyInfo.LotSize,
                //LotDesc = SaleProperty.PropertyInfo.LotDescription,
                Occupancy = SaleProperty.PropertyInfo.Occupancy,
                //UpdateGeoCodes
                Latitude = SaleProperty.PropertyInfo.Latitude,
                Longitude = SaleProperty.PropertyInfo.Longitude,
                //isBxlManaged
                //Category = SaleProperty.SpacesDimensionsInfo.TypeCategory, //check
                //NumStories = SaleProperty.SpacesDimensionsInfo.Stories,
                //SqFtTotal = SaleProperty.SpacesDimensionsInfo.SqFtTotal,
                //SqFtSource = SaleProperty.SpacesDimensionsInfo.SqFtSource,
                //EntryLength
                //EntryWidth
                //SpecialtyRooms
                //MasterBedrrom
                //numBedrooms
                BathsFull = SaleProperty.SpacesDimensionsInfo.BathsFull,
                BathsHalf = SaleProperty.SpacesDimensionsInfo.BathsHalf,
                //masterBathDescription
                //GarageDesc = this.SaleProperty.SpacesDimensionsInfo.GarageDescription.ToString(),
                //ParkingDesc = this.SaleProperty.SpacesDimensionsInfo.GarageDescription.ToString(),
                //OtherParking = SaleProperty.SpacesDimensionsInfo.OtherParking,
                Beds = SaleProperty.SpacesDimensionsInfo.NumBedrooms,
                //PropertyDescription
                //InclusionsDesc = SaleProperty.FeaturesInfo.Inclusions,
                //NumFireplaces = SaleProperty.FeaturesInfo.Fireplaces,
                //FireplaceDesc = SaleProperty.FeaturesInfo.FireplaceDescription.ToStringFromEnumMembers(),
                //FloorsDesc = SaleProperty.FeaturesInfo.Floors,
                //WindowCoverings = SaleProperty.FeaturesInfo.WindowCoverings,
                //HasAccessibility
                //AccessibilityDesc = SaleProperty.FeaturesInfo.Accessibility,
                //HousingStyleDesc = SaleProperty.FeaturesInfo.HousingStyle,
                //ExteriorFeatures = SaleProperty.FeaturesInfo.ExteriorFeatures,
                //RoofDesc = SaleProperty.FeaturesInfo.RoofDescription,
                //FoundationDesc = SaleProperty.FeaturesInfo.Foundation,
                //ExteriorDesc = SaleProperty.FeaturesInfo.Exterior,
                //HasPool = SaleProperty.FeaturesInfo.HasPrivatePool,
                //PoolDesc = SaleProperty.FeaturesInfo.PrivatePool,
                //FacesDesc = SaleProperty.FeaturesInfo.HomeFaces, //check
                //SupElectricity = SaleProperty.FeaturesInfo.SupplierElectricity,
                //SupWater = SaleProperty.FeaturesInfo.SupplierWater,
                //SupGarbage = SaleProperty.FeaturesInfo.SupplierGarbage,
                //SupGas = SaleProperty.FeaturesInfo.SupplierGas,
                //SupSewer = SaleProperty.FeaturesInfo.SupplierSewer,
                //SupOther = SaleProperty.FeaturesInfo.SupplierOther,
                //HeatSystemDesc = SaleProperty.FeaturesInfo.HeatSystem,
                //CoolSystemDesc = SaleProperty.FeaturesInfo.CoolingSystem,
                //HeatingFuel = SaleProperty.FeaturesInfo.HeatingFuel,
                //WaterAccessDesc = SaleProperty.FeaturesInfo.WaterSewer, //check
                //GreenCerts = SaleProperty.FeaturesInfo.GreenCertification,
                //EnergyDesc = SaleProperty.FeaturesInfo.EnergyFeatures, //check
                //GreenFeatures = SaleProperty.FeaturesInfo.GreenFeatures,
                //CommonFeatures = this.SaleProperty.FeaturesInfo.NeighborhoodAmenities, //check
                //lotImprovements
                TaxRate = SaleProperty.FinancialInfo.TaxRate,
                TaxYear = SaleProperty.FinancialInfo.TaxYear,
                //IsMultiParcel = SaleProperty.FinancialInfo.IsMultipleTaxed.ToString(), //check
                TitleCo = SaleProperty.FinancialInfo.TitleCompany,
                //PROPSDTRMS = SaleProperty.FinancialInfo.ProposedTerms,
                //HasMultipleHOA = SaleProperty.FinancialInfo.HasMultipleHOA.ToString(),
                //AgentBonusAmount = SaleProperty.FinancialInfo.AgentBonusAmount.ToString(),
                //CompBuyBonusExpireDate = SaleProperty.FinancialInfo.BonusExpirationDate, //check
                //BuyerIncentive = SaleProperty.FinancialInfo.HasBuyerIncentive.ToString(), //check
                //CompBuy = SaleProperty.FinancialInfo.BuyersAgentCommission,
                //AltPhoneCommunity = SaleProperty.ShowingInfo.AltPhoneCommunity,
                //AgentListApptPhone = SaleProperty.ShowingInfo.AgentListApptPhone,
                //Showing = SaleProperty.ShowingInfo.Showing,
                //RealtorContactEmail = SaleProperty.ShowingInfo.RealtorContactEmail,
                Directions = SaleProperty.ShowingInfo.Directions,
                AgentPrivateRemarks = SaleProperty.ShowingInfo.AgentPrivateRemarks,
                SchoolDistrict = SaleProperty.SchoolsInfo.SchoolDistrict.ToStringFromEnumMember(),
                SchoolName1 = SaleProperty.SchoolsInfo.ElementarySchool.ToStringFromEnumMember(),
                SchoolName2 = SaleProperty.SchoolsInfo.MiddleSchool.ToStringFromEnumMember(),
                SchoolName3 = SaleProperty.SchoolsInfo.HighSchool.ToStringFromEnumMember(),
            };

            foreach (var room in SaleProperty.Rooms)
            {
                var width = room.Width;
                var length = room.Length;
                var level = room.Level.ToStringFromEnumMember();
                switch (room.RoomType)
                {
                    case RoomType.MasterBedroom:
                        residentialListingRequest.Bed1Level = level;
                        residentialListingRequest.Bed1Length = length;
                        residentialListingRequest.Bed1Width = width;
                        //residentialListingRequest.Bed1Desc = SaleProperty.SpacesDimensionsInfo.MasterBedroom;
                        break;
                    case RoomType.Bedroom:
                        if (residentialListingRequest.Bed2Level == null)
                        {
                            residentialListingRequest.Bed2Level = level;
                            residentialListingRequest.Bed2Length = length;
                            residentialListingRequest.Bed2Width = width;
                            break;
                        }

                        if (residentialListingRequest.Bed3Level == null)
                        {
                            residentialListingRequest.Bed3Level = level;
                            residentialListingRequest.Bed3Length = length;
                            residentialListingRequest.Bed3Width = width;
                            break;
                        }

                        if (residentialListingRequest.Bed4Level == null)
                        {
                            residentialListingRequest.Bed4Level = level;
                            residentialListingRequest.Bed4Length = length;
                            residentialListingRequest.Bed4Width = width;
                            break;
                        }

                        if (residentialListingRequest.Bed5Level == null)
                        {
                            residentialListingRequest.Bed5Level = level;
                            residentialListingRequest.Bed5Length = length;
                            residentialListingRequest.Bed5Width = width;
                            break;
                        }
                        break;
                    case RoomType.Breakfast:
                        residentialListingRequest.BreakfastLevel = level;
                        residentialListingRequest.BreakfastLength = length;
                        residentialListingRequest.BreakfastWidth = width;
                        break;
                    case RoomType.Dining:
                        residentialListingRequest.DiningRoomLevel = level;
                        residentialListingRequest.BreakfastLength = length;
                        residentialListingRequest.DiningRoomWidth = width;
                        break;
                    case RoomType.EntryFoyer:
                        residentialListingRequest.LivingRoom3Level = level;
                        residentialListingRequest.LivingRoom3Length = length;
                        residentialListingRequest.LivingRoom3Width = width;
                        break;
                    case RoomType.Family:
                        residentialListingRequest.LivingRoom2Level = level;
                        residentialListingRequest.LivingRoom2Length = length;
                        residentialListingRequest.LivingRoom2Width = width;
                        break;
                    case RoomType.Game:
                        residentialListingRequest.OtherRoom1Level = level;
                        residentialListingRequest.OtherRoom1Length = length;
                        residentialListingRequest.OtherRoom1Width = width;
                        break;
                    case RoomType.Kitchen:
                        residentialListingRequest.KitchenLevel = level;
                        residentialListingRequest.KitchenLength = length;
                        residentialListingRequest.KitchenWidth = width;
                        break;
                    case RoomType.LivingRoom:
                        residentialListingRequest.LivingRoom1Level = level;
                        residentialListingRequest.LivingRoom1Length = length;
                        residentialListingRequest.LivingRoom1Width = width;
                        break;
                    case RoomType.MasterBath:
                        residentialListingRequest.Bath1Level = level;
                        residentialListingRequest.Bath1Length = length;
                        residentialListingRequest.Bath1Width = width;
                        //residentialListingRequest.BedBathDesc = SaleProperty.SpacesDimensionsInfo.MasterBathDescription;
                        break;
                    //case RoomType.MasterBedroomCloset:
                    //    residentialListingRequest.ClosetLength = length;
                    //    residentialListingRequest.ClosetWidth = width;
                    //    break;
                    case RoomType.MediaHomeTheatre:
                        residentialListingRequest.OtherRoom2Level = level;
                        residentialListingRequest.OtherRoom2Length = length;
                        residentialListingRequest.OtherRoom2Width = width;
                        break;
                    case RoomType.OfficeStudy:
                        residentialListingRequest.StudyLevel = level;
                        residentialListingRequest.StudyLength = length;
                        residentialListingRequest.StudyWidth = width;
                        break;
                    case RoomType.UtilityLaundry:
                        residentialListingRequest.UtilityRoomLevel = level;
                        residentialListingRequest.UtilityRoomLength = length;
                        residentialListingRequest.UtilityRoomWidth = width;
                        break;
                    //case RoomType.Studen:
                    //case RoomType.Other:
                    //case RoomType.Office:
                    //case RoomType.HalfBath:
                    //case RoomType.FullBath:
                    default:
                        break;

                }
            }

            foreach (var openHouse in SaleProperty.OpenHouses)
            {
                switch (openHouse.Type)
                {
                    case OpenHouseType.Saturday:
                        //residentialListingRequest.OHRefreshmentsSat = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeSat = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeSat = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsSat = openHouse.Comments;
                        break;
                    case OpenHouseType.Sunday:
                        //residentialListingRequest.OHRefreshmentsSun = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeSun = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeSun = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsSun = openHouse.Comments;
                        break;
                    case OpenHouseType.Monday:
                        //residentialListingRequest.OHRefreshmentsMon = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeMon = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeMon = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsMon = openHouse.Comments;
                        break;
                    case OpenHouseType.Tuesday:
                        //residentialListingRequest.OHRefreshmentsTue = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeTue = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeTue = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsTue = openHouse.Comments;
                        break;
                    case OpenHouseType.Wednesday:
                        //residentialListingRequest.OHRefreshmentsWed = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeWed = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeWed = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsWed = openHouse.Comments;
                        break;
                    case OpenHouseType.Thursday:
                        //residentialListingRequest.OHRefreshmentsThu = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeThu = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeThu = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsThu = openHouse.Comments;
                        break;
                    case OpenHouseType.Friday:
                        //residentialListingRequest.OHRefreshmentsFri = openHouse.Refreshments;
                        residentialListingRequest.OHStartTimeFri = openHouse.StartTime.ToString();
                        residentialListingRequest.OHEndTimeFri = openHouse.EndTime.ToString();
                        //residentialListingRequest.OHCommentsFri = openHouse.Comments;
                        break;
                    default: break;
                }
            }

            return residentialListingRequest;
        }
    }
}
