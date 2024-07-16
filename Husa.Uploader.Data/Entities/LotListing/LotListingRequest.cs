namespace Husa.Uploader.Data.Entities.LotListing
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;

    public abstract class LotListingRequest
    {
        // Never remove this property
        public string WorkingBy { get; set; }
        public bool IsNewListing => string.IsNullOrWhiteSpace(this.MLSNum);
        public string CompanyName { get; set; }
        public Guid CompanyId { get; set; }
        public Guid LotListingRequestID { get; set; }
        public DateTime? SysCreatedOn { get; set; }
        public string Address { get; set; }
        public string MLSNum { get; set; }
        public string ListStatus { get; set; }
        public string MarketName { get; set; }

        public abstract MarketCode MarketCode { get; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string UnitNum { get; set; }
        public int InternalLotRequestID { get; set; }
        public Guid InternalLotRequestGUID { get; set; }
        public bool UpdateGeocodes { get; set; }

        public abstract LotListingRequest CreateFromApiResponse();

        public abstract LotListingRequest CreateFromApiResponseDetail();

        public UploadListingItem AsUploadItem(
            string builderName,
            string brokerOffice,
            string isLeasing,
            string isLot,
            Entity currentEntity,
            string worker,
            string workingStatus,
            string workingSourceAction) => new()
            {
                RequestId = this.LotListingRequestID,
                MlsNumber = this.IsNewListing ? $"New {currentEntity}" : this.MLSNum,
                Address = this.Address,
                Status = !string.IsNullOrEmpty(this.ListStatus) ? this.ListStatus : string.Empty,
                Market = this.MarketName,
                CompanyName = this.CompanyName,
                BuilderName = builderName,
                BrokerOffice = brokerOffice,
                FullLotListing = this,
                UnitNumber = this.UnitNum,
                IsLeasing = isLeasing,
                IsLot = isLot,
                InternalLotRequestId = this.InternalLotRequestID,
                WorkingBy = worker,
                WorkingStatus = workingStatus,
                WorkingSourceAction = workingSourceAction,
            };
    }
}
