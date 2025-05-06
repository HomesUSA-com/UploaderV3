namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using System.Reflection;
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
                lotListingRequest.GolfDescription = featureInfo.GolfCourseName?.ToStringFromEnumMember();
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
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                lotListingRequest.Restrictions = financialInfo.Restrictions.ToStringFromEnumMembers();
            }
        }
    }
}
