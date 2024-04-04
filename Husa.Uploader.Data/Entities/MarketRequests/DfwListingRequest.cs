namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using System.Linq;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Api.Contracts.Response;
    using Husa.Quicklister.Dfw.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Dfw.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class DfwListingRequest : ResidentialListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

        private readonly SaleListingRequestQueryResponse listingResponse;
        private readonly SaleListingRequestDetailResponse listingDetailResponse;

        public DfwListingRequest(SaleListingRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public DfwListingRequest(SaleListingRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private DfwListingRequest()
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

        public bool HasHoa { get; set; }
        public bool HasDishwasher { get; set; }
        public bool HasCompactor { get; set; }
        public bool HasCommunityPool { get; set; }
        public bool HasIcemaker { get; set; }
        public bool HasDisposal { get; set; }
        public bool HasMicrowave { get; set; }
        public bool IsActiveCommunity { get; set; }
        public bool HasUtilitiesDescription { get; set; }
        public bool HasOtherFees { get; set; }
        public string WasherConnections { get; set; }
        public bool AdultCommunity { get; set; }
        public bool HasAccessibility { get; set; }
        public bool IsSmartHome { get; set; }
        public bool HasGarage { get; set; }
        public bool HasAttachedGarage { get; set; }
        public string PatioAndPorchFeatures { get; set; }
        public string ShowingContactType { get; set; }
        public bool HasMudDistrict { get; set; }
        public bool IsSpecialTaxingAuthority { get; set; }
        public string HoaTerm { get; set; }
        public bool HasPropertyAttached { get; set; }

        public override MarketCode MarketCode => MarketCode.DFW;
        public override BuiltStatus BuiltStatus => this.YearBuiltDesc switch
        {
            "BEBLT" => BuiltStatus.ToBeBuilt,
            "NVLIV" => BuiltStatus.ReadyNow,
            _ => BuiltStatus.WithCompletion,
        };

        public override ResidentialListingRequest CreateFromApiResponse() => new DfwListingRequest()
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
            EnableOpenHouse = this.listingResponse.EnableOpenHouse,
            UpdateGeocodes = this.listingResponse.UpdateGeocodes,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new DfwListingRequest
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
                SellerBuyerCost = this.SellerBuyerCost,
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
                residentialListingRequest.TaxID = propertyInfo.TaxId;
                residentialListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                residentialListingRequest.PropSubType = propertyInfo.PropertySubType?.ToStringFromEnumMember();
                residentialListingRequest.LotDim = propertyInfo.LotDimension;
                residentialListingRequest.LotDesc = propertyInfo.LotDescription.ToStringFromEnumMembers();
                residentialListingRequest.LotSize = propertyInfo.LotSize?.ToStringFromEnumMember();
                residentialListingRequest.LotSizeSrc = "OTHER";
                residentialListingRequest.IsNewConstruction = propertyInfo.IsNewConstruction;
                residentialListingRequest.HousingTypeDesc = propertyInfo.HousingType?.ToStringFromEnumMembers();
                residentialListingRequest.LotSizeAcres = propertyInfo.Acres.ToString();
                residentialListingRequest.HasPropertyAttached = propertyInfo.HasPropertyAttached;
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                if (spacesDimensionsInfo is null)
                {
                    throw new ArgumentNullException(nameof(spacesDimensionsInfo));
                }

                residentialListingRequest.Beds = spacesDimensionsInfo.NumBedrooms;
                residentialListingRequest.BathsFull = spacesDimensionsInfo.NumBathsFull;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.NumBathsHalf;
                residentialListingRequest.NumStories = spacesDimensionsInfo.Stories?.ToStringFromEnumMember();
                residentialListingRequest.SqFtTotal = spacesDimensionsInfo.SqftTotal;
                residentialListingRequest.SqFtSource = spacesDimensionsInfo.SqftSource?.ToStringFromEnumMember();
                residentialListingRequest.NumLivingAreas = spacesDimensionsInfo.NumLivingAreas;
                residentialListingRequest.NumDiningAreas = spacesDimensionsInfo.NumDiningAreas;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                if (featuresInfo is null)
                {
                    throw new ArgumentNullException(nameof(featuresInfo));
                }

                residentialListingRequest.ExteriorDesc = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.ConstructionDesc = featuresInfo.ConstructionMaterials.ToStringFromEnumMembers();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.NumberFireplaces = featuresInfo.NumFireplaces?.ToString();
                residentialListingRequest.FloorsDesc = featuresInfo.Floors.ToStringFromEnumMembers();
                residentialListingRequest.GarageDesc = featuresInfo.GarageDescription.ToStringFromEnumMembers();
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.GarageCapacity = featuresInfo.GarageSpaces;
                residentialListingRequest.InteriorDesc = featuresInfo.InteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.PublicRemarks = featuresInfo.PropertyDescription;
                residentialListingRequest.GreenFeatures = featuresInfo.GreenCertification.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.GreenWater.ToStringFromEnumMembers();
                residentialListingRequest.HasPool = featuresInfo.HasPool;
                residentialListingRequest.PoolDesc = featuresInfo.Pool.ToStringFromEnumMembers();
                residentialListingRequest.HousingStyleDesc = featuresInfo.HousingStyle.ToStringFromEnumMembers();
                residentialListingRequest.AdultCommunity = featuresInfo.AdultCommunity;
                residentialListingRequest.HasAccessibility = featuresInfo.HasAccessibility;
                residentialListingRequest.AccessibilityDesc = featuresInfo.Accessibility.ToStringFromEnumMembers();
                residentialListingRequest.IsSmartHome = featuresInfo.IsSmartHome;
                residentialListingRequest.CarportCapacity = featuresInfo.CarpotSpaces;
                residentialListingRequest.HasGarage = featuresInfo.HasGarage;
                residentialListingRequest.HasAttachedGarage = featuresInfo.HasAttachedGarage;
                residentialListingRequest.GarageLength = featuresInfo.GarageLength;
                residentialListingRequest.GarageWidth = featuresInfo.GarageWidth;
                residentialListingRequest.AppliancesDesc = featuresInfo.Appliances.ToStringFromEnumMembers();
                residentialListingRequest.SecurityDesc = featuresInfo.SecurityFeatures.ToStringFromEnumMembers();
                residentialListingRequest.LaundryFacilityDesc = featuresInfo.LaundryFeatures.ToStringFromEnumMembers();
                residentialListingRequest.PatioAndPorchFeatures = featuresInfo.PatioAndPorchFeatures.ToStringFromEnumMembers();
                residentialListingRequest.FenceDesc = featuresInfo.Fencing.ToStringFromEnumMembers();
                residentialListingRequest.UtilitiesDesc = featuresInfo.UtilitiesDescription.ToStringFromEnumMembers();
                residentialListingRequest.HasMudDistrict = featuresInfo.HasMudDistrict;
                residentialListingRequest.IsSpecialTaxingAuthority = featuresInfo.IsSpecialTaxingAuthority;
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                residentialListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount.DecimalToString();
                residentialListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = financialInfo.BonusExpirationDate;
                residentialListingRequest.BuyerCheckBox = financialInfo.HasBuyerIncentive;
                residentialListingRequest.BuyerIncentive = financialInfo.BuyersAgentCommission.DecimalToString();
                residentialListingRequest.BuyerIncentiveDesc = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.HOA = financialInfo.HOARequirement?.ToStringFromEnumMember();
                residentialListingRequest.AssocFee = financialInfo.HoaFee.HasValue ? decimal.ToInt32(financialInfo.HoaFee.Value) : 0;
                residentialListingRequest.AssocPhone = financialInfo.HoaPhone;
                residentialListingRequest.TitleCo = financialInfo.TitleCompany;
                residentialListingRequest.TitleCoPhone = financialInfo.TitleCompanyPhone;
                residentialListingRequest.TitleCoLocation = financialInfo.TitleCompanyLocation;
                residentialListingRequest.HoaTerm = financialInfo.HoaTerm?.ToStringFromEnumMember();
                residentialListingRequest.AssocName = financialInfo.HoaManagement;
                residentialListingRequest.AssocFeeIncludes = financialInfo.HoaIncludes?.ToStringFromEnumMembers();
            }

            void FillShowingInfo(ShowingResponse showingInfo)
            {
                if (showingInfo is null)
                {
                    throw new ArgumentNullException(nameof(showingInfo));
                }

                residentialListingRequest.AgentListApptPhone = showingInfo.ContactPhone;
                residentialListingRequest.OtherPhone = showingInfo.OccupantPhone;
                residentialListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
                residentialListingRequest.ShowingInstructions = showingInfo.ShowingInstructions;
                residentialListingRequest.RealtorContactEmail = showingInfo.RealtorContactEmail.ToStringFromCollection(";");
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.EnableOpenHouse = showingInfo.EnableOpenHouses;
                residentialListingRequest.AllowPendingList = showingInfo.ShowOpenHousesPending;
                residentialListingRequest.Showing = showingInfo.ShowingRequirements.ToStringFromEnumMembers();
                residentialListingRequest.ShowingContactType = showingInfo.ShowingContactType.ToStringFromEnumMembers();
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
                residentialListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName7 = schoolsInfo.IntermediateSchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName5 = schoolsInfo.JuniorSchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName6 = schoolsInfo.SeniorSchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName4 = schoolsInfo.PrimarySchool?.ToStringFromEnumMember();
            }

            void FillStatusInfo(SaleListingStatusFieldsResponse statusInfo)
            {
                if (statusInfo is null)
                {
                    throw new ArgumentNullException(nameof(statusInfo));
                }

                residentialListingRequest.SoldPrice = statusInfo.ClosePrice.HasValue ? Math.Floor(statusInfo.ClosePrice.Value) : 0;
                residentialListingRequest.ContractDate = statusInfo.ContractDate;
                residentialListingRequest.ContingencyInfo = statusInfo.ContingencyInfo;
                residentialListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                residentialListingRequest.PendingDate = statusInfo.PendingDate;
                residentialListingRequest.ClosedDate = statusInfo.ClosedDate;
                residentialListingRequest.SellConcess = statusInfo.SellConcess;
                residentialListingRequest.OffMarketDate = statusInfo.OffMarketDate;
                residentialListingRequest.BackOnMarketDate = statusInfo.BackOnMarketDate;
                residentialListingRequest.MortgageCoSold = statusInfo.MortgageCompany;
                residentialListingRequest.SoldTerms = statusInfo.SoldTerms?.ToStringFromEnumMember();
                residentialListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                residentialListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
                residentialListingRequest.HasBuyerAgent = statusInfo.HasBuyerAgent;
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
                        Width = room.Width,
                        Length = room.Length,
                        Level = room.Level.ToStringFromEnumMember(),
                        RoomType = room.RoomType.ToStringFromEnumMember(),
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
                        Refreshments = openHouse.Refreshments.ToStringFromEnumMembers(),
                        Type = OpenHouseType.Public,
                    });
                }

                residentialListingRequest.OpenHouse = this.OpenHouse;
            }
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

        public string GetAgentRemarksMessage(string constructionStage)
        {
            var privateRemarks = "LIMITED SERVICE LISTING: Buyer verifies dimensions & ISD info. Use Bldr contract.";

            var bonusMessage = string.IsNullOrWhiteSpace(this.MLSNum) ? this.GetAgentBonusRemarksMessage() : string.Empty;
            if (!string.IsNullOrWhiteSpace(bonusMessage))
            {
                privateRemarks = $"{bonusMessage} {privateRemarks}";
            }

            var saleOfficeInfo = this.GetSalesAssociateRemarksMessage();
            if (!string.IsNullOrWhiteSpace(saleOfficeInfo))
            {
                privateRemarks += $" {saleOfficeInfo}";
            }

            if (!string.IsNullOrWhiteSpace(this.PlanProfileName))
            {
                privateRemarks += $" Plan: {this.PlanProfileName}.";
            }

            if (!string.IsNullOrWhiteSpace(this.RealtorContactEmail))
            {
                privateRemarks += $" Email contact: {this.RealtorContactEmail}.";
            }

            const string homeUnderConstruction = "Home is under construction. For your safety, call appt number for showings.";

            return constructionStage != "NCC" ? $"{homeUnderConstruction} {privateRemarks}" : privateRemarks;
        }
    }
}
