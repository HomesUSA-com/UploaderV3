namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class AborShowingTimeUploadService : ShowingTimeUploadService
    {
        public AborShowingTimeUploadService(
            IAborUploadService marketUploadService,
            ILogger<AborShowingTimeUploadService> logger)
            : base(marketUploadService, "SABOR:966512", logger)
        {
        }
    }
}
