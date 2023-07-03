namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Sabor.Api.Contracts.Response;
    using Husa.Quicklister.Sabor.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Sabor.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.Sabor.Domain.Enums;
    using Husa.Uploader.Crosscutting.Converters;
    using Husa.Uploader.Crosscutting.Extensions;

    public class SaborListingRequest : ResidentialListingRequest
    {
        private readonly ListingSaleRequestQueryResponse listingResponse;
        private readonly ListingSaleRequestDetailResponse listingDetailResponse;

        public SaborListingRequest(ListingSaleRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public SaborListingRequest(ListingSaleRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private SaborListingRequest()
            : base()
        {
        }

        public override MarketCode MarketCode => MarketCode.SanAntonio;

        public override ResidentialListingRequest CreateFromApiResponse() => new SaborListingRequest
        {
            ResidentialListingRequestID = this.listingResponse.Id,
            OwnerName = this.listingResponse.OwnerName,
            CompanyName = this.listingResponse.OwnerName,
            MLSNum = this.listingResponse.MlsNumber,
            MarketName = this.listingResponse.Market,
            CityCode = this.listingResponse.City.ToStringFromEnumMember(),
            Subdivision = this.listingResponse.Subdivision,
            Zip = this.listingResponse.ZipCode,
            Address = this.listingResponse.Address,
            ListPrice = this.listingResponse.ListPrice.HasValue ? (int)this.listingResponse.ListPrice.Value : default,
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new SaborListingRequest
            {
                ResidentialListingRequestID = this.listingDetailResponse.Id,
                ResidentialListingRequestGUID = this.listingDetailResponse.Id,
                ResidentialListingID = this.listingDetailResponse.ListingSaleId,
                MarketName = this.MarketCode.GetEnumDescription(),
                ListPrice = (int)this.listingDetailResponse.ListPrice,
                MLSNum = this.listingDetailResponse.MlsNumber,
                MlsStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                ListStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                ExpiredDate = this.listingDetailResponse.ExpirationDate,
            };

            FillSalePropertyInfo(this.listingDetailResponse.SaleProperty.SalePropertyInfo);
            FillAddressInfo(this.listingDetailResponse.SaleProperty.AddressInfo);
            FillPropertyInfo(this.listingDetailResponse.SaleProperty.PropertyInfo);
            FillSpacesDimensionsInfo(this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo);
            FillFeaturesInfo(this.listingDetailResponse.SaleProperty.FeaturesInfo);
            FillFinancialInfo(this.listingDetailResponse.SaleProperty.FinancialInfo);
            FillShowingInfo(this.listingDetailResponse.SaleProperty.ShowingInfo);
            FillSchoolsInfo(this.listingDetailResponse.SaleProperty.SchoolsInfo);
            FillHoaInfo(this.listingDetailResponse.SaleProperty.Hoas);
            FillRoomsInfo(this.listingDetailResponse.SaleProperty.Rooms);
            FillOpenHouseInfo(this.listingDetailResponse.SaleProperty.OpenHouses);

            return residentialListingRequest;

            void FillSalePropertyInfo(SalePropertyResponse saleProperty)
            {
                if (saleProperty is null)
                {
                    throw new ArgumentNullException(nameof(saleProperty));
                }

                residentialListingRequest.BuilderName = saleProperty.OwnerName;
                residentialListingRequest.CompanyName = saleProperty.OwnerName;
                residentialListingRequest.OwnerName = saleProperty.OwnerName;
            }

            void FillAddressInfo(AddressInfoResponse addressInfo)
            {
                if (addressInfo is null)
                {
                    throw new ArgumentNullException(nameof(addressInfo));
                }

                residentialListingRequest.StreetNum = addressInfo.StreetNumber;
                residentialListingRequest.StreetName = addressInfo.StreetName;
                residentialListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                residentialListingRequest.State = addressInfo.State?.ToStringFromEnumMember();
                residentialListingRequest.Zip = addressInfo.ZipCode;
                residentialListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                residentialListingRequest.LotNum = addressInfo.LotNum;
                residentialListingRequest.Block = addressInfo.Block;
                residentialListingRequest.Subdivision = addressInfo.Subdivision;
            }

            void FillPropertyInfo(PropertyInfoResponse propertyInfo)
            {
                if (propertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                residentialListingRequest.BuildCompletionDate = propertyInfo.ConstructionCompletionDate;
                residentialListingRequest.YearBuiltDesc = propertyInfo.ConstructionStage.ToString();
                residentialListingRequest.YearBuilt = propertyInfo.ConstructionStartYear;
                residentialListingRequest.Legal = propertyInfo.LegalDescription;
                residentialListingRequest.TaxID = propertyInfo.TaxId;
                residentialListingRequest.MLSArea = propertyInfo.MlsArea?.ToStringFromEnumMember();
                residentialListingRequest.MapscoMapBook = propertyInfo.MapscoGrid;
                residentialListingRequest.MapscoMapCoord = propertyInfo.MapscoGrid;
                residentialListingRequest.LotDim = propertyInfo.LotDimension;
                residentialListingRequest.LotSize = propertyInfo.LotSize;
                residentialListingRequest.LotDesc = propertyInfo.LotDescription.ToStringFromEnumMembers();
                residentialListingRequest.Occupancy = propertyInfo.Occupancy.ToStringFromEnumMembers();
                residentialListingRequest.Latitude = propertyInfo.Latitude;
                residentialListingRequest.Longitude = propertyInfo.Longitude;
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                if (spacesDimensionsInfo is null)
                {
                    throw new ArgumentNullException(nameof(spacesDimensionsInfo));
                }

                residentialListingRequest.Category = spacesDimensionsInfo.TypeCategory.ToStringFromEnumMember();
                residentialListingRequest.NumStories = spacesDimensionsInfo.Stories?.ToStringFromEnumMember();
                residentialListingRequest.SqFtTotal = spacesDimensionsInfo.SqFtTotal;
                residentialListingRequest.SqFtSource = spacesDimensionsInfo.SqFtSource?.ToStringFromEnumMember();
                residentialListingRequest.InteriorDesc = spacesDimensionsInfo.SpecialtyRooms.ToStringFromEnumMembers();
                residentialListingRequest.BathsFull = spacesDimensionsInfo.BathsFull;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.BathsHalf;
                residentialListingRequest.ParkingDesc = spacesDimensionsInfo.GarageDescription.ToStringFromEnumMembers();
                residentialListingRequest.OtherParking = spacesDimensionsInfo.OtherParking?.ToStringFromEnumMembers();
                residentialListingRequest.Beds = spacesDimensionsInfo.NumBedrooms;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                if (featuresInfo is null)
                {
                    throw new ArgumentNullException(nameof(featuresInfo));
                }

                residentialListingRequest.InclusionsDesc = featuresInfo.Inclusions?.ToStringFromEnumMembers();
                residentialListingRequest.NumberFireplaces = featuresInfo.Fireplaces.ToFireplaceOption();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.FloorsDesc = featuresInfo.Floors?.ToStringFromEnumMembers();
                residentialListingRequest.WindowCoverings = featuresInfo.WindowCoverings?.ToStringFromEnumMembers();
                residentialListingRequest.HasHandicapAmenities = featuresInfo.HasAccessibility.BoolToYesNoBool();
                residentialListingRequest.AccessibilityDesc = featuresInfo.Accessibility.ToStringFromEnumMembers();
                residentialListingRequest.HousingStyleDesc = featuresInfo.HousingStyle.ToStringFromEnumMembers();
                residentialListingRequest.ExteriorFeatures = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.ExteriorDesc = featuresInfo.Exterior.ToStringFromEnumMembers();
                residentialListingRequest.HasPool = featuresInfo.HasPrivatePool;
                residentialListingRequest.PoolDesc = featuresInfo.PrivatePool.ToStringFromEnumMembers();
                residentialListingRequest.FacesDesc = featuresInfo.HomeFaces.ToStringFromEnumMembers();
                residentialListingRequest.SupElectricity = featuresInfo.SupplierElectricity;
                residentialListingRequest.SupWater = featuresInfo.SupplierWater;
                residentialListingRequest.SupGarbage = featuresInfo.SupplierGarbage;
                residentialListingRequest.SupGas = featuresInfo.SupplierGas;
                residentialListingRequest.SupSewer = featuresInfo.SupplierSewer;
                residentialListingRequest.SupOther = featuresInfo.SupplierOther;
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.HeatingFuel = featuresInfo.HeatingFuel.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                residentialListingRequest.GreenCerts = featuresInfo.GreenCertification.ToStringFromEnumMembers();
                residentialListingRequest.EnergyDesc = featuresInfo.EnergyFeatures.ToStringFromEnumMembers();
                residentialListingRequest.GreenFeatures = featuresInfo.GreenFeatures.ToStringFromEnumMembers();
                residentialListingRequest.CommonFeatures = featuresInfo.NeighborhoodAmenities.ToStringFromEnumMembers();
                residentialListingRequest.PublicRemarks = featuresInfo.PropertyDescription;
                residentialListingRequest.UtilitiesDesc = featuresInfo.LotImprovements.ToStringFromEnumMembers();
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                residentialListingRequest.TaxRate = financialInfo.TaxRate.StrictDecimalToString();
                residentialListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                residentialListingRequest.IsMultiParcel = financialInfo.IsMultipleTaxed.ToString();
                residentialListingRequest.TitleCo = financialInfo.TitleCompany;
                residentialListingRequest.ProposedTerms = financialInfo.ProposedTerms.ToStringFromEnumMembers();
                residentialListingRequest.HasMultipleHOA = financialInfo.HasMultipleHOA.ToString();
                residentialListingRequest.CompBuy = financialInfo.BuyersAgentCommission?.ToString();
                residentialListingRequest.CompBuyType = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.HOA = financialInfo.HOARequirement?.ToStringFromEnumMember();
                residentialListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount.DecimalToString();
                residentialListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = financialInfo.BonusExpirationDate;
                residentialListingRequest.BuyerCheckBox = financialInfo.HasBuyerIncentive;
                residentialListingRequest.BuyerIncentive = financialInfo.BuyersAgentCommission.DecimalToString();
                residentialListingRequest.BuyerIncentiveDesc = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
            }

            void FillShowingInfo(ShowingResponse showingInfo)
            {
                if (showingInfo is null)
                {
                    throw new ArgumentNullException(nameof(showingInfo));
                }

                residentialListingRequest.AltPhoneCommunity = showingInfo.AltPhoneCommunity;
                residentialListingRequest.AgentListApptPhone = showingInfo.AgentListApptPhone;
                residentialListingRequest.Showing = showingInfo.Showing?.ToStringFromEnumMember();
                residentialListingRequest.RealtorContactEmail = showingInfo.RealtorContactEmail;
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
            }

            void FillSchoolsInfo(SchoolsResponse schoolsInfo)
            {
                if (schoolsInfo is null)
                {
                    throw new ArgumentNullException(nameof(schoolsInfo));
                }

                residentialListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName1 = schoolsInfo.ElementarySchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName2 = schoolsInfo.MiddleSchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName3 = schoolsInfo.HighSchool?.ToStringFromEnumMember();
            }

            void FillHoaInfo(IEnumerable<HoaResponse> hoas)
            {
                if (hoas == null || !hoas.Any())
                {
                    return;
                }

                residentialListingRequest.HOAs = new List<HoaRequest>();
                foreach (HoaResponse hoa in hoas)
                {
                    var hoaRequest = new HoaRequest
                    {
                        Phone = new HoaPhone(hoa.ContactPhone),
                        Name = hoa.Name,
                        Fee = (int)hoa.Fee,
                        TransferFee = (int)hoa.TransferFee,
                        FeePaid = hoa.BillingFrequency.ToStringFromEnumMember(),
                    };
                    residentialListingRequest.HOAs.Add(hoaRequest);
                }

                residentialListingRequest.HasMultipleHOA = hoas.Count().ToStringFromHasMultipleHOA();
                residentialListingRequest.NumHoas = hoas.Count().ToString();
            }

            void FillRoomsInfo(IEnumerable<RoomResponse> rooms)
            {
                if (rooms == null || !rooms.Any())
                {
                    return;
                }

                foreach (var room in rooms)
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
                            residentialListingRequest.Bed1Desc = room.Features.ToStringFromEnumMembers();
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
                            residentialListingRequest.DiningRoomLength = length;
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
                            residentialListingRequest.BedBathDesc = room.Features.ToStringFromEnumMembers();
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
                        default:
                            break;
                    }
                }
            }

            void FillOpenHouseInfo(IEnumerable<OpenHouseResponse> openHouses)
            {
                if (openHouses == null || !openHouses.Any())
                {
                    return;
                }

                foreach (var openHouse in openHouses)
                {
                    switch (openHouse.Type)
                    {
                        case OpenHouseType.Saturday:
                            //// residentialListingRequest.OHRefreshmentsSat = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeSat = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeSat = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsSat = openHouse.Comments;
                            break;
                        case OpenHouseType.Sunday:
                            //// residentialListingRequest.OHRefreshmentsSun = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeSun = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeSun = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsSun = openHouse.Comments;
                            break;
                        case OpenHouseType.Monday:
                            //// residentialListingRequest.OHRefreshmentsMon = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeMon = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeMon = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsMon = openHouse.Comments;
                            break;
                        case OpenHouseType.Tuesday:
                            //// residentialListingRequest.OHRefreshmentsTue = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeTue = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeTue = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsTue = openHouse.Comments;
                            break;
                        case OpenHouseType.Wednesday:
                            //// residentialListingRequest.OHRefreshmentsWed = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeWed = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeWed = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsWed = openHouse.Comments;
                            break;
                        case OpenHouseType.Thursday:
                            //// residentialListingRequest.OHRefreshmentsThu = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeThu = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeThu = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsThu = openHouse.Comments;
                            break;
                        case OpenHouseType.Friday:
                            //// residentialListingRequest.OHRefreshmentsFri = openHouse.Refreshments;
                            residentialListingRequest.OHStartTimeFri = openHouse.StartTime.ToString();
                            residentialListingRequest.OHEndTimeFri = openHouse.EndTime.ToString();
                            //// residentialListingRequest.OHCommentsFri = openHouse.Comments;
                            break;
                        default: break;
                    }
                }
            }
        }

        public string GetAgentRemarksMessage()
        {
            const string homeUnderConstruction = "Home is under construction. For your safety, call appt number for showings";

            var realtorContactEmail = !string.IsNullOrEmpty(this.EmailRealtorsContact) ? this.EmailRealtorsContact : this.RealtorContactEmail;
            var message = this.GetPrivateRemarks(useExtendedRemarks: true, addPlanName: false);

            if (!string.IsNullOrWhiteSpace(realtorContactEmail) &&
                !message.ToLower().Contains("email contact") &&
                !message.ToLower().Contains(realtorContactEmail))
            {
                message += $"Email contact: {realtorContactEmail}. ";
            }

            var bonusMessage = string.IsNullOrWhiteSpace(this.MLSNum) ? this.GetAgentBonusRemarksMessage() : string.Empty;
            var incompletedBuiltNote = this.YearBuiltDesc == "Incomplete"
                 && !message.Contains(homeUnderConstruction) ? $"{homeUnderConstruction}. " : string.Empty;

            return bonusMessage + incompletedBuiltNote + message;
        }

        public override string GetPublicRemarks()
        {
            var builtNote = "MLS# " + this.MLSNum + " - Built by " + this.CompanyName + " - ";
            if (this.YearBuiltDesc == "Complete")
            {
                string dateFormat = "MMM dd";
                int diffDays = DateTime.Now.Subtract((DateTime)this.BuildCompletionDate).Days;
                if (diffDays > 365)
                {
                    dateFormat = "MMM dd yyyy";
                }

                if (!string.IsNullOrEmpty(this.RemarksFormatFromCompany) && this.RemarksFormatFromCompany == "SD")
                {
                    builtNote += "CONST. COMPLETED " + this.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                }
                else
                {
                    builtNote += "Ready Now! ~ ";
                }
            }
            else
            {
                builtNote += GetCompletionText() + " completion! ~ ";
            }

            if (this.IncludeRemarks != null && !this.IncludeRemarks.Value)
            {
                builtNote = string.Empty;
            }

            if (!string.IsNullOrEmpty(this.PublicRemarks) && this.PublicRemarks.Contains('~'))
            {
                int tempIndex = this.PublicRemarks.IndexOf('~') + 1;
                return builtNote + this.PublicRemarks[tempIndex..].Trim().RemoveSlash();
            }

            return builtNote + this.PublicRemarks.RemoveSlash();

            string GetCompletionText()
            {
                const string defaultMonth = "January";

                if (!this.BuildCompletionDate.HasValue)
                {
                    return defaultMonth;
                }

                return this.BuildCompletionDate.Value.Month switch
                {
                    2 => "February",
                    3 => "March",
                    4 => "April",
                    5 => "May",
                    6 => "June",
                    7 => "July",
                    8 => "August",
                    9 => "September",
                    10 => "October",
                    11 => "November",
                    12 => "December",
                    _ => "January",
                };
            }
        }

        public override string GetBuyerAgentComp(string compBuy, string compBuyType)
        {
            // Remove zeroes after decimal point if available.
            if (compBuy.Contains('.'))
            {
                compBuy = compBuy.TrimEnd('0').TrimEnd('.');
            }

            string formattedNumber = compBuyType switch
            {
                "%" => compBuy + "%",
                "$" => "$" + compBuy,
                _ => throw new ArgumentException("Invalid type."),
            };

            return formattedNumber;
        }
    }
}
