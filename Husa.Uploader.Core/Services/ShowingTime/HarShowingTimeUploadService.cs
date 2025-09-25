namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Microsoft.Extensions.Logging;

    public class HarShowingTimeUploadService : ShowingTimeUploadService
    {
        public HarShowingTimeUploadService(
            IHarUploadService marketUploadService,
            ILogger<HarShowingTimeUploadService> logger,
            ISleepService sleepService)
            : base(marketUploadService, "SABOR:966512", logger, sleepService)
        {
        }
    }
}
