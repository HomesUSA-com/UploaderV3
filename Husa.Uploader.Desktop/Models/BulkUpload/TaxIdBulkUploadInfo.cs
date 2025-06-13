namespace Husa.Uploader.Desktop.Models.BulkUpload
{
    using Husa.Extensions.Common.Enums;

    public record TaxIdBulkUploadInfo
    {
        public TaxIdBulkUploadInfo(MarketCode market)
        {
            this.Market = market;
        }

        public TaxIdBulkUploadInfo()
        {
        }

        public MarketCode? Market { get; init; }

        public bool IsValid => this.Market.HasValue;
    }
}
