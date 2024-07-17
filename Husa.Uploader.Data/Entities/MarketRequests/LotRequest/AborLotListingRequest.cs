namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.Abor.Api.Contracts.Response.LotListing;
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

                lotListingRequest.Legal = propertyInfo.LegalDescription;
                lotListingRequest.TaxID = propertyInfo.TaxId;
                lotListingRequest.MLSArea = propertyInfo.MlsArea?.ToStringFromEnumMember();
                lotListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                lotListingRequest.OtherFees = propertyInfo.TaxLot;
                lotListingRequest.FemaFloodPlain = propertyInfo.FemaFloodPlain.ToStringFromEnumMembers();
                lotListingRequest.BuilderRestrictions = propertyInfo.BuilderRestrictions;
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
                lotListingRequest.UnitNum = addressInfo.UnitNumber;
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
        }
    }
}
