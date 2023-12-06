namespace Husa.Uploader.Data.Repositories
{
    using System.Linq;
    using System.Linq.Expressions;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Extensions.Api.Client.Interfaces;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using QuicklisterStatus = Husa.Quicklister.Extensions.Domain.Enums.ListingRequestState;

    public class ListingRequestRepository : IListingRequestRepository
    {
        private readonly IQuicklisterSaborClient quicklisterSaborClient;
        private readonly IQuicklisterCtxClient quicklisterCtxClient;
        private readonly IQuicklisterAborClient quicklisterAborClient;
        private readonly IQuicklisterHarClient quicklisterHarClient;
        private readonly ILogger<ListingRequestRepository> logger;
        private readonly MarketConfiguration marketConfiguration;

        public ListingRequestRepository(
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterSaborClient quicklisterSaborClient,
            IQuicklisterCtxClient quicklisterCtxClient,
            IQuicklisterAborClient quicklisterAborClient,
            IQuicklisterHarClient quicklisterHarClient,
            ILogger<ListingRequestRepository> logger)
        {
            this.quicklisterSaborClient = quicklisterSaborClient ?? throw new ArgumentNullException(nameof(quicklisterSaborClient));
            this.quicklisterCtxClient = quicklisterCtxClient ?? throw new ArgumentNullException(nameof(quicklisterCtxClient));
            this.quicklisterAborClient = quicklisterAborClient ?? throw new ArgumentNullException(nameof(quicklisterAborClient));
            this.quicklisterHarClient = quicklisterHarClient ?? throw new ArgumentNullException(nameof(quicklisterHarClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default)
        {
            var tasks = new[]
            {
                this.GetRequestByMarket(
                    this.marketConfiguration.Sabor,
                    this.quicklisterSaborClient.ListingSaleRequest,
                    request => new SaborListingRequest(request).CreateFromApiResponse(),
                    token),
                this.GetRequestByMarket(
                    this.marketConfiguration.Ctx,
                    this.quicklisterCtxClient.ListingSaleRequest,
                    request => new CtxListingRequest(request).CreateFromApiResponse(),
                    token),
                this.GetRequestByMarket(
                    this.marketConfiguration.Abor,
                    this.quicklisterAborClient.ListingSaleRequest,
                    request => new AborListingRequest(request).CreateFromApiResponse(),
                    token),
                this.GetRequestByMarket(
                    this.marketConfiguration.Har,
                    this.quicklisterHarClient.ListingSaleRequest,
                    request => new HarListingRequest(request).CreateFromApiResponse(),
                    token),
            };

            var requestsByMarket = await Task.WhenAll(tasks);

            var pendingRequests = requestsByMarket.SelectMany(a => a).ToList();
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
                MarketCode.Austin => await GetFromAbor(),
                MarketCode.Houston => await GetFromHar(),
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

            async Task<ResidentialListingRequest> GetFromAbor()
            {
                var request = await this.quicklisterAborClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return new AborListingRequest(request).CreateFromApiResponseDetail();
            }

            async Task<ResidentialListingRequest> GetFromHar()
            {
                var request = await this.quicklisterHarClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return new HarListingRequest(request).CreateFromApiResponseDetail();
            }
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetRequestByMarket<TSaleListingRequestResponse, TListingRequestDetailResponse>(
            MarketSettings marketSettings,
            ISaleListingRequest<TSaleListingRequestResponse, TListingRequestDetailResponse> requestClient,
            Expression<Func<TSaleListingRequestResponse, ResidentialListingRequest>> projection,
            CancellationToken token = default)
            where TSaleListingRequestResponse : class, ISaleListingRequestResponse
            where TListingRequestDetailResponse : IListingRequestDetailResponse
        {
            if (marketSettings.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for {marketCode}", marketSettings.MarketCode);

                var requests = await requestClient.GetListRequestAsync(
                    new()
                    {
                        RequestState = QuicklisterStatus.Pending,
                    },
                    token);

                if (requests.Data.Any())
                {
                    return requests.Data.AsQueryable().Select(projection).ToList();
                }
            }

            return new List<ResidentialListingRequest>();
        }
    }
}
