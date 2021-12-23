namespace Husa.Cargador.Support
{
    using Husa.Cargador.ViewModels;
    using Husa.Cargador.ViewModels.Enum;
    using Husa.Core.UploaderBase;
    using System;
    using System.Collections.Generic;

    public class ClassTransform
    {
        public IEnumerable<ResidentialListingRequest> CosmoObjectToResidentialListingRequest(IEnumerable<ListingRequestSale> listingsRequestSale)
        {
            var residentialListingRequests = new List<ResidentialListingRequest>();

            foreach(var listingRequest in listingsRequestSale)
            {
                var residentialListingRequest = new ResidentialListingRequest();
                residentialListingRequest.MarketUsername = "username";
                residentialListingRequest.MarketPassword = "password";
                residentialListingRequest.MarketID = 5;
                residentialListingRequest.MarketName = "DFW";

                residentialListingRequest.ContingencyInfo = listingRequest.ContingencyInfo;
                residentialListingRequest.ContractDate = listingRequest.ContractDate;
                residentialListingRequest.ExpiredDateOption = listingRequest.ExpiredDateOption;
                ////residentialListingRequest.Financing = listingRequest.Financing;
                residentialListingRequest.KickOutInfo = listingRequest.KickOutInformation;
                residentialListingRequest.MortgageCoSold = listingRequest.MortgageCompany;
                decimal test = 0;
                decimal.TryParse(listingRequest.SellerPaid, out test);
                residentialListingRequest.SellerPaid = test;
                residentialListingRequest.StateCode =  listingRequest.ListingRequestState.ToString();
                residentialListingRequest.ListPrice = (decimal)listingRequest.ListPrice;
                residentialListingRequest.MLSArea = listingRequest.MlsArea;
                residentialListingRequest.MLSSubArea = listingRequest.MlsSubArea;
                residentialListingRequest.MLSNum = listingRequest.MlsNumber;
                residentialListingRequest.MlsStatus = listingRequest.MlsStatus.ToString();
                ////residentialListingRequest.AgentID_SELL = listingRequest.AgentID;
                residentialListingRequest.BackOnMarketDate = listingRequest.BackOnMarketDate;
                ////residentialListingRequest. = listingRequest.CancelDate;
                residentialListingRequest.CancelledOption = listingRequest.CancelledOption;
                residentialListingRequest.CancelledReason = listingRequest.CancelledReason;
                residentialListingRequest.ClosedDate = listingRequest.ClosedDate;
                ////residentialListingRequest. = listingRequest.ClosePrice;
                residentialListingRequest.EstClosedDate = listingRequest.EstimatedClosedDate;
                residentialListingRequest.OffMarketDate = listingRequest.OffMarketDate;
                residentialListingRequest.PendingDate = listingRequest.PendingDate;
                residentialListingRequest.SysCreatedOn = listingRequest.CreatedOn;
                ////residentialListingRequest.SysCreatedBy = listingRequest.CreatedBy;
                residentialListingRequest.SysModifiedOn = listingRequest.ModifiedOn;
                ////residentialListingRequest.SysModifiedBy = listingRequest.ModifiedBy;
                ///
                #region PropertyTab
                residentialListingRequest.BuildCompletionDate = listingRequest.SaleProperty.PropertyTab.ConstCompletionDate;
                ////residentialListingRequest. = listingRequest.SaleProperty.PropertyTab.LakeName;
                ////residentialListingRequest. = listingRequest.SaleProperty.PropertyTab.ConstructionStartYear;
                ////residentialListingRequest. = listingRequest.SaleProperty.PropertyTab.ConstructionStage;
                residentialListingRequest.MasterPlannedCommunityName = listingRequest.SaleProperty.PropertyTab.MasterPlannedCommunity;
                residentialListingRequest.TaxID = listingRequest.SaleProperty.PropertyTab.TaxId;
                residentialListingRequest.Acres = listingRequest.SaleProperty.PropertyTab.Acres;
                residentialListingRequest.Block = listingRequest.SaleProperty.PropertyTab.Block;
                residentialListingRequest.City = listingRequest.SaleProperty.PropertyTab.City;
                residentialListingRequest.County = listingRequest.SaleProperty.PropertyTab.County;
                residentialListingRequest.Latitude = listingRequest.SaleProperty.PropertyTab.Latitude ;
                residentialListingRequest.Longitude = listingRequest.SaleProperty.PropertyTab.Longitude;
                residentialListingRequest.LotNum = listingRequest.SaleProperty.PropertyTab.Lot;
                residentialListingRequest.LotDesc = listingRequest.SaleProperty.PropertyTab.LotDescription;
                residentialListingRequest.LotDim = listingRequest.SaleProperty.PropertyTab.LotDimensions;
                residentialListingRequest.LotSizeAcres = listingRequest.SaleProperty.PropertyTab.LotSizeAcreage;
                residentialListingRequest.OwnerName = listingRequest.SaleProperty.PropertyTab.OwnerName;
                ////residentialListingRequest. = listingRequest.SaleProperty.PropertyTab.PreDirection;
                residentialListingRequest.State = listingRequest.SaleProperty.PropertyTab.State;
                residentialListingRequest.StreetName = listingRequest.SaleProperty.PropertyTab.StreetName;
                residentialListingRequest.StreetNum = listingRequest.SaleProperty.PropertyTab.StreetNum;
                residentialListingRequest.StreetType = listingRequest.SaleProperty.PropertyTab.StreetType;
                residentialListingRequest.Subdivision = listingRequest.SaleProperty.PropertyTab.Subdivision;
                residentialListingRequest.UnitNum = listingRequest.SaleProperty.PropertyTab.UnitNumber;
                residentialListingRequest.Zip = listingRequest.SaleProperty.PropertyTab.ZipCode;
                #endregion

                #region SpaceAndDimensionTab
                residentialListingRequest.CarportCapacity = listingRequest.SaleProperty.SpaceAndDimensionsTab.CarpotSpaces;
                residentialListingRequest.BathsFull = listingRequest.SaleProperty.SpaceAndDimensionsTab.FullBaths;
                residentialListingRequest.GarageDesc = listingRequest.SaleProperty.SpaceAndDimensionsTab.GarageDescription;
                residentialListingRequest.GarageLength = listingRequest.SaleProperty.SpaceAndDimensionsTab.GarageLength;
                ////residentialListingRequest. = listingRequest.SaleProperty.SpaceAndDimensionsTab.GarageSpaces;
                residentialListingRequest.GarageWidth = listingRequest.SaleProperty.SpaceAndDimensionsTab.GarageWidth;
                residentialListingRequest.HousingTypeDesc = listingRequest.SaleProperty.SpaceAndDimensionsTab.HousingType;
                residentialListingRequest.BathsHalf = listingRequest.SaleProperty.SpaceAndDimensionsTab.HalfBaths;
                residentialListingRequest.PropType = listingRequest.SaleProperty.SpaceAndDimensionsTab.PropertyType;
                residentialListingRequest.NumStories = listingRequest.SaleProperty.SpaceAndDimensionsTab.Stories;
                residentialListingRequest.SqFtTotal = listingRequest.SaleProperty.SpaceAndDimensionsTab.SquareFeets; 
                residentialListingRequest.SqFtSource = listingRequest.SaleProperty.SpaceAndDimensionsTab.SquareFeetSource;
                ////residentialListingRequest. = listingRequest.SaleProperty.SpaceAndDimensionsTab.MasterBedroom;
                ////residentialListingRequest. = listingRequest.SaleProperty.SpaceAndDimensionsTab.UtilityRoom;
                #region Rooms
                foreach (var room in listingRequest.SaleProperty.SpaceAndDimensionsTab.Rooms)
                {
                    var dimensions = room.Dimensions.Split('x');
                    var width = Int32.Parse(dimensions[0]);
                    var length = Int32.Parse(dimensions[1]);
                    switch(room.Type)
                    {
                        case RoomTypeEnum.BEDROO:
                            if (residentialListingRequest.Bed2Level == null)
                            {
                                residentialListingRequest.Bed2Level = room.Level;
                                residentialListingRequest.Bed2Length = length;
                                residentialListingRequest.Bed2Width = width;
                                break;
                            }

                            if (residentialListingRequest.Bed3Level == null)
                            {
                                residentialListingRequest.Bed3Level = room.Level;
                                residentialListingRequest.Bed3Length = length;
                                residentialListingRequest.Bed3Width = width;
                                break;
                            }

                            if (residentialListingRequest.Bed4Level == null)
                            {
                                residentialListingRequest.Bed4Level = room.Level;
                                residentialListingRequest.Bed4Length = length;
                                residentialListingRequest.Bed4Width = width;
                                break;
                            }

                            if (residentialListingRequest.Bed5Level == null)
                            {
                                residentialListingRequest.Bed5Level = room.Level;
                                residentialListingRequest.Bed5Length = length;
                                residentialListingRequest.Bed5Width = width;
                                break;
                            }
                            break;

                        case RoomTypeEnum.BREROO:
                            residentialListingRequest.BreakfastLevel = room.Level;
                            residentialListingRequest.BreakfastLength = length;
                            residentialListingRequest.BreakfastWidth = width;
                            residentialListingRequest.OtherRoomDesc = listingRequest.SaleProperty.SpaceAndDimensionsTab.UtilityRoom;
                            break;
                        
                        case RoomTypeEnum.DINROO:
                            residentialListingRequest.DiningRoomLevel = room.Level;
                            residentialListingRequest.DiningRoomLength = length;
                            residentialListingRequest.DiningRoomWidth = width;
                            break;

                        case RoomTypeEnum.GAMROO:
                            residentialListingRequest.LivingRoom3Level = room.Level;
                            residentialListingRequest.LivingRoom3Length = length;
                            residentialListingRequest.LivingRoom3Width = width;
                            break;

                        case RoomTypeEnum.KITCHE:
                            residentialListingRequest.KitchenLevel = room.Level;
                            residentialListingRequest.KitchenLength = length;
                            residentialListingRequest.KitchenWidth = width;
                            break;

                        case RoomTypeEnum.LIVROO:
                            if (residentialListingRequest.LivingRoom1Level == null)
                            {
                                residentialListingRequest.LivingRoom1Level = room.Level;
                                residentialListingRequest.LivingRoom1Length = length;
                                residentialListingRequest.LivingRoom1Width = width;
                                break;
                            }

                            if (residentialListingRequest.LivingRoom2Level == null)
                            {
                                residentialListingRequest.LivingRoom2Level = room.Level;
                                residentialListingRequest.LivingRoom2Length = length;
                                residentialListingRequest.LivingRoom2Width = width;
                                break;
                            }

                            break;

                        case RoomTypeEnum.MASBED:
                            residentialListingRequest.Bed1Level = room.Level;
                            residentialListingRequest.Bed1Length = length;
                            residentialListingRequest.Bed1Width = width;
                            residentialListingRequest.BedBathDesc = listingRequest.SaleProperty.SpaceAndDimensionsTab.MasterBedroom;
                            break;

                        case RoomTypeEnum.MEDROO:
                        case RoomTypeEnum.OTHER:
                            if (residentialListingRequest.OtherRoom1Level == null)
                            {
                                residentialListingRequest.OtherRoom1Level = room.Level;
                                residentialListingRequest.OtherRoom1Length = length;
                                residentialListingRequest.OtherRoom1Width = width;
                                break;
                            }

                            if (residentialListingRequest.OtherRoom2Level == null)
                            {
                                residentialListingRequest.OtherRoom2Level = room.Level;
                                residentialListingRequest.OtherRoom2Length = length;
                                residentialListingRequest.OtherRoom2Width = width;
                                break;
                            }

                            break;

                        case RoomTypeEnum.STUDEN:
                            residentialListingRequest.StudyLevel = room.Level;
                            residentialListingRequest.StudyLength = length;
                            residentialListingRequest.StudyWidth = width;
                            break;

                        case RoomTypeEnum.UTIROO:
                            residentialListingRequest.UtilityRoomLevel = room.Level;
                            residentialListingRequest.UtilityRoomLength = length;
                            residentialListingRequest.UtilityRoomWidth = width;
                            break;

                        default:
                            break;

                    }
                }
                #endregion
                #endregion

                #region FeatureTab
                ////residentialListingRequest. = listingRequest.SaleProperty.FeatureTab.Accessibility;
                residentialListingRequest.AccessibilityDesc = listingRequest.SaleProperty.FeatureTab.AccessibilityFeatures;
                ////residentialListingRequest. = listingRequest.SaleProperty.FeatureTab.AlarmSecurity;
                ///residentialListingRequest. = listingRequest.SaleProperty.FeatureTab.AlarmSecurityType;
                residentialListingRequest.BreakfastLevel = listingRequest.SaleProperty.FeatureTab.BreakFastRoom;
                residentialListingRequest.ConstructionDesc = listingRequest.SaleProperty.FeatureTab.Construction;
                residentialListingRequest.EnergyDesc = listingRequest.SaleProperty.FeatureTab.EnergyFeatures;
                residentialListingRequest.ExteriorDesc = listingRequest.SaleProperty.FeatureTab.Exterior;
                residentialListingRequest.FenceDesc = listingRequest.SaleProperty.FeatureTab.Fence;
                residentialListingRequest.FireplaceDesc = listingRequest.SaleProperty.FeatureTab.FirePlaceDescription;
                residentialListingRequest.FloorsDesc = listingRequest.SaleProperty.FeatureTab.Flooring;
                residentialListingRequest.FoundationDesc = listingRequest.SaleProperty.FeatureTab.Foundation;
                residentialListingRequest.GreenCerts = listingRequest.SaleProperty.FeatureTab.GreenCertification;
                residentialListingRequest.GreenFeatures = listingRequest.SaleProperty.FeatureTab.GreenFeatures;
                residentialListingRequest.HeatSystemDesc = listingRequest.SaleProperty.FeatureTab.HeatingCooling;
                residentialListingRequest.HousingStyleDesc = listingRequest.SaleProperty.FeatureTab.HousingStyle;
                residentialListingRequest.InteriorDesc = listingRequest.SaleProperty.FeatureTab.Interior;
                residentialListingRequest.KitchenDesc = listingRequest.SaleProperty.FeatureTab.KitchenDescription;
                residentialListingRequest.KitchenEquipmentDesc = listingRequest.SaleProperty.FeatureTab.KitchenEquipment;
                residentialListingRequest.NumFireplaces = listingRequest.SaleProperty.FeatureTab.NumFirePlaces;
                residentialListingRequest.HasPool = listingRequest.SaleProperty.FeatureTab.PrivatePool.ToString();
                residentialListingRequest.PoolDesc = listingRequest.SaleProperty.FeatureTab.PrivatePoolDescription;
                residentialListingRequest.RoofDesc = listingRequest.SaleProperty.FeatureTab.Roof;
                residentialListingRequest.SMARTFEATURESAPP = listingRequest.SaleProperty.FeatureTab.SmartFeatures.ToString();
                ////residentialListingRequest. = listingRequest.SaleProperty.FeatureTab.NeighborhoodAmenities;
                residentialListingRequest.MUDDistrict = listingRequest.SaleProperty.FeatureTab.MudDistrict.ToString();
                ////residentialListingRequest. = listingRequest.SaleProperty.FeatureTab.PropertyDescription;
                residentialListingRequest.Utilities = listingRequest.SaleProperty.FeatureTab.Utilities;
                #endregion

                #region FinantialSchoolTab
                residentialListingRequest.AgentCommissionDollarsYN = listingRequest.SaleProperty.FinancialSchoolTab.AgentBonus;
                residentialListingRequest.AgentCommissionPercentYN = listingRequest.SaleProperty.FinancialSchoolTab.AgentBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = listingRequest.SaleProperty.FinancialSchoolTab.AgentBonusAmount;
                ////residentialListingRequest. = listingRequest.SaleProperty.FinancialSchoolTab.AgentBonusExpirationDate;
                ////residentialListingRequest. = listingRequest.SaleProperty.FinancialSchoolTab.BillingFrequency;
                residentialListingRequest.BuyerIncentive = listingRequest.SaleProperty.FinancialSchoolTab.BuyerIncentive.ToString();
                residentialListingRequest.HOA = listingRequest.SaleProperty.FinancialSchoolTab.HOAFee.ToString();
                residentialListingRequest.TitleCo = listingRequest.SaleProperty.FinancialSchoolTab.PreferredTitleCompany;
                residentialListingRequest.TitleCoPhone = listingRequest.SaleProperty.FinancialSchoolTab.TitleCompanyPhone;
                residentialListingRequest.TitleCoLocation = listingRequest.SaleProperty.FinancialSchoolTab.TitleCompanyLocation;
                ////residentialListingRequest. = listingRequest.SaleProperty.FinancialSchoolTab.AgentCommission;
                residentialListingRequest.SchoolName1 = listingRequest.SaleProperty.FinancialSchoolTab.ElementarySchool;
                residentialListingRequest.HasHOA = listingRequest.SaleProperty.FinancialSchoolTab.HOA.ToString();
                residentialListingRequest.AssocFeeIncludes = listingRequest.SaleProperty.FinancialSchoolTab.HOAIncludes;
                residentialListingRequest.AssocName = listingRequest.SaleProperty.FinancialSchoolTab.HOAManagementCompany;
                residentialListingRequest.AssocPhone = listingRequest.SaleProperty.FinancialSchoolTab.HOAManagementCompanyPhone;
                residentialListingRequest.SchoolName3 = listingRequest.SaleProperty.FinancialSchoolTab.HighSchool;
                residentialListingRequest.SchoolName7 = listingRequest.SaleProperty.FinancialSchoolTab.IntermediateSchool;
                residentialListingRequest.SchoolName5 = listingRequest.SaleProperty.FinancialSchoolTab.JuniorHighSchool;
                residentialListingRequest.SchoolName2 = listingRequest.SaleProperty.FinancialSchoolTab.MiddleSchool;
                ////residentialListingRequest. = listingRequest.SaleProperty.FinancialSchoolTab.PrimarySchool;
                residentialListingRequest.SchoolName6 = listingRequest.SaleProperty.FinancialSchoolTab.SeniorSchool;
                residentialListingRequest.SchoolDistrict = listingRequest.SaleProperty.FinancialSchoolTab.SchoolDistrict;
                #endregion

                #region ShowingTab
                residentialListingRequest.AlternatePhoneFromCompany = listingRequest.SaleProperty.ShowingTab.AltPhone;
                #region OpenHouse

                foreach(var openHouse in listingRequest.SaleProperty.ShowingTab.OpenHouses)
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
                }
                #endregion

                residentialListingRequest.AgentListApptPhone = listingRequest.SaleProperty.ShowingTab.ApptPhone;
                ////residentialListingRequest. = listingRequest.SaleProperty.ShowingTab.AgentorRemarks;
                residentialListingRequest.Directions = listingRequest.SaleProperty.ShowingTab.Directions;
                residentialListingRequest.EmailRealtorsContact = listingRequest.SaleProperty.ShowingTab.EmailforRealtors;
                residentialListingRequest.Showing = listingRequest.SaleProperty.ShowingTab.Showing;
                residentialListingRequest.ShowingInstructions = listingRequest.SaleProperty.ShowingTab.ShowingInstructions;
                #endregion


                residentialListingRequests.Add(residentialListingRequest);
            }

            return residentialListingRequests;
        }
    }
}
