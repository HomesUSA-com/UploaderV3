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
        private readonly IQuicklisterDfwClient quicklisterDfwClient;
        private readonly ILogger<ListingRequestRepository> logger;
        private readonly MarketConfiguration marketConfiguration;

        public ListingRequestRepository(
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterSaborClient quicklisterSaborClient,
            IQuicklisterCtxClient quicklisterCtxClient,
            IQuicklisterAborClient quicklisterAborClient,
            IQuicklisterHarClient quicklisterHarClient,
            IQuicklisterDfwClient quicklisterDfwClient,
            ILogger<ListingRequestRepository> logger)
        {
            this.quicklisterSaborClient = quicklisterSaborClient ?? throw new ArgumentNullException(nameof(quicklisterSaborClient));
            this.quicklisterCtxClient = quicklisterCtxClient ?? throw new ArgumentNullException(nameof(quicklisterCtxClient));
            this.quicklisterAborClient = quicklisterAborClient ?? throw new ArgumentNullException(nameof(quicklisterAborClient));
            this.quicklisterHarClient = quicklisterHarClient ?? throw new ArgumentNullException(nameof(quicklisterHarClient));
            this.quicklisterDfwClient = quicklisterDfwClient ?? throw new ArgumentNullException(nameof(quicklisterDfwClient));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingRequests(CancellationToken token = default)
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
                this.GetRequestByMarket(
                    this.marketConfiguration.Dfw,
                    this.quicklisterDfwClient.ListingSaleRequest,
                    request => new DfwListingRequest(request).CreateFromApiResponse(),
                    token),
            };

            var requestsByMarket = await Task.WhenAll(tasks);

            var pendingRequests = requestsByMarket.SelectMany(a => a).OrderBy(x => x.SysCreatedOn).ToList();
            return pendingRequests.Distinct();
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingRequestsByMarketAndAction(MarketCode marketCode, RequestFieldChange requestFieldChange, CancellationToken token = default)
        {
            var requestsByMarket = await GetListingRequestsByMarketAction(marketCode, requestFieldChange, token);

            var pendingRequests = requestsByMarket.Select(a => a).OrderBy(x => x.SysCreatedOn).ToList();
            return pendingRequests.Distinct();

            async Task<IEnumerable<ResidentialListingRequest>> GetListingRequestsByMarketAction(MarketCode marketCode, RequestFieldChange requestFieldChange, CancellationToken token)
            {
                switch (marketCode)
                {
                    case MarketCode.SanAntonio:
                        return await this.GetRequestByMarketAndAction(
                            this.marketConfiguration.Sabor,
                            this.quicklisterSaborClient.ListingSaleRequest,
                            request => new SaborListingRequest(request).CreateFromApiResponse(),
                            requestFieldChange,
                            token);
                    case MarketCode.CTX:
                        return await this.GetRequestByMarketAndAction(
                            this.marketConfiguration.Ctx,
                            this.quicklisterCtxClient.ListingSaleRequest,
                            request => new CtxListingRequest(request).CreateFromApiResponse(),
                            requestFieldChange,
                            token);
                    case MarketCode.Austin:
                        return await this.GetRequestByMarketAndAction(
                        this.marketConfiguration.Abor,
                        this.quicklisterAborClient.ListingSaleRequest,
                        request => new AborListingRequest(request).CreateFromApiResponse(),
                        requestFieldChange,
                        token);
                    case MarketCode.Houston:
                        return await this.GetRequestByMarketAndAction(
                        this.marketConfiguration.Har,
                        this.quicklisterHarClient.ListingSaleRequest,
                        request => new HarListingRequest(request).CreateFromApiResponse(),
                        requestFieldChange,
                        token);
                    case MarketCode.DFW:
                        return await this.GetRequestByMarketAndAction(
                        this.marketConfiguration.Dfw,
                        this.quicklisterDfwClient.ListingSaleRequest,
                        request => new DfwListingRequest(request).CreateFromApiResponse(),
                        requestFieldChange,
                        token);
                    default:
                        throw new NotSupportedException($"The market {marketCode} is not yet supported");
                }
            }
        }

        public Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token = default)
        {
            if (residentialListingRequestId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(residentialListingRequestId)}' cannot be null or empty.", nameof(residentialListingRequestId));
            }

            return this.GetRequestById(residentialListingRequestId, marketCode, token);
        }

        public Task<string> GetListingMlsNumber(Guid residentialListingId, MarketCode marketCode, CancellationToken token = default)
        {
            if (residentialListingId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(residentialListingId)}' cannot be null or empty.", nameof(residentialListingId));
            }

            return this.GetListingMlsNumberById(residentialListingId, marketCode, token);
        }

        private async Task<string> GetListingMlsNumberById(Guid residentialListingId, MarketCode marketCode, CancellationToken token)
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
                var listing = await this.quicklisterSaborClient.SaleListing.GetByIdAsync(residentialListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromCtx()
            {
                var listing = await this.quicklisterCtxClient.SaleListing.GetByIdAsync(residentialListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromAbor()
            {
                var listing = await this.quicklisterAborClient.SaleListing.GetByIdAsync(residentialListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromHar()
            {
                var listing = await this.quicklisterHarClient.SaleListing.GetByIdAsync(residentialListingId, token);
                return listing.MlsNumber;
            }

            async Task<string> GetFromDfw()
            {
                var listing = await this.quicklisterDfwClient.SaleListing.GetByIdAsync(residentialListingId, token);
                return listing.MlsNumber;
            }
        }

        private async Task<ResidentialListingRequest> GetRequestById(Guid residentialListingRequestId, MarketCode marketCode, CancellationToken token)
        {
            ResidentialListingRequest listingRequest = marketCode switch
            {
                MarketCode.SanAntonio => await GetFromSabor(),
                MarketCode.CTX => await GetFromCtx(),
                MarketCode.Austin => await GetFromAbor(),
                MarketCode.Houston => await GetFromHar(),
                MarketCode.DFW => await GetFromDfw(),
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

            async Task<ResidentialListingRequest> GetFromDfw()
            {
                var request = await this.quicklisterDfwClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return new DfwListingRequest(request).CreateFromApiResponseDetail();
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

            return new List<ResidentialListingRequest>();
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetRequestByMarketAndAction<TSaleListingRequestResponse, TListingRequestDetailResponse>(
            MarketSettings marketSettings,
            ISaleListingRequest<TSaleListingRequestResponse, TListingRequestDetailResponse> requestClient,
            Expression<Func<TSaleListingRequestResponse, ResidentialListingRequest>> projection,
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

            return new List<ResidentialListingRequest>();
        }
    }
}
