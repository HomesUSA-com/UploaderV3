namespace Husa.Uploader.Data.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Contracts.Response.ListingRequest;
    using Husa.Uploader.Data.Entities.LotListing;

    public class AborLotListingRequest : LotListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

        private readonly ListingRequestQueryResponse listingResponse;
        private readonly ListingRequestDetailResponse listingDetailResponse;

        public AborLotListingRequest(ListingRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public AborLotListingRequest(ListingRequestDetailResponse listingDetailResponse)
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
            Address = this.listingResponse.Address,
            ListStatus = this.listingResponse.MlsStatus.ToStringFromEnumMember(),
            SysCreatedOn = this.listingResponse.SysCreatedOn,
        };

        public override LotListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new AborLotListingRequest
            {
                LotListingRequestID = this.listingDetailResponse.Id,
                MarketName = this.MarketCode.GetEnumDescription(),
                MLSNum = this.listingDetailResponse.MlsNumber,
                ListStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
            };

            return residentialListingRequest;
        }
    }
}
