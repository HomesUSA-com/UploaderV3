namespace Husa.Uploader.Support
{
    using Husa.Core.UploaderBase;
    using Husa.Uploader.ViewModels;
    using Husa.Uploader.ViewModels.Enum;
    using System.Collections.Generic;

    public class ClassTransform
    {
        public IEnumerable<ResidentialListingRequest> CosmoObjectToResidentialListingRequest(IEnumerable<ListingRequestSale> listingsRequestSale)
        {
            var residentialListingRequests = new List<ResidentialListingRequest>();

            foreach (var listingRequest in listingsRequestSale)
            {
                var residentialListingRequest = new ResidentialListingRequest();
                residentialListingRequest.ResidentialListingRequestID = listingRequest.ListingSaleId;
                residentialListingRequest.MarketUsername = "096651";
                residentialListingRequest.MarketPassword = /*"8Hw*whRhxD&rr"*/"r&v4RJDUv";
                residentialListingRequest.MarketID = 8;
                residentialListingRequest.MarketName = "San Antonio";
                residentialListingRequest.ListPrice = (decimal)listingRequest.ListPrice;
                residentialListingRequest.MLSNum = ""/*listingRequest.MlsNumber*/;
                residentialListingRequest.MlsStatus = listingRequest.MlsStatus.ToString();
                residentialListingRequest.SysCreatedOn = listingRequest.CreatedOn;
                residentialListingRequest.SysCreatedBy = listingRequest.CreatedBy;
                residentialListingRequest.SysModifiedOn = listingRequest.ModifiedOn;
                residentialListingRequest.SysModifiedBy = listingRequest.ModifiedBy;
                residentialListingRequest.BuilderName = listingRequest.SaleProperty.OwnerName;
                residentialListingRequest.CompanyName = listingRequest.SaleProperty.OwnerName;
                residentialListingRequest.OwnerName = listingRequest.SaleProperty.OwnerName;
                residentialListingRequest.PlanProfileID = listingRequest.SaleProperty.PlanId;
                residentialListingRequest.CommunityProfileID = listingRequest.SaleProperty.CommunityId;
                residentialListingRequest.ListStatus = listingRequest.MlsStatus;

                #region AddressInfo
                residentialListingRequest.StreetNum = listingRequest.SaleProperty.AddressInfo.StreetNumber.ToString();
                residentialListingRequest.StreetName = listingRequest.SaleProperty.AddressInfo.StreetName;
                residentialListingRequest.City = listingRequest.SaleProperty.AddressInfo.City;
                residentialListingRequest.State = listingRequest.SaleProperty.AddressInfo.State;
                residentialListingRequest.Zip = listingRequest.SaleProperty.AddressInfo.ZipCode;
                residentialListingRequest.County = listingRequest.SaleProperty.AddressInfo.County;
                residentialListingRequest.LotNum = listingRequest.SaleProperty.AddressInfo.LotNum;
                residentialListingRequest.Block = listingRequest.SaleProperty.AddressInfo.Block;
                residentialListingRequest.Subdivision = listingRequest.SaleProperty.AddressInfo.Subdivision;
                #endregion

                #region PropertyInfo
                residentialListingRequest.BuildCompletionDate = listingRequest.SaleProperty.PropertyInfo.ConstructionCompletionDate;// check
                residentialListingRequest.YearBuiltDesc = listingRequest.SaleProperty.PropertyInfo.ConstructionStage.ToString();
                residentialListingRequest.YearBuilt = listingRequest.SaleProperty.PropertyInfo.ConstructionStartYear;
                //ConstructionStage
                //ConstructionYear
                residentialListingRequest.Legal = listingRequest.SaleProperty.PropertyInfo.LegalDescription; //check
                residentialListingRequest.TaxID = listingRequest.SaleProperty.PropertyInfo.TaxId;
                residentialListingRequest.MLSArea = listingRequest.SaleProperty.PropertyInfo.MlsArea;
                residentialListingRequest.MapscoMapBook = listingRequest.SaleProperty.PropertyInfo.MapscoGrid; // check
                residentialListingRequest.LotDim = listingRequest.SaleProperty.PropertyInfo.LotDimension;
                residentialListingRequest.LotSize = listingRequest.SaleProperty.PropertyInfo.LotSize;
                residentialListingRequest.LotDesc = listingRequest.SaleProperty.PropertyInfo.LotDescription;
                residentialListingRequest.Occupancy = listingRequest.SaleProperty.PropertyInfo.Occupancy;
                //UpdateGeoCodes
                residentialListingRequest.Latitude = listingRequest.SaleProperty.PropertyInfo.Latitude;
                residentialListingRequest.Longitude = listingRequest.SaleProperty.PropertyInfo.Longitude;
                //isBxlManaged
                #endregion

                #region SpaceAndDimensionInfo
                residentialListingRequest.Category = listingRequest.SaleProperty.SpacesDimensionsInfo.TypeCategory; //check
                residentialListingRequest.NumStories = listingRequest.SaleProperty.SpacesDimensionsInfo.Stories;
                residentialListingRequest.SqFtTotal = listingRequest.SaleProperty.SpacesDimensionsInfo.SqFtTotal;
                residentialListingRequest.SqFtSource = listingRequest.SaleProperty.SpacesDimensionsInfo.SqFtSource;
                //EntryLength
                //EntryWidth
                //SpecialtyRooms
                //MasterBedrrom
                //numBedrooms
                residentialListingRequest.BathsFull = listingRequest.SaleProperty.SpacesDimensionsInfo.BathsFull;
                residentialListingRequest.BathsHalf = listingRequest.SaleProperty.SpacesDimensionsInfo.BathsHalf;
                //masterBathDescription
                residentialListingRequest.GarageDesc = listingRequest.SaleProperty.SpacesDimensionsInfo.GarageDescription;
                residentialListingRequest.ParkingDesc = listingRequest.SaleProperty.SpacesDimensionsInfo.GarageDescription;
                residentialListingRequest.OtherParking = listingRequest.SaleProperty.SpacesDimensionsInfo.OtherParking;

                residentialListingRequest.Beds = listingRequest.SaleProperty.SpacesDimensionsInfo.NumBedrooms;
                #endregion

                #region Rooms
                foreach (var room in listingRequest.SaleProperty.Rooms)
                {
                    var width = room.Width;
                    var length = room.Length;
                    var level = room.Level;
                    switch (room.RoomType)
                    {
                        case RoomType.MasterBedroom:
                            residentialListingRequest.Bed1Level = level;
                            residentialListingRequest.Bed1Length = length;
                            residentialListingRequest.Bed1Width = width;
                            residentialListingRequest.Bed1Desc = listingRequest.SaleProperty.SpacesDimensionsInfo.MasterBedroom;
                            break;
                        case RoomType.Bed:
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
                        case RoomType.Entry:
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
                        case RoomType.Living:
                            residentialListingRequest.LivingRoom1Level = level;
                            residentialListingRequest.LivingRoom1Length = length;
                            residentialListingRequest.LivingRoom1Width = width;
                            break;
                        case RoomType.MasterBath:
                            residentialListingRequest.Bath1Level = level;
                            residentialListingRequest.Bath1Length = length;
                            residentialListingRequest.Bath1Width = width;
                            residentialListingRequest.BedBathDesc = listingRequest.SaleProperty.SpacesDimensionsInfo.MasterBathDescription;
                            break;
                        case RoomType.MasterBedroomCloset:
                            residentialListingRequest.ClosetLength = length;
                            residentialListingRequest.ClosetWidth = width;
                            break;
                        case RoomType.Media:
                            residentialListingRequest.OtherRoom2Level = level;
                            residentialListingRequest.OtherRoom2Length = length;
                            residentialListingRequest.OtherRoom2Width = width;
                            break;
                        case RoomType.Study:
                            residentialListingRequest.StudyLevel = level;
                            residentialListingRequest.StudyLength = length;
                            residentialListingRequest.StudyWidth = width;
                            break;
                        case RoomType.Utility:
                            residentialListingRequest.UtilityRoomLevel = level;
                            residentialListingRequest.UtilityRoomLength = length;
                            residentialListingRequest.UtilityRoomWidth = width;
                            break;
                        case RoomType.Studen:
                        case RoomType.Other:
                        case RoomType.Office:
                        case RoomType.HalfBath:
                        case RoomType.FullBath:
                        default:
                            break;

                    }
                }
                #endregion

                #region FeaturesInfo
                //PropertyDescription
                residentialListingRequest.InclusionsDesc = listingRequest.SaleProperty.FeaturesInfo.Inclusions;
                residentialListingRequest.NumFireplaces = listingRequest.SaleProperty.FeaturesInfo.Fireplaces;
                residentialListingRequest.FireplaceDesc = listingRequest.SaleProperty.FeaturesInfo.FireplaceDescription;
                residentialListingRequest.FloorsDesc = listingRequest.SaleProperty.FeaturesInfo.Floors;
                residentialListingRequest.WindowCoverings = listingRequest.SaleProperty.FeaturesInfo.WindowCoverings;
                //HasAccessibility
                residentialListingRequest.AccessibilityDesc = listingRequest.SaleProperty.FeaturesInfo.Accessibility;
                residentialListingRequest.HousingStyleDesc = listingRequest.SaleProperty.FeaturesInfo.HousingStyle;
                residentialListingRequest.ExteriorFeatures = listingRequest.SaleProperty.FeaturesInfo.ExteriorFeatures;
                residentialListingRequest.RoofDesc = listingRequest.SaleProperty.FeaturesInfo.RoofDescription;
                residentialListingRequest.FoundationDesc = listingRequest.SaleProperty.FeaturesInfo.Foundation;
                residentialListingRequest.ExteriorDesc = listingRequest.SaleProperty.FeaturesInfo.Exterior;
                residentialListingRequest.HasPool = listingRequest.SaleProperty.FeaturesInfo.HasPrivatePool;
                residentialListingRequest.PoolDesc = listingRequest.SaleProperty.FeaturesInfo.PrivatePool;
                residentialListingRequest.FacesDesc = listingRequest.SaleProperty.FeaturesInfo.HomeFaces; //check
                residentialListingRequest.SupElectricity = listingRequest.SaleProperty.FeaturesInfo.SupplierElectricity;
                residentialListingRequest.SupWater = listingRequest.SaleProperty.FeaturesInfo.SupplierWater;
                residentialListingRequest.SupGarbage = listingRequest.SaleProperty.FeaturesInfo.SupplierGarbage;
                residentialListingRequest.SupGarbage = listingRequest.SaleProperty.FeaturesInfo.SupplierGarbage;
                residentialListingRequest.SupGas = listingRequest.SaleProperty.FeaturesInfo.SupplierGas;
                residentialListingRequest.SupSewer = listingRequest.SaleProperty.FeaturesInfo.SupplierSewer;
                residentialListingRequest.SupOther = listingRequest.SaleProperty.FeaturesInfo.SupplierOther;
                residentialListingRequest.HeatSystemDesc = listingRequest.SaleProperty.FeaturesInfo.HeatSystem;
                residentialListingRequest.CoolSystemDesc = listingRequest.SaleProperty.FeaturesInfo.CoolingSystem;
                residentialListingRequest.HeatingFuel = listingRequest.SaleProperty.FeaturesInfo.HeatingFuel;
                residentialListingRequest.WaterAccessDesc = listingRequest.SaleProperty.FeaturesInfo.WaterSewer; //check
                residentialListingRequest.GreenCerts = listingRequest.SaleProperty.FeaturesInfo.GreenCertification;
                residentialListingRequest.EnergyDesc = listingRequest.SaleProperty.FeaturesInfo.EnergyFeatures; //check
                residentialListingRequest.GreenFeatures = listingRequest.SaleProperty.FeaturesInfo.GreenFeatures;
                residentialListingRequest.CommonFeatures = listingRequest.SaleProperty.FeaturesInfo.NeighborhoodAmenities; //check
                //lotImprovements
                #endregion

                #region FinantialInfo
                residentialListingRequest.TaxRate = listingRequest.SaleProperty.FinancialInfo.TaxRate;
                residentialListingRequest.TaxYear = listingRequest.SaleProperty.FinancialInfo.TaxYear;
                residentialListingRequest.IsMultiParcel = listingRequest.SaleProperty.FinancialInfo.IsMultipleTaxed.ToString(); //check
                residentialListingRequest.TitleCo = listingRequest.SaleProperty.FinancialInfo.TitleCompany;
                residentialListingRequest.PROPSDTRMS = listingRequest.SaleProperty.FinancialInfo.ProposedTerms;
                //hoaRequirement
                residentialListingRequest.HasMultipleHOA = listingRequest.SaleProperty.FinancialInfo.HasMultipleHOA.ToString();
                //NumHOA
                //buyersAgentCommission
                //HasAgentBonus
                //hasBonusWithAmount
                residentialListingRequest.AgentBonusAmount = listingRequest.SaleProperty.FinancialInfo.AgentBonusAmount.ToString();
                residentialListingRequest.CompBuyBonusExpireDate = listingRequest.SaleProperty.FinancialInfo.BonusExpirationDate; //check
                residentialListingRequest.BuyerIncentive = listingRequest.SaleProperty.FinancialInfo.HasBuyerIncentive.ToString(); //check
                residentialListingRequest.CompBuy = listingRequest.SaleProperty.FinancialInfo.BuyersAgentCommission;

                #endregion

                #region ShowingInfo
                residentialListingRequest.AltPhoneCommunity = listingRequest.SaleProperty.ShowingInfo.AltPhoneCommunity;
                residentialListingRequest.AgentListApptPhone = listingRequest.SaleProperty.ShowingInfo.AgentListApptPhone;
                residentialListingRequest.Showing = listingRequest.SaleProperty.ShowingInfo.Showing;
                residentialListingRequest.RealtorContactEmail = listingRequest.SaleProperty.ShowingInfo.RealtorContactEmail;
                residentialListingRequest.Directions = listingRequest.SaleProperty.ShowingInfo.Directions;
                residentialListingRequest.AgentPrivateRemarks = listingRequest.SaleProperty.ShowingInfo.AgentPrivateRemarks;
                #endregion

                #region SchoolsInfo
                residentialListingRequest.SchoolDistrict = listingRequest.SaleProperty.SchoolsInfo.SchoolDistrict;
                residentialListingRequest.SchoolName1 = listingRequest.SaleProperty.SchoolsInfo.ElementarySchool;
                residentialListingRequest.SchoolName2 = listingRequest.SaleProperty.SchoolsInfo.MiddleSchool;
                residentialListingRequest.SchoolName3 = listingRequest.SaleProperty.SchoolsInfo.HighSchool;
                #endregion

                #region OpenHouse
                /*
                foreach (var openHouse in listingRequest.SaleProperty.ShowingTab.OpenHouses)
                {
                    switch (openHouse.Type) 
                    {
                        case OpenHouseTypeEnum.Saturday:
                            residentialListingRequest.OHRefreshmentsSat = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeSat = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeSat = openHouse.EndTime;
                            residentialListingRequest.OHCommentsSat = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Sunday:
                            residentialListingRequest.OHRefreshmentsSun = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeSun = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeSun = openHouse.EndTime;
                            residentialListingRequest.OHCommentsSun = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Monday:
                            residentialListingRequest.OHRefreshmentsMon = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeMon = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeMon = openHouse.EndTime;
                            residentialListingRequest.OHCommentsMon = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Tuesday:
                            residentialListingRequest.OHRefreshmentsTue = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeTue = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeTue = openHouse.EndTime;
                            residentialListingRequest.OHCommentsTue = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Wednesday:
                            residentialListingRequest.OHRefreshmentsWed = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeWed = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeWed = openHouse.EndTime;
                            residentialListingRequest.OHCommentsWed = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Thursday:
                            residentialListingRequest.OHRefreshmentsThu = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeThu = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeThu = openHouse.EndTime;
                            residentialListingRequest.OHCommentsThu = openHouse.Comments;
                            break;
                        case OpenHouseTypeEnum.Friday:
                            residentialListingRequest.OHRefreshmentsFri = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeFri = openHouse.StartTime;
                            residentialListingRequest.OHEndTimeFri = openHouse.EndTime;
                            residentialListingRequest.OHCommentsFri = openHouse.Comments;
                            break;
                        default: break;
                    }
                }*/
                #endregion

                #region HOAS
                #endregion
                residentialListingRequests.Add(residentialListingRequest);
            }

            return residentialListingRequests;
        }
    }
}
