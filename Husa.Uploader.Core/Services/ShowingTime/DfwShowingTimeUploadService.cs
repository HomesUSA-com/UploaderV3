namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Microsoft.Extensions.Logging;

    public class DfwShowingTimeUploadService : ShowingTimeUploadService
    {
        public DfwShowingTimeUploadService(
            IDfwUploadService marketUploadService,
            ILogger<DfwShowingTimeUploadService> logger,
            ISleepService sleepService)
            : base(marketUploadService, "SABOR:966512", logger, sleepService)
        {
        }
    }
}
