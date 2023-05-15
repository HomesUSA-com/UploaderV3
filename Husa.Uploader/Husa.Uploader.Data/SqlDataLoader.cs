using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Crosscutting.Options;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;
using Husa.Uploader.Data.QuicklisterEntities.Ctx;
using Husa.Uploader.Data.QuicklisterEntities.Sabor;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Husa.Uploader.Data
{
    public class SqlDataLoader : ISqlDataLoader
    {
        private readonly CosmosClient cosmosClient;
        private readonly IOptions<CosmosDbOptions> options;
        private readonly HttpClient httpClient;
        private readonly ILogger<SqlDataLoader> logger;
        private readonly MarketConfiguration marketConfiguration;

        public SqlDataLoader(
            CosmosClient cosmosClient,
            IOptions<CosmosDbOptions> options,
            IOptions<ApplicationOptions> applicationOptions,
            HttpClient httpClient,
            ILogger<SqlDataLoader> logger)
        {
            if (applicationOptions is null)
            {
                throw new ArgumentNullException(nameof(applicationOptions));
            }

            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.marketConfiguration = applicationOptions?.Value?.MarketInfo ?? throw new ArgumentNullException(nameof(applicationOptions));
        }

        //FIXME: This method is incomplete
        public async Task<IEnumerable<ResidentialListingRequest>> GetListingData(CancellationToken token = default)
        {
            var pendingRequests = new List<ResidentialListingRequest>();
            if (this.marketConfiguration.Sabor.IsEnabled)
            {
                var internalSARLRCosmo = await this.GetMarketData<SaborListingRequestSale>(
                    this.marketConfiguration.Sabor,
                    this.options.Value.Databases.SaborDatabase,
                    token);
                if (internalSARLRCosmo != null)
                {
                    pendingRequests.AddRange(internalSARLRCosmo);
                }
            }

            if (this.marketConfiguration.Ctx.IsEnabled)
            {
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

        public IEnumerable<IListingMedia> GetListingMedia(Guid residentialListingRequestId, string marketName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingRequest(string residentialListingRequestId, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(residentialListingRequestId))
            {
                throw new ArgumentException($"'{nameof(residentialListingRequestId)}' cannot be null or empty.", nameof(residentialListingRequestId));
            }
            var marketInfo = this.marketConfiguration.Sabor;
            var saleContainer = this.cosmosClient.GetContainer(this.options.Value.Databases.SaborDatabase, this.options.Value.SaleCollectionName);
            using var query =
                saleContainer.GetItemLinqQueryable<SaborListingRequestSale>(allowSynchronousQueryExecution: false)
                .Where(listingRequest => !listingRequest.IsDeleted)
                .Where(listingRequest => listingRequest.Id == residentialListingRequestId)
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

        private async Task<byte[]> GetByteArrayFromURL(string url)
        {
            var imageBytes =  await this.httpClient.GetByteArrayAsync(url);
            return imageBytes;
        }

        private static string GetExtension(byte[] data)
        {
            if (IsPng(data))
                return ".png";

            if (IsJpg(data))
                return ".jpg";

            if (IsGif(data))
                return ".gif";

            return null;
        }

        private static bool IsPng(byte[] data)
        {
            return data != null && data.Length > 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E &&
                   data[3] == 0x47 && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A;
        }

        private static bool IsJpg(byte[] data)
        {
            return data != null && data.Length > 4 && data[0] == 0xff && data[1] == 0xd8 && data[data.Length - 2] == 0xff && data[data.Length - 1] == 0xd9;
        }

        private static bool IsGif(byte[] data)
        {
            return data != null && data.Length > 6 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38 && (data[4] == 0x39 || data[4] == 0x37) && data[5] == 0x61;
        }
    }
}
