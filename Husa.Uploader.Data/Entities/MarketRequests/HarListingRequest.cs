namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using System.Linq;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Har.Api.Contracts.Response;
    using Husa.Quicklister.Har.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Har.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class HarListingRequest : ResidentialListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

        private readonly ListingSaleRequestQueryResponse listingResponse;
        private readonly ListingSaleRequestDetailResponse listingDetailResponse;

        public HarListingRequest(ListingSaleRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public HarListingRequest(ListingSaleRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private HarListingRequest()
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

        public override MarketCode MarketCode => MarketCode.Houston;
        public override BuiltStatus BuiltStatus => this.YearBuiltDesc switch
        {
            "BEBLT" => BuiltStatus.ToBeBuilt,
            "NVLIV" => BuiltStatus.ReadyNow,
            _ => BuiltStatus.WithCompletion,
        };

        public override ResidentialListingRequest CreateFromApiResponse() => new HarListingRequest()
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
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new HarListingRequest
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
                residentialListingRequest.Legal = propertyInfo.LegalDescription;
                residentialListingRequest.TaxID = propertyInfo.TaxId;
                residentialListingRequest.MLSArea = propertyInfo.MlsArea?.ToStringFromEnumMember();
                residentialListingRequest.MLSSubArea = propertyInfo.SubArea?.ToStringFromEnumMember();
                residentialListingRequest.SectionNum = propertyInfo.SectionNum;
                residentialListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                residentialListingRequest.Latitude = propertyInfo.Latitude;
                residentialListingRequest.Longitude = propertyInfo.Longitude;
                residentialListingRequest.PropSubType = propertyInfo.PropertyType.ToStringFromEnumMembers();
                residentialListingRequest.LotDim = propertyInfo.LotDimension;
                residentialListingRequest.LotDesc = propertyInfo.LotDescription.ToStringFromEnumMembers();
                residentialListingRequest.LotSize = propertyInfo.LotSize?.ToStringFromEnumMember();
                residentialListingRequest.LotSizeAcres = propertyInfo.LotSizeSqft?.ToString();
                residentialListingRequest.LotSizeSrc = propertyInfo.LotSizeSource?.ToStringFromEnumMember();
                residentialListingRequest.MapscoMapCoord = propertyInfo.MapscoGrid;
                residentialListingRequest.IsNewConstruction = propertyInfo.IsNewConstruction;
                residentialListingRequest.LegalSubdivision = propertyInfo.LegalSubdivision?.ToStringFromEnumMember();
                residentialListingRequest.IsPlannedDevelopment = propertyInfo.IsPlannedCommunity;
                residentialListingRequest.PlannedDevelopment = propertyInfo.PlannedCommunity?.GetEnumDescription();
                residentialListingRequest.HousingTypeDesc = propertyInfo.HousingType?.ToStringFromEnumMember();
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                if (spacesDimensionsInfo is null)
                {
                    throw new ArgumentNullException(nameof(spacesDimensionsInfo));
                }

                residentialListingRequest.Beds = spacesDimensionsInfo.NumBedrooms;
                residentialListingRequest.BathsFull = spacesDimensionsInfo.BathsFull;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.BathsHalf;
                residentialListingRequest.BedroomDescription = spacesDimensionsInfo.BedroomDescription.ToStringFromEnumMembers();
                residentialListingRequest.RoomDescription = spacesDimensionsInfo.RoomDescription.ToStringFromEnumMembers();
                residentialListingRequest.BedBathDesc = spacesDimensionsInfo.PrimaryBathDescription.ToStringFromEnumMembers();
                residentialListingRequest.KitchenDescription = spacesDimensionsInfo.KitchenDescription.ToStringFromEnumMembers();
                residentialListingRequest.NumStories = spacesDimensionsInfo.Stories?.ToString();
                residentialListingRequest.SqFtTotal = spacesDimensionsInfo.SqftTotal;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                if (featuresInfo is null)
                {
                    throw new ArgumentNullException(nameof(featuresInfo));
                }

                residentialListingRequest.ExteriorDesc = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.FacesDesc = featuresInfo.HomeFaces.ToStringFromEnumMembers();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.ConstructionDesc = featuresInfo.ConstructionMaterials.ToStringFromEnumMembers();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.NumberFireplaces = featuresInfo.Fireplaces?.ToString();
                residentialListingRequest.FloorsDesc = featuresInfo.Floors.ToStringFromEnumMembers();
                residentialListingRequest.GarageDesc = featuresInfo.GarageDescription.ToStringFromEnumMembers();
                residentialListingRequest.WaterfrontFeatures = featuresInfo.WaterfrontFeatures.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                residentialListingRequest.RestrictionsDesc = featuresInfo.RestrictionsDescription.ToStringFromEnumMembers();
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.GarageCapacity = featuresInfo.GarageSpaces;
                residentialListingRequest.InteriorDesc = featuresInfo.InteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.PublicRemarks = featuresInfo.PropertyDescription;
                residentialListingRequest.HasDishwasher = featuresInfo.HasDishwasher;
                residentialListingRequest.HasCompactor = featuresInfo.HasCompactor;
                residentialListingRequest.HasDisposal = featuresInfo.HasDisposal;
                residentialListingRequest.HasIcemaker = featuresInfo.HasIcemaker;
                residentialListingRequest.HasCommunityPool = featuresInfo.HasCommunityPool;
                residentialListingRequest.OvenDesc = featuresInfo.OvenDescription.ToStringFromEnumMembers();
                residentialListingRequest.GolfCourseName = featuresInfo.GolfCourseName?.GetEnumDescription();
                residentialListingRequest.Disclosures = featuresInfo.Disclosures.ToStringFromEnumMembers();
                residentialListingRequest.EnergyDesc = featuresInfo.EnergyFeatures.ToStringFromEnumMembers();
                residentialListingRequest.HasMicrowave = featuresInfo.HasMicrowawe;
                residentialListingRequest.HasUtilitiesDescription = featuresInfo.HasUtilitiesDescription;
                residentialListingRequest.GreenCerts = featuresInfo.GreenCertification.ToStringFromEnumMembers();
                residentialListingRequest.HasPool = featuresInfo.HasPool;
                residentialListingRequest.PoolDesc = featuresInfo.Pool.ToStringFromEnumMembers();
                residentialListingRequest.CountertopsDesc = featuresInfo.Countertops;
                residentialListingRequest.HousingStyleDesc = featuresInfo.HousingStyle.ToStringFromEnumMembers();
                residentialListingRequest.AccessInstructionsDesc = featuresInfo.AccessInstructions.ToStringFromEnumMembers();
                residentialListingRequest.RangeDesc = featuresInfo.RangeDescription.ToStringFromEnumMembers();
                residentialListingRequest.WasherConnections = featuresInfo.WasherConnections.ToStringFromEnumMembers();

                residentialListingRequest.IsActiveCommunity = featuresInfo.IsActiveCommunity;
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                residentialListingRequest.FinancingProposed = financialInfo.AcceptableFinancing.ToStringFromEnumMembers();
                residentialListingRequest.ExemptionsDesc = financialInfo.TaxExemptions;
                residentialListingRequest.TaxRate = financialInfo.TaxRate.StrictDecimalToString();
                residentialListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                residentialListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount.DecimalToString();
                residentialListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = financialInfo.BonusExpirationDate;
                residentialListingRequest.BuyerCheckBox = financialInfo.HasBuyerIncentive;
                residentialListingRequest.BuyerIncentive = financialInfo.BuyersAgentCommission.DecimalToString();
                residentialListingRequest.BuyerIncentiveDesc = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.HOA = financialInfo.HOARequirement?.ToStringFromEnumMember();
                residentialListingRequest.HasHoa = financialInfo.HasHoa;
                residentialListingRequest.AssocName = financialInfo.HoaName;
                residentialListingRequest.AssocFee = financialInfo.HoaFee.HasValue ? decimal.ToInt32(financialInfo.HoaFee.Value) : 0;
                residentialListingRequest.AssocFeeFrequency = financialInfo.BillingFrequency?.ToStringFromEnumMember();
                residentialListingRequest.AssocPhone = financialInfo.HoaPhone;
                residentialListingRequest.OtherFeesInclude = financialInfo.OtherFeesInclude;
                residentialListingRequest.HasOtherFees = financialInfo.HasOtherFees;
                residentialListingRequest.OtherFees = financialInfo.OtherFeeAmount.ToString();
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
                residentialListingRequest.ShowingInstructions = showingInfo.ShowingInstructions.ToStringFromEnumMembers();
                residentialListingRequest.RealtorContactEmail = showingInfo.RealtorContactEmail.ToStringFromCollection(";");
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.EnableOpenHouse = showingInfo.EnableOpenHouses;
                residentialListingRequest.AllowPendingList = showingInfo.ShowOpenHousesPending;
            }

            void FillSchoolsInfo(SchoolsResponse schoolsInfo)
            {
                if (schoolsInfo is null)
                {
                    throw new ArgumentNullException(nameof(schoolsInfo));
                }

                residentialListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.ToStringFromEnumMember();
                residentialListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
            }

            void FillStatusInfo(ListingSaleStatusFieldsResponse statusInfo)
            {
                if (statusInfo is null)
                {
                    throw new ArgumentNullException(nameof(statusInfo));
                }

                residentialListingRequest.SoldPrice = statusInfo.ClosePrice.HasValue ? Math.Floor(statusInfo.ClosePrice.Value) : 0;
                residentialListingRequest.ClosedDate = statusInfo.ClosedDate;
                residentialListingRequest.ContractDate = statusInfo.ContractDate;
                residentialListingRequest.SellConcess = statusInfo.SellConcess;
                residentialListingRequest.OffMarketDate = statusInfo.OffMarketDate;
                residentialListingRequest.BackOnMarketDate = statusInfo.BackOnMarketDate;
                residentialListingRequest.SoldTerms = statusInfo.SoldTerms?.ToStringFromEnumMember();
                residentialListingRequest.HasContingencyInfo = statusInfo.HasContingencyInfo;
                residentialListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                residentialListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                residentialListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
                residentialListingRequest.ContingencyInfo = statusInfo.ContingencyInfo.ToStringFromEnumMembers();
                residentialListingRequest.ExpiredDate = statusInfo.ExpiredDate;
                residentialListingRequest.SellingAgentLicenseNum = statusInfo.TrecLicenseNumber;
                residentialListingRequest.HasBuyerAgent = statusInfo.HasBuyerAgent;
                residentialListingRequest.TitlePaidBy = statusInfo.TitlePaidBy?.ToStringFromEnumMember();
                residentialListingRequest.RepairsPaidBySeller = statusInfo.RepairsAmount;

                if (!string.IsNullOrEmpty(statusInfo.SellerBuyerCost))
                {
                    residentialListingRequest.SellerBuyerCost = decimal.Parse(statusInfo.SellerBuyerCost);
                }
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
    }
}
