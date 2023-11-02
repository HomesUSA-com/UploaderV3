namespace Husa.Uploader.Core.Services.BulkUpload
{
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload;
    using Husa.Uploader.Crosscutting.Enums;
    using Microsoft.Extensions.Logging;

    public class SaborBulkUploadService : ISaborBulkUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly ILogger<SaborBulkUploadService> logger;

        public SaborBulkUploadService(
            IUploaderClient uploaderClient,
            ILogger<SaborBulkUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.SanAntonio;

        public RequestFieldChange RequestFieldChange { get; set; }

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public UploadResult Logout()
        {
            throw new NotImplementedException();
        }

        public void SetRequestFieldChange(RequestFieldChange requestFieldChange)
        {
            this.RequestFieldChange = requestFieldChange;
        }

        public Task<UploadResult> Upload(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
