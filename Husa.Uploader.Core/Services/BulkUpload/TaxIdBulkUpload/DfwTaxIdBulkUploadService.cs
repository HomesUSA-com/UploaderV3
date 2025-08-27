namespace Husa.Uploader.Core.Services.BulkUpload.TaxIdBulkUpload
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload.TaxIdBulkUpload;
    using Microsoft.Extensions.Logging;

    public class DfwTaxIdBulkUploadService : TaxIdBulkUploadService<IDfwUploadService>, IDfwTaxIdBulkUploadService
    {
        public DfwTaxIdBulkUploadService(
            IDfwUploadService uploadService,
            ILogger<DfwTaxIdBulkUploadService> logger)
            : base(uploadService, logger)
        {
        }

        public override MarketCode CurrentMarket => MarketCode.DFW;
    }
}
