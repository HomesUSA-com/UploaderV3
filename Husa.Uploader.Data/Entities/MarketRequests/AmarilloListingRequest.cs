namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Amarillo.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class AmarilloListingRequest : ResidentialListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";
        private readonly SaleListingRequestQueryResponse listingResponse;
        private readonly SaleListingRequestDetailResponse listingDetailResponse;

        public AmarilloListingRequest(SaleListingRequestQueryResponse listingResponse)
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public AmarilloListingRequest(SaleListingRequestDetailResponse listingDetailResponse)
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private AmarilloListingRequest()
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

        public string Zone { get; set; }
        public string RealtorType { get; set; }
        public bool HasDetachedGarage { get; set; }
        public bool HasCarpot { get; set; }
        public bool HasRVParking { get; set; }
        public bool HasAttachedGarage { get; set; }
        public string PropertyId { get; set; }
        public string CommonName { get; set; }
        public bool IsPropertyInPID { get; set; }
        public bool IsInsideCityLimits { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorPhone { get; set; }
        public string SupervisorLicense { get; set; }
        public bool HasForeignSeller { get; set;  }
        public bool HasLenderOwned { get; set; }
        public bool HasDistressedSale { get; set; }
        public bool HasPotentialShortSale { get; set; }
        public bool HasForeclosed { get; set; }
        public bool HasTenant { get; set; }
        public string DateLeaseExpires { get; set; }
        public string SpecialFeatures { get; set; }
        public string OtherSpecialFeatures { get; set; }
        public string ParkingFeatures { get; set; }
        public string CommunityFeatures { get; set; }
        public bool HoaRequirement { get; set; }
        public string ConstructionType { get; set; }
        public string StoriesFeatures { get; set; }
        public int NumQuarterBaths { get; set; }
        public int NumThreeQuartersBaths { get; set; }
        public bool HasPowder { get; set; }
        public bool HasHollywoodJackAndJill { get; set; }
        public string LaundryFeatures { get; set; }
        public string DetachedStructures { get; set; }
        public string WaterFeature { get; set; }
        public int WaterHeater { get; set; }

        public override MarketCode MarketCode => MarketCode.Amarillo;

        public override BuiltStatus BuiltStatus => this.YearBuiltDesc switch
        {
            "PROPOS" => BuiltStatus.ToBeBuilt,
            "NCC" => BuiltStatus.ReadyNow,
            _ => BuiltStatus.WithCompletion,
        };

        public bool HasHoa { get; set; }
        public string PatioAndPorchFeatures { get; set; }

        public override ResidentialListingRequest CreateFromApiResponse() => new AmarilloListingRequest()
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
            ListStatus = this.listingResponse.MlsStatus.GetEnumDescription(),
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
            EnableOpenHouse = this.listingResponse.EnableOpenHouse,
            UpdateGeocodes = this.listingResponse.UpdateGeocodes,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail(CompanyServicesManager.Api.Contracts.Response.CompanyDetail company)
        {
            var residentialListingRequest = new AmarilloListingRequest
            {
                ResidentialListingRequestID = this.listingDetailResponse.Id,
                ResidentialListingRequestGUID = this.listingDetailResponse.Id,
                ResidentialListingID = this.listingDetailResponse.ListingSaleId,
                MarketName = this.MarketCode.GetEnumDescription(),
                ListPrice = (int)this.listingDetailResponse.ListPrice,
                MLSNum = this.listingDetailResponse.MlsNumber,
                MlsStatus = this.listingDetailResponse.MlsStatus.GetEnumDescription(),
                ListStatus = this.listingDetailResponse.MlsStatus.GetEnumDescription(),
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                ExpectedActiveDate = DateTime.Now.ToString("MM/dd/yy"),
                ExpiredDate = this.listingDetailResponse.ExpirationDate,
                SellerBuyerCost = this.SellerBuyerCost,
                RemarksFormatFromCompany = company?.MlsInfo?.RemarksForCompletedHomes,
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
                ArgumentNullException.ThrowIfNull(nameof(salePropertyInfo));

                residentialListingRequest.BuilderName = salePropertyInfo.OwnerName;
                residentialListingRequest.CompanyName = salePropertyInfo.OwnerName;
                residentialListingRequest.OwnerName = salePropertyInfo.OwnerName;
                residentialListingRequest.PlanProfileName = salePropertyInfo.PlanName;
                residentialListingRequest.CompanyId = salePropertyInfo.CompanyId;
            }

            void FillAddressInfo(AddressInfoResponse addressInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(addressInfo));

                residentialListingRequest.StreetNum = addressInfo.StreetNumber;
                residentialListingRequest.StreetName = addressInfo.StreetName;
                residentialListingRequest.StreetDir = addressInfo.StreetDirection?.GetEnumDescription();
                residentialListingRequest.City = addressInfo.City.GetEnumDescription();
                residentialListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                residentialListingRequest.State = addressInfo.State.ToStringFromEnumMember();
                residentialListingRequest.Zip = addressInfo.ZipCode;
                residentialListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                residentialListingRequest.StreetType = addressInfo.StreetType?.ToStringFromEnumMember();
                residentialListingRequest.Subdivision = addressInfo.Subdivision;
                residentialListingRequest.StateCode = addressInfo.State.ToStringFromEnumMember();
            }

            void FillPropertyInfo(PropertyInfoResponse propertyInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(propertyInfo));

                residentialListingRequest.BuildCompletionDate = propertyInfo.ConstructionCompletionDate;
                residentialListingRequest.YearBuiltDesc = propertyInfo.ConstructionStage?.ToStringFromEnumMember();
                residentialListingRequest.YearBuilt = propertyInfo.ConstructionStartYear;
                residentialListingRequest.Legal = propertyInfo.LegalDescription;
                residentialListingRequest.TaxID = propertyInfo.TaxId;
                residentialListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                residentialListingRequest.LotSize = propertyInfo.LotDimension;
                residentialListingRequest.LotDim = propertyInfo.LotSize?.ToString();
                residentialListingRequest.LotSizeAcres = propertyInfo.Acres?.ToString();
                residentialListingRequest.LotDesc = propertyInfo.LotDescription.ToStringFromEnumMembers();
                residentialListingRequest.Zone = propertyInfo.Zone?.ToStringFromEnumMember();
                residentialListingRequest.MLSArea = propertyInfo.MlsArea?.GetEnumDescription();
                residentialListingRequest.KeyboxNumber = propertyInfo.LockBoxNumber?.ToString();
                residentialListingRequest.PropertyId = propertyInfo.PropertyId.ToString();
                residentialListingRequest.CommonName = propertyInfo.CommonName;
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(spacesDimensionsInfo));

                residentialListingRequest.NumLivingAreas = spacesDimensionsInfo.NumLivingAreas;
                residentialListingRequest.BathsFull = spacesDimensionsInfo.NumFullBaths;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.NumHalfBaths;
                residentialListingRequest.Beds = spacesDimensionsInfo.NumBedrooms;
                residentialListingRequest.BathsTotal = spacesDimensionsInfo.NumBathrooms;
                residentialListingRequest.SqFtSource = spacesDimensionsInfo.SqFtSource?.ToStringFromEnumMember();
                residentialListingRequest.RealtorType = spacesDimensionsInfo.RealtorType?.ToStringFromEnumMember();
                residentialListingRequest.StoriesFeatures = spacesDimensionsInfo.StoriesFeatures.ToStringFromEnumMembers();
                residentialListingRequest.NumQuarterBaths = spacesDimensionsInfo.NumQuarterBaths ?? 0;
                residentialListingRequest.NumThreeQuartersBaths = spacesDimensionsInfo.NumThreeQuartersBaths ?? 0;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(featuresInfo));

                residentialListingRequest.ExteriorDesc = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.FacesDesc = featuresInfo.HomeFaces?.ToStringFromEnumMember();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.NumberFireplaces = featuresInfo.NumFireplaces?.ToString();
                residentialListingRequest.GarageDesc = featuresInfo.GarageRemarks;
                residentialListingRequest.AppliancesDesc = featuresInfo.Appliances.ToStringFromEnumMembers();
                residentialListingRequest.FenceDesc = featuresInfo.Fence.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                residentialListingRequest.CommonFeatures = featuresInfo.CommunityFeatures.ToStringFromEnumMembers();
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.LaundryLocDesc = featuresInfo.LaundryLocation.ToStringFromEnumMembers();
                residentialListingRequest.InteriorDesc = featuresInfo.InteriorFeatures?.ToString();
                residentialListingRequest.PublicRemarks = featuresInfo.PropertyDescription;
                residentialListingRequest.HousingStyleDesc = featuresInfo.HousingStyle.HasValue ? featuresInfo.HousingStyle.Value.ToStringFromEnumMember() : string.Empty;
                residentialListingRequest.GarageCapacity = featuresInfo.GarageSpaces;
                residentialListingRequest.HasAttachedGarage = featuresInfo.HasAttachedGarage;
                residentialListingRequest.HasDetachedGarage = featuresInfo.HasDetachedGarage;
                residentialListingRequest.HasCarpot = featuresInfo.HasCarport;
                residentialListingRequest.HasRVParking = featuresInfo.HasRvParking;
                residentialListingRequest.IsPropertyInPID = featuresInfo.IsPropertyInPID;
                residentialListingRequest.IsInsideCityLimits = featuresInfo.IsInsideCityLimits;
                residentialListingRequest.HasForeignSeller = featuresInfo.HasForeignSeller;
                residentialListingRequest.HasLenderOwned = featuresInfo.HasLenderOwned;
                residentialListingRequest.HasDistressedSale = featuresInfo.HasDistressedSale;
                residentialListingRequest.HasPotentialShortSale = featuresInfo.HasPotentialShortSale;
                residentialListingRequest.HasForeclosed = featuresInfo.HasForeclosed;
                residentialListingRequest.HasTenant = featuresInfo.HasTenant;
                residentialListingRequest.DateLeaseExpires = featuresInfo.DateLeaseExpires.HasValue ? featuresInfo.DateLeaseExpires.Value.ToShortDateString() : string.Empty;
                residentialListingRequest.SpecialFeatures = featuresInfo.SpecialFeatures.ToStringFromEnumMembers();
                residentialListingRequest.OtherSpecialFeatures = featuresInfo.OtherSpecialFeatures;
                residentialListingRequest.ParkingFeatures = featuresInfo.ParkingFeatures.ToStringFromEnumMembers();
                residentialListingRequest.CommunityFeatures = featuresInfo.CommunityFeatures.ToStringFromEnumMembers();
                residentialListingRequest.ConstructionType = featuresInfo.ConstructionType.ToStringFromEnumMembers();
                residentialListingRequest.CommunityFeatures = featuresInfo.CommunityFeatures.ToStringFromEnumMembers();
                residentialListingRequest.InteriorDesc = featuresInfo.InteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.HasPowder = featuresInfo.HasPowder;
                residentialListingRequest.HasHollywoodJackAndJill = featuresInfo.HasHollywoodJackAndJill;
                residentialListingRequest.LaundryFeatures = featuresInfo.LaundryFeatures.ToStringFromEnumMembers();
                residentialListingRequest.DetachedStructures = featuresInfo.DetachedStructures.ToStringFromEnumMembers();
                residentialListingRequest.WaterFeature = featuresInfo.WaterFeature.ToStringFromEnumMembers();
                residentialListingRequest.PoolDesc = featuresInfo.Pool.ToStringFromEnumMembers();
                residentialListingRequest.WaterHeater = featuresInfo.WaterHeater ?? 0;
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(financialInfo));

                residentialListingRequest.FinancingProposed = financialInfo.PossibleFinancing.ToStringFromEnumMembers();
                residentialListingRequest.ExemptionsDesc = financialInfo.TaxExemptions?.ToString();
                residentialListingRequest.TaxRate = financialInfo.TaxRate?.ToString();
                residentialListingRequest.TitleCo = financialInfo.SuggestedTitleCompany;
                residentialListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount?.ToString();
                residentialListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = financialInfo.BonusExpirationDate;
                residentialListingRequest.BuyerIncentive = financialInfo.BuyersAgentCommission?.ToString();
                residentialListingRequest.BuyerIncentiveDesc = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.HoaRequirement = financialInfo.HoaRequirement;
            }

            void FillShowingInfo(ShowingResponse showingInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(showingInfo));

                residentialListingRequest.AgentListApptPhone = showingInfo.ContactPhone;
                residentialListingRequest.OtherPhone = showingInfo.OccupantPhone;
                residentialListingRequest.ShowingInstructions = showingInfo.ShowingInstructions;
                residentialListingRequest.RealtorContactEmail = showingInfo.RealtorContactEmail.ToStringFromCollection(";");
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
                residentialListingRequest.AgentPrivateRemarks2 = showingInfo.AgentPrivateRemarksAdditional;
                residentialListingRequest.EnableOpenHouse = showingInfo.EnableOpenHouses;
                residentialListingRequest.AllowPendingList = showingInfo.ShowOpenHousesPending;
                residentialListingRequest.OwnerName = showingInfo.OwnerName;
                residentialListingRequest.SupervisorName = showingInfo.SupervisorName;
                residentialListingRequest.SupervisorPhone = showingInfo.SupervisorPhone;
                residentialListingRequest.SupervisorLicense = showingInfo.SupervisorLicense;
            }

            void FillSchoolsInfo(SchoolsResponse schoolsInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(schoolsInfo));

                residentialListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName1 = schoolsInfo.ElementarySchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName2 = schoolsInfo.IntermediateSchool?.ToStringFromEnumMember();
                residentialListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
            }

            void FillStatusInfo(SaleListingStatusFieldsResponse statusInfo)
            {
                ArgumentNullException.ThrowIfNull(nameof(statusInfo));

                residentialListingRequest.SoldPrice = statusInfo.ClosePrice;
                residentialListingRequest.ClosedDate = statusInfo.ClosedDate;
                residentialListingRequest.PendingDate = statusInfo.PendingDate;
                residentialListingRequest.SellConcess = statusInfo.SellConcess;
                residentialListingRequest.OffMarketDate = statusInfo.OffMarketDate;
                residentialListingRequest.BackOnMarketDate = statusInfo.BackOnMarketDate;
                residentialListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                residentialListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                residentialListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
            }

            void FillRoomsInfo(IEnumerable<RoomResponse> rooms)
            {
                if (rooms == null || !rooms.Any())
                {
                    return;
                }

                if (this.Rooms == null)
                {
                    this.Rooms = new List<ResidentialListingRequestRoom>();
                }

                foreach (var room in rooms)
                {
                    this.Rooms.Add(new()
                    {
                        RoomType = room.RoomType.ToStringFromEnumMember(),
                        Length = room.Length,
                        Width = room.Width,
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

                if (this.OpenHouse == null)
                {
                    this.OpenHouse = new List<OpenHouseRequest>();
                }

                foreach (var openHouse in openHouses)
                {
                    this.OpenHouse.Add(new()
                    {
                        Date = OpenHouseExtensions.GetNextWeekday(DateTime.Today, Enum.Parse<DayOfWeek>(openHouse.Type.ToString(), ignoreCase: true)),
                        StartTime = (TimeSpan)openHouse.StartTime,
                        EndTime = (TimeSpan)openHouse.EndTime,
                        Active = true,
                        Type = OpenHouseType.Public,
                    });
                }

                residentialListingRequest.OpenHouse = this.OpenHouse;
            }
        }

        public override string GetAgentRemarksMessage()
        {
            var privateRemarks = string.Empty;
            const string limitedServiceMessage = "LIMITED SERVICE LISTING: Buyer verifies dimensions & ISD info. Use Bldr contract.";
            const string homeUnderConstruction = "Home is under construction. For your safety, call appt number for showings.";
            var saleOfficeInfo = this.GetSalesAssociateRemarksMessage();

            var bonusMessage = string.IsNullOrWhiteSpace(this.MLSNum) ? this.GetAgentBonusRemarksMessage() : string.Empty;

            if (!string.IsNullOrWhiteSpace(bonusMessage))
            {
                privateRemarks = $"{bonusMessage} {privateRemarks}";
            }

            privateRemarks += this.BuiltStatus != BuiltStatus.ReadyNow ? $"{homeUnderConstruction}" : string.Empty;
            privateRemarks += !string.IsNullOrWhiteSpace(saleOfficeInfo) ? $" {saleOfficeInfo}" : string.Empty;
            privateRemarks += $" {limitedServiceMessage}";
            privateRemarks += !string.IsNullOrWhiteSpace(this.PlanProfileName) ? $" Plan: {this.PlanProfileName}." : string.Empty;
            privateRemarks += !string.IsNullOrWhiteSpace(this.RealtorContactEmail) ? $" Email contact: {this.RealtorContactEmail}." : string.Empty;

            return privateRemarks;
        }
    }
}
