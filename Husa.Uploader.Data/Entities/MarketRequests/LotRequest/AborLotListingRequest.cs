namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.Abor.Api.Contracts.Response.LotListing;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities.LotListing;

    public class AborLotListingRequest : LotListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

        private readonly ListingLotRequestQueryResponse listingResponse;
        private readonly LotListingRequestDetailResponse listingDetailResponse;

        public AborLotListingRequest(ListingLotRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public AborLotListingRequest(LotListingRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private AborLotListingRequest()
            : base()
        {
        }

        public override MarketCode MarketCode => MarketCode.Austin;

        public override LotListingRequest CreateFromApiResponse() => new AborLotListingRequest()
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
            var lotListingRequest = new AborLotListingRequest
            {
                LotListingRequestID = this.listingDetailResponse.Id,
                MarketName = this.MarketCode.GetEnumDescription(),
                MLSNum = this.listingDetailResponse.MlsNumber,
                ListPrice = this.listingDetailResponse.ListPrice.HasValue ? (int)this.listingDetailResponse.ListPrice.Value : 0,
                ListDate = this.ListDate,
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                ExpectedActiveDate = DateTime.Now.ToString("MM/dd/yy"),
                ExpiredDate = this.listingDetailResponse.ExpirationDate,
            };

            FillMainInfo(this.listingDetailResponse);
            FillLotPropertyInfo(this.listingDetailResponse.PropertyInfo);
            FillLotAddressInfo(this.listingDetailResponse.AddressInfo);
            FillLotSchoolsInfo(this.listingDetailResponse.SchoolsInfo);
            FillLotFeaturesInfo(this.listingDetailResponse.FeaturesInfo);
            FillLotFinantialInfo(this.listingDetailResponse.FinancialInfo);
            FillLotShowingInfo(this.listingDetailResponse.ShowingInfo);

            return lotListingRequest;

            void FillMainInfo(LotListingRequestDetailResponse lotListingRequestDetailResponse)
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

            void FillLotPropertyInfo(LotPropertyResponse propertyInfo)
            {
                if (propertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                lotListingRequest.LegalDescription = propertyInfo.LegalDescription;
                lotListingRequest.TaxId = propertyInfo.TaxId;
                lotListingRequest.MlsArea = propertyInfo.MlsArea?.ToStringFromEnumMember();
                lotListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                lotListingRequest.OtherFees = propertyInfo.TaxLot;
                lotListingRequest.FemaFloodPlain = propertyInfo.FemaFloodPlain.ToStringFromEnumMembers();
                lotListingRequest.BuilderRestrictions = propertyInfo.BuilderRestrictions;
                lotListingRequest.PropertySubType = propertyInfo.PropertySubType?.ToStringFromEnumMember();
                lotListingRequest.SurfaceWater = propertyInfo.SurfaceWater;
                lotListingRequest.LotSize = propertyInfo.LotSize;
                lotListingRequest.LotDimension = propertyInfo.LotDimension;
                lotListingRequest.AlsoListedAs = propertyInfo.AlsoListedAs;
                lotListingRequest.TypeOfHomeAllowed = propertyInfo.TypeOfHomeAllowed?.ToStringFromEnumMembers();
                lotListingRequest.LiveStock = propertyInfo.LiveStock;
                lotListingRequest.NumberOfPonds = propertyInfo.NumberOfPonds;
                lotListingRequest.NumberOfWells = propertyInfo.NumberOfWells;
                lotListingRequest.CommercialAllowed = propertyInfo.CommercialAllowed;
                lotListingRequest.SoilType = propertyInfo.SoilType.ToStringFromEnumMembers();
                lotListingRequest.LotDescription = propertyInfo.LotDescription.ToStringFromEnumMembers();
                lotListingRequest.PropCondition = propertyInfo.PropCondition.ToStringFromEnumMembers();
            }

            void FillLotAddressInfo(LotAddressResponse addressInfo)
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
                lotListingRequest.Subdivision = addressInfo.Subdivision;
                lotListingRequest.UnitNumber = addressInfo.UnitNumber;
            }

            void FillLotSchoolsInfo(LotSchoolsResponse schoolsInfo)
            {
                if (schoolsInfo is null)
                {
                    throw new ArgumentNullException(nameof(schoolsInfo));
                }

                lotListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.ToStringFromEnumMember();
                lotListingRequest.SchoolName1 = schoolsInfo.ElementarySchool?.ToStringFromEnumMember();
                lotListingRequest.SchoolName2 = schoolsInfo.MiddleSchool?.ToStringFromEnumMember();
                lotListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
                lotListingRequest.SchoolName4 = schoolsInfo.OtherElementarySchool;
                lotListingRequest.SchoolName5 = schoolsInfo.OtherMiddleSchool;
                lotListingRequest.SchoolName6 = schoolsInfo.OtherHighSchool;
            }

            void FillLotFeaturesInfo(LotFeaturesResponse featureInfo)
            {
                if (featureInfo is null)
                {
                    throw new ArgumentNullException(nameof(featureInfo));
                }

                lotListingRequest.DistanceToWaterAccess = featureInfo.DistanceToWaterAccess?.ToStringFromEnumMember();
                lotListingRequest.WaterBodyName = featureInfo.WaterBodyName?.ToStringFromEnumMember();
                lotListingRequest.View = featureInfo.View?.ToStringFromEnumMembers();
                lotListingRequest.HorseAmenities = featureInfo.HorseAmenities?.ToStringFromEnumMembers();
                lotListingRequest.WaterfrontFeatures = featureInfo.WaterfrontFeatures?.ToStringFromEnumMembers();
                lotListingRequest.MineralsFeatures = featureInfo.MineralsFeatures?.ToStringFromEnumMembers();
                lotListingRequest.Fencing = featureInfo.Fencing?.ToStringFromEnumMembers();
                lotListingRequest.RestrictionsDescription = featureInfo.RestrictionsDescription?.ToStringFromEnumMembers();
                lotListingRequest.RoadSurface = featureInfo.RoadSurface?.ToStringFromEnumMembers();
                lotListingRequest.OtherStructures = featureInfo.OtherStructures?.ToStringFromEnumMembers();
                lotListingRequest.ExteriorFeatures = featureInfo.ExteriorFeatures?.ToStringFromEnumMembers();
                lotListingRequest.NeighborhoodAmenities = featureInfo.NeighborhoodAmenities?.ToStringFromEnumMembers();
                lotListingRequest.Disclosures = featureInfo.Disclosures?.ToStringFromEnumMembers();
                lotListingRequest.DocumentsAvailable = featureInfo.DocumentsAvailable?.ToStringFromEnumMembers();
                lotListingRequest.UtilitiesDescription = featureInfo.UtilitiesDescription?.ToStringFromEnumMembers();
                lotListingRequest.GroundWaterConservDistric = featureInfo.GroundWaterConservDistric;
                lotListingRequest.WaterSource = featureInfo.WaterSource?.ToStringFromEnumMembers();
                lotListingRequest.WaterSewer = featureInfo.WaterSewer?.ToStringFromEnumMembers();
            }

            void FillLotFinantialInfo(LotFinancialResponse financialRequest)
            {
                if (financialRequest is null)
                {
                    throw new ArgumentNullException(nameof(financialRequest));
                }

                lotListingRequest.HasHoa = financialRequest.HasHoa;
                lotListingRequest.HoaName = financialRequest.HoaName;
                lotListingRequest.HoaFee = financialRequest.HoaFee.DecimalToString();
                lotListingRequest.HOARequirement = financialRequest.HOARequirement?.ToStringFromEnumMember();
                lotListingRequest.BillingFrequency = financialRequest.BillingFrequency?.ToStringFromEnumMember();
                lotListingRequest.HoaIncludes = financialRequest.HoaIncludes?.ToStringFromEnumMembers();
                lotListingRequest.AcceptableFinancing = financialRequest.AcceptableFinancing?.ToStringFromEnumMembers();
                lotListingRequest.EstimatedTax = financialRequest.EstimatedTax.DecimalToString();
                lotListingRequest.TaxYear = financialRequest.TaxYear.IntegerToString();
                lotListingRequest.TaxAssesedValue = financialRequest.TaxAssesedValue.IntegerToString();
                lotListingRequest.TaxRate = financialRequest.TaxRate.DecimalToString();
                lotListingRequest.TaxExemptions = financialRequest.TaxExemptions?.ToStringFromEnumMembers();
                lotListingRequest.LandTitleEvidence = financialRequest.LandTitleEvidence?.ToStringFromEnumMember();
                lotListingRequest.PreferredTitleCompany = financialRequest.PreferredTitleCompany;
                lotListingRequest.HasBuyerIncentive = financialRequest.HasBuyerIncentive;
                lotListingRequest.BuyersAgentCommission = financialRequest.BuyersAgentCommission;
                lotListingRequest.BuyersAgentCommissionType = financialRequest.BuyersAgentCommissionType.ToStringFromEnumMember();
                lotListingRequest.HasAgentBonus = financialRequest.HasAgentBonus;
                lotListingRequest.HasBonusWithAmount = financialRequest.HasBonusWithAmount;
                lotListingRequest.AgentBonusAmount = financialRequest.AgentBonusAmount.DecimalToString();
                lotListingRequest.AgentBonusAmountType = financialRequest.AgentBonusAmountType.ToStringFromEnumMember();
                lotListingRequest.BonusExpirationDate = financialRequest.BonusExpirationDate;
            }

            void FillLotShowingInfo(LotShowingResponse showingInfo)
            {
                if (showingInfo is null)
                {
                    throw new ArgumentNullException(nameof(showingInfo));
                }

                lotListingRequest.AgentListApptPhone = showingInfo.ApptPhone;
                lotListingRequest.OwnerName = showingInfo.OwnerName;
                lotListingRequest.ShowingRequirements = showingInfo.ShowingRequirements?.ToStringFromEnumMembers();
                lotListingRequest.ApptPhone = showingInfo.ApptPhone;
                lotListingRequest.ShowingServicePhone = showingInfo.ShowingServicePhone;
                lotListingRequest.ShowingInstructions = showingInfo.ShowingInstructions;
                lotListingRequest.PublicRemarks = showingInfo.PublicRemarks;
                lotListingRequest.Directions = showingInfo.Directions;
                lotListingRequest.ShowingContactType = showingInfo.ShowingContactType?.ToStringFromEnumMembers();
                lotListingRequest.ShowingContactName = showingInfo.ShowingContactName;
            }
        }
    }
}
