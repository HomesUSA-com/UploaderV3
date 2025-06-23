namespace Husa.Uploader.Core.Services.BulkUpload.TaxIdBulkUpload
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload.TaxIdBulkUpload;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Microsoft.Extensions.Logging;

    public class DfwTaxIdBulkUploadService : IDfwTaxIdBulkUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IDfwUploadService uploadService;
        private readonly ILogger<DfwTaxIdBulkUploadService> logger;

        public DfwTaxIdBulkUploadService(
            IUploaderClient uploaderClient,
            IDfwUploadService uploadService,
            ILogger<DfwTaxIdBulkUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.DFW;

        public List<TaxIdBulkUploadListingItem> BulkListings { get; set; }

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public UploaderResponse Logout()
        {
            throw new NotImplementedException();
        }

        public void SetBulkListings(List<TaxIdBulkUploadListingItem> bulkListings)
        {
            this.BulkListings = bulkListings;
        }

        public async Task<UploaderResponse> Upload(CancellationToken cancellationToken = default)
        {
            UploaderResponse response = new();
            if (this.BulkListings == null)
            {
                response.UploadResult = UploadResult.Failure;
                response.UploadInformation = this.uploaderClient.UploadInformation;
                return response;
            }

            foreach (var group in this.BulkListings.GroupBy(item => item.OwnerName))
            {
                await this.ProcessCompanyGroup(group, cancellationToken);
            }

            response.UploadResult = UploadResult.Success;
            return response;
        }

        private async Task ProcessCompanyGroup(IGrouping<string, TaxIdBulkUploadListingItem> group, CancellationToken cancellationToken = default)
        {
            var logInForCompany = true;

            foreach (var bulkFullListing in group)
            {
                await this.uploadService.TaxIdUpdate(bulkFullListing, logInForCompany, cancellationToken: cancellationToken);
                logInForCompany = false;
            }

            this.uploadService.Logout();
        }
    }
}
