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
                residentialListingRequest.MarketID = 8;
                residentialListingRequest.MarketName = "San Antonio";
                residentialListingRequest.ListPrice = (decimal)listingRequest.ListPrice;
                residentialListingRequest.MLSNum = listingRequest.MlsNumber;
                residentialListingRequest.MlsStatus = listingRequest.MlsStatus.ToString();
                residentialListingRequest.SysCreatedOn = listingRequest.CreatedOn;
                residentialListingRequest.SysCreatedBy = listingRequest.CreatedBy;
                residentialListingRequest.SysModifiedOn = listingRequest.ModifiedOn;
                residentialListingRequest.SysModifiedBy = listingRequest.ModifiedBy;

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
                residentialListingRequest.NumStories = int.Parse(listingRequest.SaleProperty.SpacesDimensionsInfo.Stories);
                residentialListingRequest.SqFtTotal = listingRequest.SaleProperty.SpacesDimensionsInfo.SqFtTotal;
                residentialListingRequest.SqFtSource = listingRequest.SaleProperty.SpacesDimensionsInfo.SqFtSource;
                //EntryLength
                //EntryWidth
                //SpecialtyRooms
                //MasterBedrrom
                //numBedrooms
                residentialListingRequest.BathsFull= listingRequest.SaleProperty.SpacesDimensionsInfo.BathsFull;
                residentialListingRequest.BathsHalf= listingRequest.SaleProperty.SpacesDimensionsInfo.BathsHalf;
                //masterBathDescription
                residentialListingRequest.GarageDesc= listingRequest.SaleProperty.SpacesDimensionsInfo.GarageDescription;
                residentialListingRequest.OtherParking = listingRequest.SaleProperty.SpacesDimensionsInfo.OtherParking;
                #endregion

                #region Rooms
                /*
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
                }*/
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
                residentialListingRequest.HasPool = listingRequest.SaleProperty.FeaturesInfo.HasPrivatePool.ToString();
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
