namespace Husa.Uploader.Core.Services.BulkUpload
{
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;

    public class AborBulkUploadService : IAborBulkUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IAborUploadService uploadService;
        private readonly ILogger<AborBulkUploadService> logger;
        private readonly ISleepService sleepService;

        public AborBulkUploadService(
            IUploaderClient uploaderClient,
            IAborUploadService uploadService,
            ILogger<AborBulkUploadService> logger,
            ISleepService sleepService)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sleepService = sleepService ?? throw new ArgumentNullException(nameof(sleepService));
        }

        public MarketCode CurrentMarket => MarketCode.Austin;

        public RequestFieldChange RequestFieldChange { get; set; }

        public List<UploadListingItem> BulkListings { get; set; }

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public UploaderResponse Logout()
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

        public async Task<UploaderResponse> Upload(CancellationToken cancellationToken = default)
        {
            UploaderResponse response = new UploaderResponse();
            if (this.BulkListings == null)
            {
                response.UploadResult = UploadResult.Failure;
                response.UploadInformation = this.uploaderClient.UploadInformation;
                return response;
            }

            foreach (var group in this.BulkListings.GroupBy(item => item.CompanyName))
            {
                await this.ProcessCompanyGroup(group, cancellationToken, true);
            }

            response.UploadResult = UploadResult.Success;
            return response;
        }

        public async Task<UploaderResponse> Upload_WithAutoSave(CancellationToken cancellationToken = default)
        {
            UploaderResponse response = new UploaderResponse();

            if (this.BulkListings == null)
            {
                response.UploadResult = UploadResult.Failure;
                response.UploadInformation = this.uploaderClient.UploadInformation;
                return response;
            }

            foreach (var group in this.BulkListings.GroupBy(item => item.CompanyName))
            {
                await this.ProcessCompanyGroup(group, cancellationToken, false);
            }

            response.UploadResult = UploadResult.Success;
            return response;
        }

        private async Task ProcessCompanyGroup(IGrouping<string, UploadListingItem> group, CancellationToken cancellationToken, bool autoSave = false)
        {
            var logInForCompany = true;

            foreach (var bulkFullListing in group.Select(bulkListing => bulkListing.FullListing))
            {
                await this.ProcessListing(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                logInForCompany = false;
                this.sleepService.Sleep(400);
            }

            this.uploadService.Logout();
            this.sleepService.Sleep(400);
        }

        private async Task ProcessListing(ResidentialListingRequest bulkFullListing, CancellationToken cancellationToken, bool logInForCompany, bool autoSave = false)
        {
            switch (this.RequestFieldChange)
            {
                case RequestFieldChange.PartialUpload:
                    await this.uploadService.PartialUpload(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                    break;
                case RequestFieldChange.FullUpload:
                    await this.uploadService.Upload(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                    break;
                case RequestFieldChange.ConstructionStage:
                    await this.uploadService.UpdateStatus(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                    break;
                case RequestFieldChange.ListPrice:
                    await this.uploadService.UpdatePrice(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                    break;
                case RequestFieldChange.CompletionDate:
                    await this.uploadService.UpdateCompletionDate(bulkFullListing, cancellationToken, logInForCompany, autoSave);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
