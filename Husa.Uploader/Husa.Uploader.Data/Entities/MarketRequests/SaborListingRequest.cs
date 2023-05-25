namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Sabor.Api.Contracts.Response.ListingRequest.SaleRequest;
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

        public override ResidentialListingRequest CreateFromApiResponse() => new SaborListingRequest()
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
            ListPrice = (int)this.listingResponse.ListPrice,
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
                BuilderName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                CompanyName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                OwnerName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                PlanProfileID = this.listingDetailResponse.SaleProperty.SalePropertyInfo.PlanId,
                CommunityProfileID = this.listingDetailResponse.SaleProperty.SalePropertyInfo.CommunityId,
                StreetNum = this.listingDetailResponse.SaleProperty.AddressInfo.StreetNumber,
                StreetName = this.listingDetailResponse.SaleProperty.AddressInfo.StreetName,
                CityCode = this.listingDetailResponse.SaleProperty.AddressInfo.City.ToStringFromEnumMember(),
                State = this.listingDetailResponse.SaleProperty.AddressInfo.State?.ToStringFromEnumMember(),
                Zip = this.listingDetailResponse.SaleProperty.AddressInfo.ZipCode,
                County = this.listingDetailResponse.SaleProperty.AddressInfo.County?.ToStringFromEnumMember(),
                LotNum = this.listingDetailResponse.SaleProperty.AddressInfo.LotNum,
                Block = this.listingDetailResponse.SaleProperty.AddressInfo.Block,
                Subdivision = this.listingDetailResponse.SaleProperty.AddressInfo.Subdivision,
                BuildCompletionDate = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionCompletionDate, // check
                YearBuiltDesc = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionStage.ToString(),
                YearBuilt = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionStartYear,
                //// ConstructionStage
                //// ConstructionYear
                Legal = this.listingDetailResponse.SaleProperty.PropertyInfo.LegalDescription, // check
                TaxID = this.listingDetailResponse.SaleProperty.PropertyInfo.TaxId,
                MLSArea = this.listingDetailResponse.SaleProperty.PropertyInfo.MlsArea?.ToStringFromEnumMember(),
                MapscoMapBook = this.listingDetailResponse.SaleProperty.PropertyInfo.MapscoGrid, // check
                MapscoMapCoord = this.listingDetailResponse.SaleProperty.PropertyInfo.MapscoGrid,
                LotDim = this.listingDetailResponse.SaleProperty.PropertyInfo.LotDimension,
                LotSize = this.listingDetailResponse.SaleProperty.PropertyInfo.LotSize,
                LotDesc = this.listingDetailResponse.SaleProperty.PropertyInfo.LotDescription.ToStringFromEnumMembers(),
                Occupancy = this.listingDetailResponse.SaleProperty.PropertyInfo.Occupancy.ToStringFromEnumMembers(),
                //// UpdateGeoCodes
                Latitude = this.listingDetailResponse.SaleProperty.PropertyInfo.Latitude,
                Longitude = this.listingDetailResponse.SaleProperty.PropertyInfo.Longitude,
                //// isBxlManaged
                Category = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.TypeCategory.ToStringFromEnumMember(), // check
                NumStories = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.Stories?.ToStringFromEnumMember(),
                SqFtTotal = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SqFtTotal,
                SqFtSource = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SqFtSource?.ToStringFromEnumMember(),
                //// EntryLength
                //// EntryWidth
                //// SpecialtyRooms
                //// MasterBedrrom
                //// numBedrooms
                InteriorDesc = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SpecialtyRooms.ToStringFromEnumMembers(),
                BathsFull = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.BathsFull,
                BathsHalf = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.BathsHalf,
                //// masterBathDescription
                //// GarageDesc = listingResponse.SaleProperty.SpacesDimensionsInfo.GarageDescription.ToStringFromEnumMembers(),
                ParkingDesc = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.GarageDescription.ToStringFromEnumMembers(),
                OtherParking = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.OtherParking?.ToStringFromEnumMembers(),
                Beds = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.NumBedrooms,
                //// PropertyDescription
                InclusionsDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Inclusions?.ToStringFromEnumMembers(),
                NumberFireplaces = this.listingDetailResponse.SaleProperty.FeaturesInfo.Fireplaces.ToFireplaceOption(),
                FireplaceDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.FireplaceDescription.ToStringFromEnumMembers(),
                FloorsDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Floors?.ToStringFromEnumMembers(),
                WindowCoverings = this.listingDetailResponse.SaleProperty.FeaturesInfo.WindowCoverings?.ToStringFromEnumMembers(),
                //// HasAccessibility
                AccessibilityDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Accessibility.ToStringFromEnumMembers(),
                HousingStyleDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HousingStyle.ToStringFromEnumMembers(),
                ExteriorFeatures = this.listingDetailResponse.SaleProperty.FeaturesInfo.ExteriorFeatures.ToStringFromEnumMembers(),
                RoofDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.RoofDescription.ToStringFromEnumMembers(),
                FoundationDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Foundation.ToStringFromEnumMembers(),
                ExteriorDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Exterior.ToStringFromEnumMembers(),
                HasPool = this.listingDetailResponse.SaleProperty.FeaturesInfo.HasPrivatePool,
                PoolDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.PrivatePool.ToStringFromEnumMembers(),
                FacesDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HomeFaces.ToStringFromEnumMembers(), // check
                SupElectricity = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierElectricity,
                SupWater = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierWater,
                SupGarbage = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierGarbage,
                SupGas = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierGas,
                SupSewer = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierSewer,
                SupOther = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierOther,
                HeatSystemDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HeatSystem.ToStringFromEnumMembers(),
                CoolSystemDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.CoolingSystem.ToStringFromEnumMembers(),
                HeatingFuel = this.listingDetailResponse.SaleProperty.FeaturesInfo.HeatingFuel.ToStringFromEnumMembers(),
                WaterDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.WaterSewer.ToStringFromEnumMembers(),
                GreenCerts = this.listingDetailResponse.SaleProperty.FeaturesInfo.GreenCertification.ToStringFromEnumMembers(),
                EnergyDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.EnergyFeatures.ToStringFromEnumMembers(), // check
                GreenFeatures = this.listingDetailResponse.SaleProperty.FeaturesInfo.GreenFeatures.ToStringFromEnumMembers(),
                //// CommonFeatures = listingResponse.this.SaleProperty.FeaturesInfo.NeighborhoodAmenities, //check
                //// lotImprovements
                TaxRate = this.listingDetailResponse.SaleProperty.FinancialInfo.TaxRate,
                TaxYear = this.listingDetailResponse.SaleProperty.FinancialInfo.TaxYear,
                IsMultiParcel = this.listingDetailResponse.SaleProperty.FinancialInfo.IsMultipleTaxed.ToString(), // check
                TitleCo = this.listingDetailResponse.SaleProperty.FinancialInfo.TitleCompany,
                PROPSDTRMS = this.listingDetailResponse.SaleProperty.FinancialInfo.ProposedTerms.ToStringFromEnumMembers(),
                HasMultipleHOA = this.listingDetailResponse.SaleProperty.FinancialInfo.HasMultipleHOA.ToString(),
                AgentBonusAmount = this.listingDetailResponse.SaleProperty.FinancialInfo.AgentBonusAmount.ToString(),
                CompBuyBonusExpireDate = this.listingDetailResponse.SaleProperty.FinancialInfo.BonusExpirationDate, // check
                BuyerIncentive = this.listingDetailResponse.SaleProperty.FinancialInfo.HasBuyerIncentive.ToString(), // check
                CompBuy = this.listingDetailResponse.SaleProperty.FinancialInfo.BuyersAgentCommission?.ToString(),
                AltPhoneCommunity = this.listingDetailResponse.SaleProperty.ShowingInfo.AltPhoneCommunity,
                AgentListApptPhone = this.listingDetailResponse.SaleProperty.ShowingInfo.AgentListApptPhone,
                Showing = this.listingDetailResponse.SaleProperty.ShowingInfo.Showing?.ToStringFromEnumMember(),
                RealtorContactEmail = this.listingDetailResponse.SaleProperty.ShowingInfo.RealtorContactEmail,
                Directions = this.listingDetailResponse.SaleProperty.ShowingInfo.Directions,
                AgentPrivateRemarks = this.listingDetailResponse.SaleProperty.ShowingInfo.AgentPrivateRemarks,
                SchoolDistrict = this.listingDetailResponse.SaleProperty.SchoolsInfo.SchoolDistrict?.ToStringFromEnumMember(),
                SchoolName1 = this.listingDetailResponse.SaleProperty.SchoolsInfo.ElementarySchool?.ToStringFromEnumMember(),
                SchoolName2 = this.listingDetailResponse.SaleProperty.SchoolsInfo.MiddleSchool?.ToStringFromEnumMember(),
                SchoolName3 = this.listingDetailResponse.SaleProperty.SchoolsInfo.HighSchool?.ToStringFromEnumMember(),
                HOA = this.listingDetailResponse.SaleProperty.FinancialInfo.HOARequirement?.ToStringFromEnumMember(),
                PublicRemarks = this.listingDetailResponse.SaleProperty.FeaturesInfo.PropertyDescription,
            };

            if (this.listingDetailResponse.SaleProperty.Hoas != null && this.listingDetailResponse.SaleProperty.Hoas.Any())
            {
                var hoa = this.listingDetailResponse.SaleProperty.Hoas.First();
                residentialListingRequest.AssocFee = (int)hoa.Fee;
                residentialListingRequest.AssocName = hoa.Name;
                residentialListingRequest.AssocFeePaid = hoa.BillingFrequency.ToStringFromEnumMember();
                residentialListingRequest.AssocTransferFee = (int)hoa.TransferFee;
                residentialListingRequest.AssocPhone = hoa.ContactPhone;
            }

            foreach (var room in this.listingDetailResponse.SaleProperty.Rooms)
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
                    case RoomType.Student:
                    case RoomType.Other:
                    case RoomType.Office:
                    case RoomType.HalfBath:
                    case RoomType.FullBath:
                    default:
                        break;
                }
            }

            foreach (var openHouse in this.listingDetailResponse.SaleProperty.OpenHouses)
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

            return residentialListingRequest;
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
    }
}
