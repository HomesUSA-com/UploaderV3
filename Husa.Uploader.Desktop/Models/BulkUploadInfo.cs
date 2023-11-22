namespace Husa.Uploader.Desktop.Models
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;

    public record BulkUploadInfo
    {
        public BulkUploadInfo(RequestFieldChange requestFieldChange, MarketCode market)
        {
            this.RequestFieldChange = requestFieldChange;
            this.Market = market;
        }

        public BulkUploadInfo()
        {
        }

        public RequestFieldChange? RequestFieldChange { get; init; }

        public MarketCode? Market { get; init; }

        public bool IsValid => this.RequestFieldChange.HasValue && this.Market.HasValue;
    }
}
