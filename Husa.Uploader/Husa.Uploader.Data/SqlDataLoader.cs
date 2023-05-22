namespace Husa.Uploader.Data
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Sabor.Api.Client;
    using Husa.Quicklister.Sabor.Api.Contracts.Request.SaleRequest;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.QuicklisterEntities.Ctx;
    using Husa.Uploader.Data.QuicklisterEntities.Sabor;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using QuickliserStatus = Husa.Quicklister.Extensions.Domain.Enums.ListingRequestState;

    public class SqlDataLoader : ISqlDataLoader
    {
        private readonly CosmosClient cosmosClient;
        private readonly IOptions<CosmosDbOptions> options;
        private readonly IQuicklisterSaborClient quicklisterSaborClient;
        private readonly ILogger<SqlDataLoader> logger;
        private readonly MarketConfiguration marketConfiguration;

        public SqlDataLoader(
            CosmosClient cosmosClient,
            IOptions<CosmosDbOptions> options,
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterSaborClient quicklisterSaborClient,
            ILogger<SqlDataLoader> logger)
        {
            if (applicationOptions is null)
            {
                throw new ArgumentNullException(nameof(applicationOptions));
            }

            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.quicklisterSaborClient = quicklisterSaborClient ?? throw new ArgumentNullException(nameof(quicklisterSaborClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        //// FIXME: This method is incomplete, we should get the listings from the API, not cosmos DB directly
        public async Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default)
        {
            var pendingRequests = new List<ResidentialListingRequest>();
            if (this.marketConfiguration.Sabor.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for San Antonio");
                var filter = new ListingSaleRequestFilter
                {
                    RequestState = QuickliserStatus.Pending,
                };

                var requests = await this.quicklisterSaborClient.ListingSaleRequest.GetListRequestAsync(filter, token);

                if (requests.Data.Any())
                {
                    var internalSARLRCosmo = requests.Data
                        .Select(request => ResidentialListingRequest.CreateFromApiResponse(request, MarketCode.SanAntonio))
                        .ToList();

                    pendingRequests.AddRange(internalSARLRCosmo);
                }
            }

            if (this.marketConfiguration.Ctx.IsEnabled)
            {
                this.logger.LogInformation("Getting all pending requests for CTX");
                var internalSACTXRLRCosmo = await this.GetMarketData<CtxListingRequest>(
                    this.marketConfiguration.Ctx,
                    this.options.Value.Databases.CtxDatabase,
                    token);
                if (internalSACTXRLRCosmo != null)
                {
                    pendingRequests.AddRange(internalSACTXRLRCosmo);
                }
            }

            return pendingRequests.Distinct();
        }

        public Task<ResidentialListingRequest> GetListingRequest(Guid residentialListingRequestId, CancellationToken token = default)
        {
            if (residentialListingRequestId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(residentialListingRequestId)}' cannot be null or empty.", nameof(residentialListingRequestId));
            }

            return GetRequestById();

            async Task<ResidentialListingRequest> GetRequestById()
            {
                var request = await this.quicklisterSaborClient.ListingSaleRequest.GetListRequestSaleByIdAsync(residentialListingRequestId, token);
                return request != null ?
                    ResidentialListingRequest.CreateFromApiResponseDetail(request, MarketCode.SanAntonio) :
                    null;
            }
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetMarketData<T>(
            MarketSettings marketInfo,
            string databaseName,
            CancellationToken token = default)
            where T : IConvertToUploaderRequest
        {
            var saleContainer = this.cosmosClient.GetContainer(databaseName, this.options.Value.SaleCollectionName);
            using var query =
                saleContainer.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: false)
                .Where(listingRequest => !listingRequest.IsDeleted)
                .Where(listingRequest => listingRequest.RequestState == ListingRequestState.Pending)
                .ToFeedIterator();

            if (query.HasMoreResults)
            {
                var results = (await query.ReadNextAsync(token)).ToList();
                return results
                    .Select(request => request.ConvertFromCosmos(
                        marketName: marketInfo.Name,
                        marketUser: marketInfo.Username,
                        marketPassword: marketInfo.Password))
                    .ToList();
            }

            return null;
        }
    }
}
