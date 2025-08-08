namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Har.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.Har.Api.Contracts.Response.LotListing;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities.LotListing;

    public class HarLotListingRequest : LotListingRequest
    {
        private readonly ListingLotRequestQueryResponse listingResponse;
        private readonly LotListingRequestDetailResponse listingDetailResponse;

        public HarLotListingRequest(ListingLotRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public HarLotListingRequest(LotListingRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private HarLotListingRequest()
            : base()
        {
        }

        public string HoaPhone { get; set; }
        public string SeniorActiveCommunity { get; set; }
        public string MaintenanceFeeIncludes { get; set; }
        public string HasOtherFees { get; set; }
        public string OtherFeesInclude { get; set; }

        public override MarketCode MarketCode => MarketCode.Houston;

        public override LotListingRequest CreateFromApiResponse() => new HarLotListingRequest()
        {
            LotListingRequestID = this.listingResponse.Id,
            CompanyName = this.listingResponse.OwnerName,
            MLSNum = this.listingResponse.MlsNumber,
            MarketName = this.listingResponse.Market,
            CityCode = this.listingResponse.City.ToStringFromEnumMember(),
            Subdivision = this.listingResponse.Subdivision,
            Zip = this.listingResponse.ZipCode,
            Address = this.listingResponse.Address,
            ListPrice = this.listingResponse.ListPrice.HasValue ? (int)this.listingResponse.ListPrice.Value : 0,
            ListStatus = this.listingResponse.MlsStatus.ToStringFromEnumMember(),
            ListDate = this.ListDate,
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
            UpdateGeocodes = this.listingResponse.UpdateGeocodes,
        };

        public override LotListingRequest CreateFromApiResponseDetail()
        {
            var lotListingRequest = new HarLotListingRequest
            {
                LotListingRequestID = this.listingDetailResponse.Id,
                MarketName = this.MarketCode.GetEnumDescription(),
                MLSNum = this.listingDetailResponse.MlsNumber,
                ListStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                ListPrice = this.listingDetailResponse.ListPrice.HasValue ? (int)this.listingDetailResponse.ListPrice.Value : 0,
                ListDate = this.ListDate,
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                ExpectedActiveDate = DateTime.Now.ToString("MM/dd/yy"),
                ExpiredDate = this.listingDetailResponse.ExpirationDate,
            };

            FillLotPropertyInfo(this.listingDetailResponse);
            FillAddressInfo(this.listingDetailResponse.AddressInfo);
            FillPropertyInfo(this.listingDetailResponse.PropertyInfo);
            FillFeaturesInfo(this.listingDetailResponse.FeaturesInfo);
            FillFinancialInfo(this.listingDetailResponse.FinancialInfo);
            FillShowingInformation(this.listingDetailResponse.ShowingInfo);
            FillSchoolsInfo(this.listingDetailResponse.SchoolsInfo);

            return lotListingRequest;

            void FillLotPropertyInfo(LotListingRequestDetailResponse lotListingRequestDetailResponse)
            {
                if (lotListingRequestDetailResponse is null)
                {
                    throw new ArgumentNullException(nameof(lotListingRequestDetailResponse));
                }

                lotListingRequest.BuilderName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.CompanyName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.OwnerName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.CompanyId = lotListingRequestDetailResponse.CompanyId;
            }

            void FillAddressInfo(LotAddressResponse addressInfo)
            {
                if (addressInfo is null)
                {
                    throw new ArgumentNullException(nameof(addressInfo));
                }

                lotListingRequest.StreetNum = addressInfo.StreetNumber;
                lotListingRequest.StreetName = addressInfo.StreetName;
                lotListingRequest.City = addressInfo.City.GetEnumDescription();
                lotListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                lotListingRequest.State = addressInfo.State.ToStringFromEnumMember();
                lotListingRequest.Zip = addressInfo.ZipCode;
                lotListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                lotListingRequest.StreetType = addressInfo.StreetType?.ToStringFromEnumMember();
                lotListingRequest.LotNumber = addressInfo.LotNumber;
                lotListingRequest.Subdivision = addressInfo.Subdivision;
                lotListingRequest.StDirection = addressInfo.StreetDirPrefix?.ToStringFromEnumMember();
            }

            void FillPropertyInfo(LotPropertyResponse propertyInfo)
            {
                if (propertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                lotListingRequest.LegalDescription = propertyInfo.LegalDescription;
                lotListingRequest.TaxId = propertyInfo.TaxId;
                lotListingRequest.Acreage = propertyInfo.Acreage?.ToStringFromEnumMember();
                lotListingRequest.HasMasterPlannedCommunity = propertyInfo.IsPlannedCommunity.BoolToNumericBool();
                lotListingRequest.MasterPlannedCommunity = propertyInfo.PlannedCommunity?.GetEnumDescription();
                lotListingRequest.LegalSubdivision = propertyInfo.LegalSubdivision?.GetEnumDescription();
                lotListingRequest.LotListType = propertyInfo.LotListType?.ToStringFromEnumMember();
                lotListingRequest.LotSize = propertyInfo.LotSize;
                lotListingRequest.LotSizeSrc = propertyInfo.LotSizeSource?.ToStringFromEnumMember();
                lotListingRequest.Acres = propertyInfo.Acres.ToString();
                lotListingRequest.FrontDimensions = propertyInfo.FrontDimensions;
                lotListingRequest.BackDimensions = propertyInfo.BackDimensions;
                lotListingRequest.LeftDimensions = propertyInfo.LeftDimensions;
                lotListingRequest.RightDimensions = propertyInfo.RightDimensions;
                lotListingRequest.LotDescription = propertyInfo.LotDescription.ToStringFromEnumMembers();
            }

            void FillFeaturesInfo(LotFeaturesResponse featureInfo)
            {
                if (featureInfo is null)
                {
                    throw new ArgumentNullException(nameof(featureInfo));
                }

                lotListingRequest.HasDevelopedCommunity = featureInfo.DevelopedCommunity;
                lotListingRequest.HasTennis = featureInfo.Tennis;
                lotListingRequest.HasPool = featureInfo.PoolArea;
                lotListingRequest.HasUtilityDistrict = featureInfo.UtilityDistrict;
                lotListingRequest.ElectricServices = featureInfo.Electric?.ToStringFromEnumMember();
                lotListingRequest.GasServices = featureInfo.Gas?.ToStringFromEnumMember();
                lotListingRequest.CableServices = featureInfo.Cable?.ToStringFromEnumMember();
                lotListingRequest.PhoneServices = featureInfo.Phone?.ToStringFromEnumMember();
                lotListingRequest.GolfDescription = featureInfo.GolfCourseName?.GetEnumDescription();
                lotListingRequest.HasSubdivisionLake = featureInfo.SubdivisionLakeAccess;
                lotListingRequest.LotUse = featureInfo.LotUse.ToStringFromEnumMembers();
                lotListingRequest.LotImprovements = featureInfo.LotImprovements.ToStringFromEnumMembers();
                lotListingRequest.WaterfrontFeatures = featureInfo.WaterfrontFeatures.ToStringFromEnumMembers();
                lotListingRequest.RoadSurface = featureInfo.RoadSurface.ToStringFromEnumMembers();
                lotListingRequest.Access = featureInfo.Access.ToStringFromEnumMembers();
                lotListingRequest.WaterSewer = featureInfo.WaterSewerDescription.ToStringFromEnumMembers();
            }

            void FillFinancialInfo(LotFinancialResponse financialInfo)
            {
                ArgumentNullException.ThrowIfNull(financialInfo);

                lotListingRequest.Restrictions = financialInfo.Restrictions.ToStringFromEnumMembers();
                lotListingRequest.HasHoa = financialInfo.HasHoa;
                lotListingRequest.HoaIncludes = financialInfo.HoaAmenities.ToStringFromEnumMembers();
                lotListingRequest.HoaName = financialInfo.HoaName;
                lotListingRequest.HoaPhone = financialInfo.HoaPhone.PhoneFormat(true);
                lotListingRequest.Disclosures = financialInfo.Disclosures.ToStringFromEnumMembers();
                lotListingRequest.SeniorActiveCommunity = financialInfo.SeniorActiveCommunity.BoolToNumericBool();
                lotListingRequest.AcceptableFinancing = financialInfo.FinanacingConsidered.ToStringFromEnumMembers();
                lotListingRequest.HOARequirement = financialInfo.HoaRequirement?.ConvertToBoolean().BoolToNumericBool();
                lotListingRequest.HoaFee = financialInfo.HoaFee.DecimalToString();
                lotListingRequest.BillingFrequency = financialInfo.BillingFrequency?.ToStringFromEnumMember();
                lotListingRequest.MaintenanceFeeIncludes = financialInfo.MaintenanceFeeIncludes.ToStringFromEnumMembers();
                lotListingRequest.HasOtherFees = financialInfo.HasOtherFees.BoolToNumericBool();
                lotListingRequest.OtherFees = financialInfo.OtherFeeAmount.DecimalToString();
                lotListingRequest.OtherFeesInclude = financialInfo.OtherFeesInclude;
                lotListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                lotListingRequest.TaxRate = financialInfo.TaxRate.DecimalToString();
                lotListingRequest.TaxExemptions = financialInfo.Exemption;
                lotListingRequest.BuyersAgentCommission = financialInfo.BuyersAgentCommission;
                lotListingRequest.BuyersAgentCommissionType = financialInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                lotListingRequest.HasAgentBonus = financialInfo.HasAgentBonus;
                lotListingRequest.HasBonusWithAmount = financialInfo.HasBonusWithAmount;
                lotListingRequest.AgentBonusAmount = financialInfo.AgentBonusAmount.DecimalToString();
                lotListingRequest.AgentBonusAmountType = financialInfo.AgentBonusAmountType.ToStringFromEnumMember();
                lotListingRequest.BonusExpirationDate = financialInfo.BonusExpirationDate;
            }

            void FillShowingInformation(LotShowingResponse showingInfo)
            {
                ArgumentNullException.ThrowIfNull(showingInfo);

                lotListingRequest.PublicRemarks = showingInfo.PhysicalPropertyDescription;
                lotListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
                lotListingRequest.AgentPrivateRemarksAdditional = showingInfo.AgentPrivateRemarksAdditional;
                lotListingRequest.AgentListApptPhone = showingInfo.ContactPhone;
            }

            void FillSchoolsInfo(LotSchoolsResponse schoolsInfo)
            {
                ArgumentNullException.ThrowIfNull(schoolsInfo);

                lotListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.ToStringFromEnumMember();
                lotListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
            }
        }
    }
}
