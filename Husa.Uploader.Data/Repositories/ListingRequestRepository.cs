namespace Husa.Uploader.Data.Repositories
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using CtxContracts = Husa.Quicklister.CTX.Api.Contracts;
    using QuicklisterStatus = Husa.Quicklister.Extensions.Domain.Enums.ListingRequestState;
    using SaborContracts = Husa.Quicklister.Sabor.Api.Contracts;

    public class ListingRequestRepository : IListingRequestRepository
    {
        private readonly IQuicklisterSaborClient quicklisterSaborClient;
        private readonly IQuicklisterCtxClient quicklisterCtxClient;
        private readonly ILogger<ListingRequestRepository> logger;
        private readonly MarketConfiguration marketConfiguration;

        public ListingRequestRepository(
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterSaborClient quicklisterSaborClient,
            IQuicklisterCtxClient quicklisterCtxClient,
            ILogger<ListingRequestRepository> logger)
        {
            this.quicklisterSaborClient = quicklisterSaborClient ?? throw new ArgumentNullException(nameof(quicklisterSaborClient));
            this.quicklisterCtxClient = quicklisterCtxClient ?? throw new ArgumentNullException(nameof(quicklisterCtxClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default)
        {
            var pendingRequests = new List<ResidentialListingRequest>();
            if (this.marketConfiguration.Sabor.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for {marketCode}", MarketCode.SanAntonio);
                var filter = new SaborContracts.Request.SaleRequest.ListingSaleRequestFilter
                {
                    RequestState = QuicklisterStatus.Pending,
                };

                var requests = await this.quicklisterSaborClient.ListingSaleRequest.GetListRequestAsync(filter, token);
                if (requests.Data.Any())
                {
                    var internalSARLRCosmo = requests.Data
                        .Select(request => new SaborListingRequest(request).CreateFromApiResponse())
                        .ToList();

                    pendingRequests.AddRange(internalSARLRCosmo);
                }
            }

            if (this.marketConfiguration.Ctx.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for {marketCode}", MarketCode.CTX);

                var filter = new CtxContracts.Request.SaleRequest.ListingSaleRequestFilter
                {
                    RequestState = QuicklisterStatus.Pending,
                };
                var requests = await this.quicklisterCtxClient.ListingSaleRequest.GetListRequestAsync(filter, token);
                if (requests.Data.Any())
                {
                    var internalSARLRCosmo = requests.Data
                        .Select(request => new CtxListingRequest(request).CreateFromApiResponse())
                        .ToList();

                    pendingRequests.AddRange(internalSARLRCosmo);
                }
            }

            return pendingRequests.Distinct();
        }

        public Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token = default)
        {
            if (residentialListingRequestId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(residentialListingRequestId)}' cannot be null or empty.", nameof(residentialListingRequestId));
            }

            return this.GetRequestById(residentialListingRequestId, marketCode, token);
        }

        private async Task<ResidentialListingRequest> GetRequestById(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token)
        {
            ResidentialListingRequest listingRequest = marketCode switch
            {
                MarketCode.SanAntonio => await GetFromSabor(),
                MarketCode.CTX => await GetFromCtx(),
                _ => throw new NotSupportedException($"The market {marketCode} is not yet supported"),
            };

            return listingRequest ?? null;

            async Task<ResidentialListingRequest> GetFromSabor()
            {
                var request = await this.quicklisterSaborClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return new SaborListingRequest(request).CreateFromApiResponseDetail();
            }

            async Task<ResidentialListingRequest> GetFromCtx()
            {
                var request = await this.quicklisterCtxClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return new CtxListingRequest(request).CreateFromApiResponseDetail();
            }
        }
    }
}
