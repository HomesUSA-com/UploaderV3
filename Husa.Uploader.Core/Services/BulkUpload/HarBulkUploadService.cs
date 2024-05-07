namespace Husa.Uploader.Core.Services.BulkUpload
{
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;
    using OpenQA.Selenium;

    public class HarBulkUploadService : IHarBulkUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IHarUploadService uploadService;
        private readonly ILogger<HarBulkUploadService> logger;

        public HarBulkUploadService(
            IUploaderClient uploaderClient,
            IHarUploadService uploadService,
            ILogger<HarBulkUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.Houston;

        public RequestFieldChange RequestFieldChange { get; set; }

        public List<UploadListingItem> BulkListings { get; set; }

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public UploadResult Logout()
        {
            throw new NotImplementedException();
        }

        public void SetRequestFieldChange(RequestFieldChange requestFieldChange)
        {
            this.RequestFieldChange = requestFieldChange;
        }

        public void SetBulkListings(List<UploadListingItem> bulkListings)
        {
            this.BulkListings = bulkListings;
        }

        public async Task<UploadResult> Upload(CancellationToken cancellationToken = default)
        {
            if (this.BulkListings == null)
            {
                return UploadResult.Failure;
            }

            foreach (var group in this.BulkListings.GroupBy(item => item.CompanyName))
            {
                await this.ProcessCompanyGroup(group, cancellationToken);
            }

            return UploadResult.Success;
        }

        private async Task ProcessCompanyGroup(IGrouping<string, UploadListingItem> group, CancellationToken cancellationToken)
        {
            var logInForCompany = true;

            foreach (var bulkFullListing in group.Select(bulkListing => bulkListing.FullListing))
            {
                await this.ProcessListing(bulkFullListing, cancellationToken, logInForCompany);
                logInForCompany = false;
                Thread.Sleep(400);
                this.uploaderClient.ClickOnElement(By.Id("m_lbSubmit"));
                Thread.Sleep(400);
            }

            this.uploadService.Logout();
            Thread.Sleep(400);
        }

        private async Task ProcessListing(ResidentialListingRequest bulkFullListing, CancellationToken cancellationToken, bool logInForCompany)
        {
            switch (this.RequestFieldChange)
            {
                case RequestFieldChange.ListPrice:
                    await this.uploadService.UpdatePrice(bulkFullListing, cancellationToken, logInForCompany);
                    break;
                case RequestFieldChange.CompletionDate:
                    await this.uploadService.UpdateCompletionDate(bulkFullListing, cancellationToken, logInForCompany);
                    break;
                case RequestFieldChange.ConstructionStage:
                    await this.uploadService.UpdateStatus(bulkFullListing, cancellationToken, logInForCompany);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
