namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CtxShowingTimeUploadService : ShowingTimeUploadService
    {
        public CtxShowingTimeUploadService(
            ICtxUploadService marketUploadService,
            ILogger<CtxShowingTimeUploadService> logger,
            ISleepService sleepService)
            : base(marketUploadService, "SABOR:966512", logger, sleepService)
        {
        }
    }
}
