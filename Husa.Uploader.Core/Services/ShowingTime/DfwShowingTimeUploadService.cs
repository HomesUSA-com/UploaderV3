namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class DfwShowingTimeUploadService : ShowingTimeUploadService
    {
        public DfwShowingTimeUploadService(
            IDfwUploadService marketUploadService,
            ILogger<DfwShowingTimeUploadService> logger)
            : base(marketUploadService, logger)
        {
        }
    }
}
