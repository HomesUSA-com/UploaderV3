namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using System.Linq;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Contracts.Response;
    using Husa.Quicklister.Abor.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Abor.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class AborListingRequest : ResidentialListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

        private readonly ListingSaleRequestQueryResponse listingResponse;
        private readonly ListingSaleRequestDetailResponse listingDetailResponse;

        public AborListingRequest(ListingSaleRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public AborListingRequest(ListingSaleRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private AborListingRequest()
            : base()
        {
            this.InExtraTerritorialJurisdiction = false;
            this.IsManufacturedAllowed = false;
            this.GeographicID = DefaultIntegerAsStringValue;
            this.EarnestMoney = DefaultIntegerAsStringValue;
            this.ProspectsExempt = DefaultIntegerAsStringValue;
            this.Rooms = new List<ResidentialListingRequestRoom>();
            this.OpenHouse = new List<OpenHouseRequest>();
        }

        public override MarketCode MarketCode => MarketCode.Austin;
        public override BuiltStatus BuiltStatus => this.YearBuiltDesc switch
        {
            "TB" => BuiltStatus.ToBeBuilt,
            "NW" => BuiltStatus.ReadyNow,
            "UC" => BuiltStatus.WithCompletion,
            _ => BuiltStatus.WithCompletion,
        };

        public bool HasHoa { get; set; }
        public string PatioAndPorchFeatures { get; set; }

        public override ResidentialListingRequest CreateFromApiResponse() => new AborListingRequest()
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
            ListPrice = this.listingResponse.ListPrice.HasValue ? (int)this.listingResponse.ListPrice.Value : 0,
            ListStatus = this.listingResponse.MlsStatus.ToStringFromEnumMember(),
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new AborListingRequest
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
                ExpectedActiveDate = DateTime.Now.ToString("MM/dd/yy"),
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
            FillStatusInfo(this.listingDetailResponse.StatusFieldsInfo);
            FillRoomsInfo(this.listingDetailResponse.SaleProperty.Rooms);
            FillOpenHouseInfo(this.listingDetailResponse.SaleProperty.OpenHouses);

            return residentialListingRequest;

            void FillSalePropertyInfo(SalePropertyResponse salePropertyInfo)
            {
                if (salePropertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(salePropertyInfo));
                }

                residentialListingRequest.BuilderName = salePropertyInfo.OwnerName;
                residentialListingRequest.CompanyName = salePropertyInfo.OwnerName;
                residentialListingRequest.OwnerName = salePropertyInfo.OwnerName;
                residentialListingRequest.PlanProfileName = salePropertyInfo.PlanName;
                residentialListingRequest.CompanyId = salePropertyInfo.CompanyId;
            }

            void FillAddressInfo(AddressInfoResponse addressInfo)
            {
                if (addressInfo is null)
                {
                    throw new ArgumentNullException(nameof(addressInfo));
                }

                residentialListingRequest.StreetNum = addressInfo.StreetNumber;
                residentialListingRequest.StreetName = addressInfo.StreetName;
                residentialListingRequest.City = addressInfo.City.GetEnumDescription();
                residentialListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                residentialListingRequest.State = addressInfo.State.ToStringFromEnumMember();
                residentialListingRequest.Zip = addressInfo.ZipCode;
                residentialListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                residentialListingRequest.StreetType = addressInfo.StreetType?.ToStringFromEnumMember();
                residentialListingRequest.UnitNum = addressInfo.UnitNumber;
                residentialListingRequest.Subdivision = addressInfo.Subdivision;
                residentialListingRequest.StateCode = addressInfo.State.ToStringFromEnumMember();
            }

            void FillPropertyInfo(PropertyInfoResponse propertyInfo)
            {
                if (propertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                residentialListingRequest.BuildCompletionDate = propertyInfo.ConstructionCompletionDate;
                residentialListingRequest.YearBuiltDesc = propertyInfo.ConstructionStage?.ToStringFromEnumMember();
                residentialListingRequest.YearBuilt = propertyInfo.ConstructionStartYear;
                residentialListingRequest.Legal = propertyInfo.LegalDescription;
                residentialListingRequest.TaxID = propertyInfo.TaxId;
                residentialListingRequest.MLSArea = propertyInfo.MlsArea?.ToStringFromEnumMember();
                residentialListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                residentialListingRequest.Latitude = propertyInfo.Latitude;
                residentialListingRequest.Longitude = propertyInfo.Longitude;
                residentialListingRequest.LotDim = propertyInfo.LotDimension;
                residentialListingRequest.LotSize = propertyInfo.LotSize;
                residentialListingRequest.LotDesc = propertyInfo.LotDescription.ToStringFromEnumMembers();
                residentialListingRequest.OtherFees = propertyInfo.TaxLot;
                residentialListingRequest.PropSubType = propertyInfo.PropertyType?.ToStringFromEnumMember();
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                if (spacesDimensionsInfo is null)
                {
                    throw new ArgumentNullException(nameof(spacesDimensionsInfo));
                }

                residentialListingRequest.NumStories = spacesDimensionsInfo.StoriesTotal?.ToStringFromEnumMember();
                residentialListingRequest.NumDiningAreas = spacesDimensionsInfo.DiningAreasTotal;
                residentialListingRequest.NumLivingAreas = spacesDimensionsInfo.LivingAreasTotal;
                residentialListingRequest.BathsFull = spacesDimensionsInfo.FullBathsTotal;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.HalfBathsTotal;
                residentialListingRequest.NumBedsMainLevel = spacesDimensionsInfo.MainLevelBedroomTotal;
                residentialListingRequest.NumBedsOtherLevels = spacesDimensionsInfo.OtherLevelsBedroomTotal;
                residentialListingRequest.SqFtTotal = spacesDimensionsInfo.SqFtTotal;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                if (featuresInfo is null)
                {
                    throw new ArgumentNullException(nameof(featuresInfo));
                }

                residentialListingRequest.ExteriorDesc = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.FacesDesc = featuresInfo.HomeFaces?.ToStringFromEnumMember();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.ConstructionDesc = featuresInfo.ConstructionMaterials.ToStringFromEnumMembers();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.NumberFireplaces = featuresInfo.Fireplaces?.ToString();
                residentialListingRequest.FloorsDesc = featuresInfo.Floors.ToStringFromEnumMembers();
                residentialListingRequest.GarageDesc = featuresInfo.GarageDescription.ToStringFromEnumMembers();
                residentialListingRequest.AppliancesDesc = featuresInfo.Appliances.ToStringFromEnumMembers();
                residentialListingRequest.FenceDesc = featuresInfo.Fencing.ToStringFromEnumMembers();
                residentialListingRequest.WaterfrontFeatures = featuresInfo.WaterfrontFeatures.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                residentialListingRequest.BodyofWater = featuresInfo.WaterBodyName?.ToStringFromEnumMember();
                residentialListingRequest.DistanceToWaterAccess = featuresInfo.DistanceToWaterAccess?.ToStringFromEnumMember();
                residentialListingRequest.GreenWaterConservation = featuresInfo.WaterSource.ToStringFromEnumMembers();
                residentialListingRequest.RestrictionsDesc = featuresInfo.RestrictionsDescription.ToStringFromEnumMembers();
                residentialListingRequest.CommonFeatures = featuresInfo.NeighborhoodAmenities.ToStringFromEnumMembers();
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.SecurityDesc = featuresInfo.SecurityFeatures.ToStringFromEnumMembers();
                residentialListingRequest.UtilitiesDesc = featuresInfo.UtilitiesDescription.ToStringFromEnumMembers();
                residentialListingRequest.WindowCoverings = featuresInfo.WindowFeatures.ToStringFromEnumMembers();
                residentialListingRequest.UnitStyleDesc = featuresInfo.UnitStyle.ToStringFromEnumMembers();
                residentialListingRequest.GarageCapacity = featuresInfo.GarageSpaces;
                residentialListingRequest.LaundryLocDesc = featuresInfo.LaundryLocation.ToStringFromEnumMembers();
                residentialListingRequest.InteriorDesc = featuresInfo.InteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.GuestAccommodationsDesc = featuresInfo.GuestAccommodationsDescription.ToStringFromEnumMembers();
                residentialListingRequest.PublicRemarks = featuresInfo.PropertyDescription;
                residentialListingRequest.NumGuestBeds = featuresInfo.GuestBedroomsTotal;
                residentialListingRequest.NumGuestHalfBaths = featuresInfo.GuestHalfBathsTotal;
                residentialListingRequest.NumGuestFullBaths = featuresInfo.GuestFullBathsTotal;
                residentialListingRequest.PatioAndPorchFeatures = featuresInfo.PatioAndPorchFeatures.ToStringFromEnumMembers();
                residentialListingRequest.ViewDesc = featuresInfo.View.ToStringFromEnumMembers();
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                residentialListingRequest.FinancingProposed = financialInfo.AcceptableFinancing.ToStringFromEnumMembers();
                residentialListingRequest.ExemptionsDesc = financialInfo.TaxExemptions.ToStringFromEnumMembers();
                residentialListingRequest.TaxRate = financialInfo.TaxRate.StrictDecimalToString();
                residentialListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                residentialListingRequest.TitleCo = financialInfo.TitleCompany;
                residentialListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount.DecimalToString();
                residentialListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = financialInfo.BonusExpirationDate;
                residentialListingRequest.BuyerCheckBox = financialInfo.HasBuyerIncentive;
                residentialListingRequest.BuyerIncentive = financialInfo.BuyersAgentCommission.DecimalToString();
                residentialListingRequest.BuyerIncentiveDesc = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.HasHoa = financialInfo.HasHoa;
                residentialListingRequest.HOA = financialInfo.HOARequirement?.ToStringFromEnumMember();
                residentialListingRequest.AssocName = financialInfo.HoaName;
                residentialListingRequest.AssocFee = financialInfo.HoaFee.HasValue ? decimal.ToInt32(financialInfo.HoaFee.Value) : 0;
                residentialListingRequest.AssocFeeIncludes = financialInfo.HoaIncludes.ToStringFromEnumMembers();
                residentialListingRequest.AssocFeeFrequency = financialInfo.BillingFrequency?.ToStringFromEnumMember();
            }

            void FillShowingInfo(ShowingResponse showingInfo)
            {
                if (showingInfo is null)
                {
                    throw new ArgumentNullException(nameof(showingInfo));
                }

                residentialListingRequest.AgentListApptPhone = showingInfo.ContactPhone;
                residentialListingRequest.OtherPhone = showingInfo.OccupantPhone;
                residentialListingRequest.LockboxTypeDesc = showingInfo.LockBoxType?.ToStringFromEnumMember();
                residentialListingRequest.LockboxLocDesc = showingInfo.LockBoxSerialNumber;
                residentialListingRequest.ShowingInstructions = showingInfo.ShowingInstructions;
                residentialListingRequest.Showing = showingInfo.ShowingRequirements.ToStringFromEnumMembers();
                residentialListingRequest.RealtorContactEmail = showingInfo.RealtorContactEmail;
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
                residentialListingRequest.AgentPrivateRemarks2 = showingInfo.AgentPrivateRemarksAdditional;
                residentialListingRequest.EnableOpenHouse = showingInfo.EnableOpenHouses;
                residentialListingRequest.AllowPendingList = showingInfo.ShowOpenHousesPending;
                residentialListingRequest.OwnerName = showingInfo.OwnerName;
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
                residentialListingRequest.SchoolName4 = schoolsInfo.OtherElementarySchool;
                residentialListingRequest.SchoolName5 = schoolsInfo.OtherMiddleSchool;
                residentialListingRequest.SchoolName6 = schoolsInfo.OtherHighSchool;
            }

            void FillStatusInfo(ListingSaleStatusFieldsResponse statusInfo)
            {
                if (statusInfo is null)
                {
                    throw new ArgumentNullException(nameof(statusInfo));
                }

                residentialListingRequest.SoldPrice = statusInfo.ClosePrice;
                residentialListingRequest.ClosedDate = statusInfo.ClosedDate;
                residentialListingRequest.PendingDate = statusInfo.PendingDate;
                residentialListingRequest.SellConcess = statusInfo.SellConcess;
                residentialListingRequest.ContractDate = statusInfo.ContractDate;
                residentialListingRequest.OffMarketDate = statusInfo.OffMarketDate;
                residentialListingRequest.BackOnMarketDate = statusInfo.BackOnMarketDate;
                residentialListingRequest.SoldTerms = statusInfo.SaleTerms.ToStringFromEnumMembers();
                residentialListingRequest.HasContingencyInfo = statusInfo.HasContingencyInfo;
                residentialListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                residentialListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                residentialListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
                residentialListingRequest.ContingencyInfo = statusInfo.ContingencyInfo.ToStringFromEnumMembers();
            }

            void FillRoomsInfo(IEnumerable<RoomResponse> rooms)
            {
                if (rooms == null || !rooms.Any())
                {
                    return;
                }

                foreach (var room in rooms)
                {
                    this.Rooms.Add(new()
                    {
                        Level = room.Level.ToStringFromEnumMember(),
                        RoomType = room.RoomType.ToStringFromEnumMember(),
                        Features = room.Features.ToStringFromEnumMembers(),
                    });
                }

                residentialListingRequest.Rooms = this.Rooms;
            }

            void FillOpenHouseInfo(IEnumerable<OpenHouseResponse> openHouses)
            {
                if (openHouses == null || !openHouses.Any())
                {
                    return;
                }

                foreach (var openHouse in openHouses)
                {
                    this.OpenHouse.Add(new()
                    {
                        Date = OpenHouseExtensions.GetNextWeekday(DateTime.Today, Enum.Parse<DayOfWeek>(openHouse.Type.ToString(), ignoreCase: true)),
                        StartTime = openHouse.StartTime,
                        EndTime = openHouse.EndTime,
                        Active = true,
                        Comments = OpenHouseExtensions.GetComments(openHouse.Refreshments, openHouse.Lunch),
                        Type = OpenHouseType.Public,
                    });
                }

                residentialListingRequest.OpenHouse = this.OpenHouse;
            }
        }

        public override string GetBuyerAgentComp(string compBuy, string compBuyType)
        {
            throw new NotImplementedException();
        }

        public override string GetAgentBonusRemarksMessage()
        {
            var agentBonusAmount = this.GetAgentBonusAmount();
            if (string.IsNullOrWhiteSpace(agentBonusAmount))
            {
                return base.GetAgentBonusRemarksMessage();
            }

            var agentAmount = agentBonusAmount + " Bonus. ";
            var hasBuyerIncentive = this.BuyerCheckBox.HasValue && this.BuyerCheckBox.Value;
            return hasBuyerIncentive
                ? agentAmount + "Contact Builder for Buyer Incentive Information. "
                : agentAmount;
        }
    }
}
