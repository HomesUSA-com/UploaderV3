namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class AmarilloShowingTimeUploadService : ShowingTimeUploadService
    {
        public AmarilloShowingTimeUploadService(
            IAmarilloUploadService marketUploadService,
            ILogger<AmarilloShowingTimeUploadService> logger)
            : base(marketUploadService, "SABOR:966512", logger)
        {
        }
    }
}
