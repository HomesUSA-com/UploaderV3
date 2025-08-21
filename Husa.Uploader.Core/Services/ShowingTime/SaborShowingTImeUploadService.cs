namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class SaborShowingTimeUploadService : ShowingTimeUploadService
    {
        public SaborShowingTimeUploadService(
            ISaborUploadService marketUploadService,
            ILogger<SaborShowingTimeUploadService> logger)
            : base(marketUploadService, "SABOR:966512", logger)
        {
        }
    }
}
