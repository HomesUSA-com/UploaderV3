namespace Husa.Uploader.Core.Services.ShowingTime
{
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CtxShowingTimeUploadService : ShowingTimeUploadService
    {
        public CtxShowingTimeUploadService(
            ICtxUploadService marketUploadService,
            ILogger<CtxShowingTimeUploadService> logger)
            : base(marketUploadService, "SABOR:966512", logger)
        {
        }
    }
}
