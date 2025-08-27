namespace Husa.Uploader.Core.Services.BulkUpload.TaxIdBulkUpload
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload.TaxIdBulkUpload;
    using Microsoft.Extensions.Logging;

    public class HarTaxIdBulkUploadService : TaxIdBulkUploadService<IHarUploadService>, IHarTaxIdBulkUploadService
    {
        public HarTaxIdBulkUploadService(
            IUploaderClient uploaderClient,
            IHarUploadService uploadService,
            ILogger<HarTaxIdBulkUploadService> logger)
            : base(uploaderClient, uploadService, logger)
        {
        }

        public override MarketCode CurrentMarket => MarketCode.Houston;
    }
}
