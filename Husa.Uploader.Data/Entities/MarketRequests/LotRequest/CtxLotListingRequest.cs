namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.LotListing;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Converters;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities.LotListing;

    public class CtxLotListingRequest : LotListingRequest
    {
        private const string RemarksMessage = "No Offers of Compensation Permitted.";
        private const string DefaultIntegerAsStringValue = "0";
        private readonly ListingLotRequestQueryResponse listingResponse;
        private readonly LotListingRequestDetailResponse listingDetailResponse;

        public CtxLotListingRequest(ListingLotRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public CtxLotListingRequest(LotListingRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private CtxLotListingRequest()
            : base()
        {
            this.ProspectsExempt = DefaultIntegerAsStringValue;
            this.EarnestMoney = DefaultIntegerAsStringValue;
        }

        public override MarketCode MarketCode => MarketCode.CTX;

        public override LotListingRequest CreateFromApiResponse() => new CtxLotListingRequest
        {
            LotListingRequestID = this.listingResponse.Id,
            CompanyName = this.listingResponse.OwnerName,
            MLSNum = this.listingResponse.MlsNumber,
            MarketName = this.listingResponse.Market,
            CityCode = this.listingResponse.City.ToStringFromEnumMember(),
            Subdivision = this.listingResponse.Subdivision,
            Zip = this.listingResponse.ZipCode,
            Address = this.listingResponse.Address,
            ListPrice = (int)this.listingResponse.ListPrice,
            ListStatus = this.listingResponse.MlsStatus.ToStringFromEnumMember(),
            ListDate = this.ListDate,
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
            UpdateGeocodes = this.listingResponse.UpdateGeocodes,
        };

        public override LotListingRequest CreateFromApiResponseDetail()
        {
            var lotListingRequest = new CtxLotListingRequest
            {
                LotListingRequestID = this.listingDetailResponse.Id,
                MLSNum = this.listingDetailResponse.MlsNumber,
                MarketName = this.MarketCode.GetEnumDescription(),
                ListPrice = (int)this.listingDetailResponse.ListPrice,
                ListStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                ListDate = this.ListDate,
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
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
            FillStatusInfo(this.listingDetailResponse.StatusFieldsInfo);

            return lotListingRequest;

            void FillLotPropertyInfo(LotListingRequestDetailResponse lotListingRequestDetailResponse)
            {
                ArgumentNullException.ThrowIfNull(lotListingRequestDetailResponse);

                lotListingRequest.BuilderName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.CompanyName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.OwnerName = lotListingRequestDetailResponse.OwnerName;
                lotListingRequest.CompanyId = lotListingRequestDetailResponse.CompanyId;
            }

            void FillAddressInfo(AddressInfoResponse addressInfo)
            {
                ArgumentNullException.ThrowIfNull(addressInfo);
                _ = int.TryParse(addressInfo.LotNum, out var lotNumber);

                lotListingRequest.StreetNum = addressInfo.StreetNumber;
                lotListingRequest.StreetDir = addressInfo.StreetDirection?.ToStringFromEnumMember();
                lotListingRequest.StreetName = addressInfo.StreetName;
                lotListingRequest.City = addressInfo.City.GetEnumDescription();
                lotListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                lotListingRequest.State = addressInfo.State.ToStringFromEnumMember();
                lotListingRequest.Zip = addressInfo.ZipCode;
                lotListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                lotListingRequest.UnitNumber = addressInfo.UnitNumber;
                lotListingRequest.Block = addressInfo.Block;
                lotListingRequest.StreetType = addressInfo.StreetType?.ToStringFromEnumMember();
                lotListingRequest.LotNumber = lotNumber;
                lotListingRequest.Subdivision = addressInfo.Subdivision;
                lotListingRequest.StDirection = addressInfo.StreetDirection?.ToStringFromEnumMember();
            }

            void FillPropertyInfo(LotPropertyResponse propertyInfo)
            {
                ArgumentNullException.ThrowIfNull(propertyInfo);

                lotListingRequest.LegalDescription = propertyInfo.LegalDescription;
                lotListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                lotListingRequest.TaxId = propertyInfo.TaxId;
                lotListingRequest.Latitude = propertyInfo.Latitude;
                lotListingRequest.Longitude = propertyInfo.Longitude;
                lotListingRequest.LotListType = propertyInfo.ListingType?.ToStringFromEnumMember();
                lotListingRequest.PropertySubType = propertyInfo.TypeCategory?.ToStringFromEnumMember();
                lotListingRequest.FemaFloodPlain = propertyInfo.FemaFloodPlain?.ToStringFromEnumMember();
                lotListingRequest.Zoning = propertyInfo.Zoning;
                lotListingRequest.Flooded = propertyInfo.Flooded?.ToStringFromEnumMember();
                lotListingRequest.PropertyId = propertyInfo.PropertyId;
                lotListingRequest.Category = propertyInfo.TypeCategory?.ToStringFromEnumMember();
                lotListingRequest.DocumentsOnFile = propertyInfo.DocumentsOnFile.ToStringFromEnumMembers();
                lotListingRequest.ApxTotalSqft = propertyInfo.ApxTotalSqft;
            }

            void FillFeaturesInfo(LotFeaturesResponse featuresInfo)
            {
                ArgumentNullException.ThrowIfNull(featuresInfo);

                lotListingRequest.LotDimension = featuresInfo.LotDimension;
                lotListingRequest.LotSize = featuresInfo.LotSize;
                lotListingRequest.ExteriorFeatures = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                lotListingRequest.Fencing = featuresInfo.Fencing.ToStringFromEnumMembers();
                lotListingRequest.Waterfront = featuresInfo.WaterFront.BoolToNumericBool();
                lotListingRequest.WaterfrontFeatures = featuresInfo.WaterFeatures.ToStringFromEnumMembers();
                lotListingRequest.MineralsFeatures = featuresInfo.MineralRights.ToStringFromEnumMembers();
                lotListingRequest.RestrictionsDescription = featuresInfo.RestrictionsType.ToStringFromEnumMembers();
                lotListingRequest.NeighborhoodAmenities = featuresInfo.NeighborhoodAmenities.ToStringFromEnumMembers();
                lotListingRequest.WaterSewer = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                lotListingRequest.IsGatedCommunity = featuresInfo.GatedCommunity.BoolToNumericBool();
                lotListingRequest.HasWaterAccess = featuresInfo.WaterAccess.BoolToNumericBool();
                lotListingRequest.WaterAccessDesc = featuresInfo.WaterAccessType.ToStringFromEnumMembers();
                lotListingRequest.TopoLandDescription = featuresInfo.TopoLandDescription.ToStringFromEnumMembers();
                lotListingRequest.RoadFrontageDesc = featuresInfo.AccessRoadSurface.ToStringFromEnumMembers();
                lotListingRequest.UpgradedEnergyFeatures = featuresInfo.UpgradedEnergyFeatures.BoolToNumericBool();
                lotListingRequest.EES = featuresInfo.EESFeatures.BoolToNumericBool();
                lotListingRequest.GreenIndoorAirQuality = featuresInfo.AirQuality.ToStringFromEnumMembers();
                lotListingRequest.GreenCerts = featuresInfo.GreenBuildingVerification.ToStringFromEnumMembers();
                lotListingRequest.EESFeatures = featuresInfo.EnergyFeatures.ToStringFromEnumMembers();
                lotListingRequest.EnergyDesc = featuresInfo.GreenVerificationSource.ToStringFromEnumMembers();
                lotListingRequest.GreenWaterConservation = featuresInfo.WaterConservation.ToStringFromEnumMembers();
                lotListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                lotListingRequest.SupOther = featuresInfo.SupplierOther.ToStringFromEnumMembers();
            }

            void FillFinancialInfo(LotFinancialResponse financialInfo)
            {
                ArgumentNullException.ThrowIfNull(financialInfo);

                lotListingRequest.ProposedTerms = financialInfo.ProposedTerms.ToStringFromEnumMembers();
                lotListingRequest.Exemptions = financialInfo.Exemptions.ToStringFromEnumMembers();
                lotListingRequest.TaxRate = financialInfo.TaxRate.DecimalToString();
                lotListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                lotListingRequest.TitleCo = financialInfo.TitleCompany;
                lotListingRequest.HOA = financialInfo.HoaRequirement?.ToStringFromHOARequirementCTX();
                lotListingRequest.AssocName = financialInfo.HoaName;
                lotListingRequest.AssocTransferFee = (int?)financialInfo.HoaTransferFeeAmount;
                lotListingRequest.HoaWebsite = financialInfo.HoaWebsite;
                lotListingRequest.AssocPhone = financialInfo.HoaPhone.PhoneFormat(true);
                lotListingRequest.ManagementCompany = financialInfo.HoaMgmtCo;
                lotListingRequest.AssocFeeFrequency = financialInfo.HoaTerm?.ToStringFromEnumMember();
                lotListingRequest.AssocFeeIncludes = financialInfo.HoaFeeIncludes.ToStringFromEnumMembers();
                lotListingRequest.AssocFee = (int?)financialInfo.HoaAmount;
            }

            void FillShowingInformation(LotShowingResponse showingInfo)
            {
                ArgumentNullException.ThrowIfNull(showingInfo);

                lotListingRequest.AgentListApptPhone = showingInfo.ShowingPhone;
                lotListingRequest.OtherPhone = showingInfo.SecondShowingPhone;
                lotListingRequest.LockboxTypeDesc = showingInfo.LockboxType?.ToStringFromEnumMember();
                lotListingRequest.LockboxLocDesc = showingInfo.LockboxLocation.ToStringFromEnumMembers();
                lotListingRequest.Showing = showingInfo.Showing.ToStringFromEnumMembers();
                lotListingRequest.BuyersAgentCommission = showingInfo.BuyersAgentCommission;
                lotListingRequest.ShowingInstructions = showingInfo.Showing.ToStringFromEnumMembers();
                lotListingRequest.Directions = showingInfo.Directions;
                lotListingRequest.PublicRemarks = showingInfo.PublicRemarks;
                lotListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
            }

            void FillSchoolsInfo(SchoolsResponse schoolsInfo)
            {
                ArgumentNullException.ThrowIfNull(schoolsInfo);

                lotListingRequest.SchoolDistrict = schoolsInfo.SchoolDistrict?.GetEnumDescription();
                lotListingRequest.HighSchool = schoolsInfo.HighSchool?.ToStringFromEnumMember();
                lotListingRequest.SchoolName1 = schoolsInfo.ElementarySchool?.ToStringFromEnumMember();
                lotListingRequest.SchoolName2 = schoolsInfo.MiddleSchool?.ToStringFromEnumMember();
            }

            void FillStatusInfo(ListingSaleStatusFieldsResponse statusInfo)
            {
                ArgumentNullException.ThrowIfNull(statusInfo);

                lotListingRequest.SoldPrice = statusInfo.ClosePrice;
                lotListingRequest.ClosedDate = statusInfo.ClosedDate;
                lotListingRequest.Financing = statusInfo.SellerConcessionDescription?.ToStringFromEnumMember();
                lotListingRequest.SellConcess = statusInfo.SellConcess;
                lotListingRequest.ContractDate = statusInfo.ContractDate;
                lotListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                lotListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
                lotListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                lotListingRequest.IsWithdrawalListingAgreement = statusInfo.WithdrawalActiveListingAgreement.BoolToNumericBool();
                lotListingRequest.WithdrawalReason = statusInfo.WithdrawalReason;
                lotListingRequest.WithdrawnDate = statusInfo.WithdrawalDate;
                lotListingRequest.OffMarketDate = statusInfo.OffMarketDate;
            }
        }

        public virtual string GetPublicRemarks() => RemarksMessage;

        public virtual string GetAgentRemarksMessage() => RemarksMessage;
    }
}
