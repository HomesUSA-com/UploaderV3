namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class HarShowingTimeUploadService : ShowingTimeUploadService
    {
        public HarShowingTimeUploadService(
            IHarUploadService marketUploadService,
            ILogger<DfwShowingTimeUploadService> logger)
            : base(marketUploadService, "SABOR:966512", logger)
        {
        }
    }
}
