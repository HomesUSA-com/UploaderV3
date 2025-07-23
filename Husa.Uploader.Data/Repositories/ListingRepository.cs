namespace Husa.Uploader.Data.Repositories
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Api.Client;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Data.Projections;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ListingRepository : IListingRepository
    {
        private readonly IQuicklisterDfwClient quicklisterDfwClient;
        private readonly ILogger<ListingRepository> logger;

        public ListingRepository(
            IOptions<ApplicationOptions> applicationOptions,
            IQuicklisterDfwClient quicklisterDfwClient,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<ListingRepository> logger)
        {
            this.quicklisterDfwClient = quicklisterDfwClient ?? throw new ArgumentNullException(nameof(quicklisterDfwClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<TaxIdBulkUploadListingItem>> GetListingsWithInvalidTaxId(MarketCode marketCode, CancellationToken token = default)
        {
            this.logger.LogInformation("Getting listings with invalid Tax Id for the {market} Market", marketCode);
            var listings = marketCode switch
            {
                MarketCode.DFW => await this.quicklisterDfwClient.SaleListing.GetListingsWithInvalidTaxId(token),
                _ => throw new NotSupportedException(),
            };
            return
            [
            .. listings.AsQueryable().Select(result => result.ToTaxIdBulkUploadListingItem(marketCode))
            ];
        }
    }
}
