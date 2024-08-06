namespace Husa.Uploader.Data.Repositories
{
    using System.Linq;
    using System.Linq.Expressions;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Abor.Api.Client;
    using Husa.Quicklister.CTX.Api.Client;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Extensions.Api.Client.Interfaces;
    using Husa.Quicklister.Extensions.Api.Contracts.Request.SaleRequest;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Entities.MarketRequests.LotRequest;
    using Husa.Uploader.Data.Interfaces.LotListing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using QuicklisterStatus = Husa.Quicklister.Extensions.Domain.Enums.ListingRequestState;

    public class LotListingRequestRepository : ILotListingRequestRepository
    {
        private readonly IQuicklisterSaborClient quicklisterSaborClient;
        private readonly IQuicklisterCtxClient quicklisterCtxClient;
        private readonly IQuicklisterAborClient quicklisterAborClient;
        private readonly IQuicklisterHarClient quicklisterHarClient;
        private readonly IQuicklisterDfwClient quicklisterDfwClient;
        private readonly ILogger<LotListingRequestRepository> logger;
        private readonly MarketConfiguration marketConfiguration;

        public LotListingRequestRepository(
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterSaborClient quicklisterSaborClient,
            IQuicklisterCtxClient quicklisterCtxClient,
            IQuicklisterAborClient quicklisterAborClient,
            IQuicklisterHarClient quicklisterHarClient,
            IQuicklisterDfwClient quicklisterDfwClient,
            ILogger<LotListingRequestRepository> logger)
        {
            this.quicklisterSaborClient = quicklisterSaborClient ?? throw new ArgumentNullException(nameof(quicklisterSaborClient));
            this.quicklisterCtxClient = quicklisterCtxClient ?? throw new ArgumentNullException(nameof(quicklisterCtxClient));
            this.quicklisterAborClient = quicklisterAborClient ?? throw new ArgumentNullException(nameof(quicklisterAborClient));
            this.quicklisterHarClient = quicklisterHarClient ?? throw new ArgumentNullException(nameof(quicklisterHarClient));
            this.quicklisterDfwClient = quicklisterDfwClient ?? throw new ArgumentNullException(nameof(quicklisterDfwClient));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        public async Task<IEnumerable<LotListingRequest>> GetListingRequests(CancellationToken token = default)
        {
            var tasks = new[]
            {
                this.GetRequestByMarket(
                    this.marketConfiguration.Abor,
                    this.quicklisterAborClient.ListingLotRequest,
                    request => new AborLotListingRequest(request).CreateFromApiResponse(),
                    token),
            };

            var requestsByMarket = await Task.WhenAll(tasks);

            var pendingRequests = requestsByMarket.SelectMany(a => a).OrderBy(x => x.SysCreatedOn).ToList();
            return pendingRequests.Distinct();
        }

        public async Task<IEnumerable<LotListingRequest>> GetListingRequestsByMarketAndAction(MarketCode marketCode, RequestFieldChange requestFieldChange, CancellationToken token = default)
        {
            var requestsByMarket = await GetListingRequestsByMarketAction(marketCode, requestFieldChange, token);

            var pendingRequests = requestsByMarket.Select(a => a).OrderBy(x => x.SysCreatedOn).ToList();
            return pendingRequests.Distinct();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable IDE0062 // Make local function 'static'
            async Task<IEnumerable<LotListingRequest>> GetListingRequestsByMarketAction(MarketCode marketCode, RequestFieldChange requestFieldChange, CancellationToken token)
            {
                throw new NotImplementedException();
            }
#pragma warning restore IDE0062 // Make local function 'static'
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        public Task<LotListingRequest> GetListingRequest(Guid lotListingRequestId, MarketCode marketCode, CancellationToken token = default)
        {
            if (lotListingRequestId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(lotListingRequestId)}' cannot be null or empty.", nameof(lotListingRequestId));
            }

            return this.GetRequestById(lotListingRequestId, marketCode, token);
        }

        public Task<string> GetListingMlsNumber(Guid lotListingId, MarketCode marketCode, CancellationToken token = default)
        {
            if (lotListingId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(lotListingId)}' cannot be null or empty.", nameof(lotListingId));
            }

            return this.GetListingMlsNumberById(lotListingId, marketCode, token);
        }

        private async Task<string> GetListingMlsNumberById(Guid lotListingId, MarketCode marketCode, CancellationToken token)
        {
            var mlsNumber = marketCode switch
            {
                MarketCode.SanAntonio => await GetFromSabor(),
                MarketCode.CTX => await GetFromCtx(),
                MarketCode.Austin => await GetFromAbor(),
                MarketCode.Houston => await GetFromHar(),
                MarketCode.DFW => await GetFromDfw(),
                _ => throw new NotSupportedException($"The market {marketCode} is not yet supported"),
            };

            return mlsNumber;

            async Task<string> GetFromSabor()
            {
                var listing = await this.quicklisterSaborClient.SaleListing.GetByIdAsync(lotListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromCtx()
            {
                var listing = await this.quicklisterCtxClient.SaleListing.GetByIdAsync(lotListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromAbor()
            {
                var listing = await this.quicklisterAborClient.SaleListing.GetByIdAsync(lotListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromHar()
            {
                var listing = await this.quicklisterHarClient.SaleListing.GetByIdAsync(lotListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromDfw()
            {
                var listing = await this.quicklisterDfwClient.SaleListing.GetByIdAsync(lotListingId, token);
                return listing.MlsNumber;
            }
        }

        private async Task<LotListingRequest> GetRequestById(Guid lotListingRequestId, MarketCode marketCode, CancellationToken token)
        {
            LotListingRequest listingRequest = marketCode switch
            {
                MarketCode.Austin => await GetLotFromAbor(),
                _ => throw new NotSupportedException($"The market {marketCode} is not yet supported for Lot Listings."),
            };

            return listingRequest ?? null;

            async Task<LotListingRequest> GetLotFromAbor()
            {
                var request = await this.quicklisterAborClient.ListingLotRequest.GetListRequestSaleByIdAsync(lotListingRequestId, token);
                return new AborLotListingRequest(request).CreateFromApiResponseDetail();
            }
        }

        private async Task<IEnumerable<LotListingRequest>> GetRequestByMarket<TLotListingRequestResponse, TListingRequestDetailResponse>(
            MarketSettings marketSettings,
            ISaleListingRequest<TLotListingRequestResponse, TListingRequestDetailResponse> requestClient,
            Expression<Func<TLotListingRequestResponse, LotListingRequest>> projection,
            CancellationToken token = default)
            where TLotListingRequestResponse : class, ISaleListingRequestResponse
            where TListingRequestDetailResponse : IListingRequestDetailResponse
        {
            if (marketSettings.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for {MarketCode}", marketSettings.MarketCode);

                var saleListingRequestFilter = new SaleListingRequestFilter()
                {
                    RequestState = QuicklisterStatus.Pending,
                };

                var requests = await requestClient.GetListRequestAsync(
                    saleListingRequestFilter,
                    token);

                if (requests.Data.Any())
                {
                    return requests.Data.AsQueryable().Select(projection).ToList();
                }
            }

            return new List<LotListingRequest>();
        }

        private async Task<IEnumerable<LotListingRequest>> GetRequestByMarketAndAction<TSaleListingRequestResponse, TListingRequestDetailResponse>(
            MarketSettings marketSettings,
            ISaleListingRequest<TSaleListingRequestResponse, TListingRequestDetailResponse> requestClient,
            Expression<Func<TSaleListingRequestResponse, LotListingRequest>> projection,
            RequestFieldChange requestFieldChange,
            CancellationToken token = default)
            where TSaleListingRequestResponse : class, ISaleListingRequestResponse
            where TListingRequestDetailResponse : IListingRequestDetailResponse
        {
            if (marketSettings.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for {MarketCode} and for action {requestFieldChange}.", marketSettings.MarketCode, requestFieldChange);

                var saleListingRequestFilter = new SaleListingRequestFilter()
                {
                    RequestState = QuicklisterStatus.Pending,
                    RequestFieldChange = requestFieldChange,
                };

                var requests = await requestClient.GetListRequestAsync(
                    saleListingRequestFilter,
                    token);

                if (requests.Data.Any())
                {
                    return requests.Data.AsQueryable().Select(projection).ToList();
                }
            }

            return new List<LotListingRequest>();
        }
    }
}
