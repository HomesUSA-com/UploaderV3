namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Microsoft.Extensions.Logging;

    public class SaborShowingTimeUploadService : ShowingTimeUploadService
    {
        public SaborShowingTimeUploadService(
            ISaborUploadService marketUploadService,
            ILogger<SaborShowingTimeUploadService> logger,
            ISleepService sleepService)
            : base(marketUploadService, "SABOR:966512", logger, sleepService)
        {
        }
    }
}
