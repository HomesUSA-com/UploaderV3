namespace Husa.Uploader.Api.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("upload")]
    public class UploadController : ControllerBase
    {
        private readonly IUploaderServiceFactory serviceFactory;
        private readonly ILogger<UploadController> logger;

        public UploadController(IUploaderServiceFactory serviceFactory, IMapper mapper, ILogger<UploadController> logger)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{marketCode}/request/{requestId:guid}")]
        public async Task<IActionResult> GetFullUploadListing(
            [FromRoute][Required(AllowEmptyStrings = false)] MarketCode marketCode,
            [FromRoute][Required] Guid requestId,
            CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Starting To Upload listing for market {MarketCode} and requestId {RequestId}", marketCode, requestId);
            var currentService = this.serviceFactory.GetService(marketCode);
            var result = await currentService.FullUploadListing(marketCode, requestId, cancellationToken);
            return this.Ok(result);
        }
    }
}
