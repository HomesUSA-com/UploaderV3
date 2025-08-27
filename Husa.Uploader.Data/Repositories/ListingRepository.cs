namespace Husa.Uploader.Data.Repositories
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Quicklister.Har.Api.Client;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Data.Projections;
    using Microsoft.Extensions.Logging;

    public class ListingRepository : IListingRepository
    {
        private readonly IQuicklisterDfwClient quicklisterDfwClient;
        private readonly IQuicklisterHarClient quicklisterHarClient;
        private readonly ILogger<ListingRepository> logger;

        public ListingRepository(
            IQuicklisterDfwClient quicklisterDfwClient,
            IQuicklisterHarClient quicklisterHarClient,
            ILogger<ListingRepository> logger)
        {
            this.quicklisterDfwClient = quicklisterDfwClient ?? throw new ArgumentNullException(nameof(quicklisterDfwClient));
            this.quicklisterHarClient = quicklisterHarClient ?? throw new ArgumentNullException(nameof(quicklisterHarClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<TaxIdBulkUploadListingItem>> GetListingsWithInvalidTaxId(MarketCode marketCode, CancellationToken token = default)
        {
            this.logger.LogInformation("Getting listings with invalid Tax Id for the {market} Market", marketCode);
            var listings = marketCode switch
            {
                MarketCode.DFW => await this.quicklisterDfwClient.SaleListing.GetListingsWithInvalidTaxId(token),
                MarketCode.Houston => await this.quicklisterHarClient.SaleListing.GetListingsWithInvalidTaxId(token),
                _ => throw new NotSupportedException(),
            };
            return
            [
            .. listings.AsQueryable().Select(result => result.ToTaxIdBulkUploadListingItem(marketCode))
            ];
        }
    }
}
