namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.Common.Exceptions;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Core.Services.Common;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;

    /// <summary>
    /// Centralizes common behavior for market upload services.
    /// Concrete market services (ABOR, HAR, DFW, CTX, SABOR, Amarillo, etc.) should inherit this class.
    /// </summary>
    public abstract class MarketUploadService : IMarketUploadService
    {
        protected readonly IUploaderClient uploaderClient;
        protected readonly IMediaRepository mediaRepository;
        protected readonly IServiceSubscriptionClient serviceSubscriptionClient;
        protected readonly IListingRequestRepository listingRequestRepository;
        protected readonly ApplicationOptions options;
        protected readonly ILogger logger;
        protected readonly ISleepService sleepService;

        protected MarketUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IListingRequestRepository listingRequestRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger logger,
            ISleepService sleepService)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.listingRequestRepository = listingRequestRepository ?? throw new ArgumentNullException(nameof(listingRequestRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sleepService = sleepService ?? throw new ArgumentNullException(nameof(sleepService));
        }

        public IUploaderClient UploaderClient => this.uploaderClient;

        public IServiceSubscriptionClient ServiceSubscriptionClient => this.serviceSubscriptionClient;

        public abstract MarketCode CurrentMarket { get; }

        public virtual bool IsFlashRequired => false;

        public virtual bool CanUpload(ResidentialListingRequest listing)
            => listing != null && listing.MarketCode == this.CurrentMarket;

        public virtual void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public virtual async Task<UploaderResponse> FullUploadListing(MarketCode marketCode, Guid requestId, CancellationToken cancellationToken, bool logInFirstStepOnly = true, bool autoSave = true)
        {
            var listing = await this.listingRequestRepository.GetListingRequest(
                requestId,
                marketCode,
                cancellationToken)
                ?? throw new NotFoundException<ResidentialListingRequest>(requestId);

            listing.WorkingBy = "Uploader Aut";
            try
            {
                var core = await this.Upload(
                    listing,
                    cancellationToken: cancellationToken,
                    logIn: true,
                    autoSave: autoSave);

                if (core.UploadResult != UploadResult.Success)
                {
                    return core;
                }

                return CreateSuccessResponse();
            }
            catch (OperationCanceledException ex)
            {
                this.logger.LogWarning(ex, "FullUploadListing canceled for MLS {MLS}.", listing.MLSNum);
                return new UploaderResponse { UploadResult = UploadResult.Failure };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "FullUploadListing failed for MLS {MLS}.", listing.MLSNum);
                return this.CreateFailureResponse(ex);
            }
        }

        public abstract Task<LoginResult> Login(Guid companyId);

        public abstract UploaderResponse Logout();

        public abstract Task<UploaderResponse> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        public abstract Task<UploaderResponse> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        public abstract Task<UploaderResponse> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        public abstract Task<UploaderResponse> UpdateImages(ResidentialListingRequest listing, bool logIn = true, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        public abstract Task<UploaderResponse> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        public abstract Task<UploaderResponse> UploadVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> UploadLot(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);

        public abstract Task<UploaderResponse> UpdateLotStatus(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);

        public abstract Task<UploaderResponse> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> UpdateLotPrice(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);

        public abstract Task<UploaderResponse> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn);

        public abstract Task<UploaderResponse> TaxIdRequestCreation(TaxIdBulkUploadListingItem listing, bool logIn = true, CancellationToken cancellationToken = default);

        public abstract Task<UploaderResponse> TaxIdUpdate(ResidentialListingRequest listing, bool logIn = true, CancellationToken cancellationToken = default);

        protected static UploaderResponse CreateSuccessResponse()
    => new UploaderResponse { UploadResult = UploadResult.Success };

        // ---- Shared helpers for derived classes ----

        /// <summary>
        /// Sets the operator (optional) and initializes UploadInformation with requestId + isNewListing.
        /// </summary>
        protected void InitializeUploadSession(Guid requestId, bool isNewListing, string userFullName = null)
        {
            if (!string.IsNullOrWhiteSpace(userFullName))
            {
                this.uploaderClient.UploadInformation.UserFullName = userFullName!;
            }

            this.uploaderClient.InitializeUploadInfo(requestId, isNewListing);
        }

        /// <summary>
        /// If logIn is requested, performs Login(companyId) and pauses briefly to let UI settle.
        /// Returns null to continue the caller flow; override to short-circuit with a response if needed.
        /// </summary>
        protected virtual async Task<UploaderResponse> PerformLoginIfRequired(bool logIn, Guid companyId)
        {
            if (logIn)
            {
                await this.Login(companyId);
                this.sleepService.Sleep(1000);
            }

            return null;
        }

        /// <summary>
        /// Ensure a listing is for the current market; throws if not.
        /// </summary>
        protected void ValidateListingForMarket(ResidentialListingRequest listing)
        {
            ArgumentNullException.ThrowIfNull(listing);

            if (!this.CanUpload(listing))
            {
                throw new InvalidOperationException($"Listing market '{listing.MarketCode}' does not match service market '{this.CurrentMarket}'.");
            }
        }

        /// <summary>
        /// Standardized try/catch wrapper so derived services can focus on core steps.
        /// </summary>
        protected async Task<UploaderResponse> ExecuteWithErrorHandling(
            ResidentialListingRequest listing,
            Func<ResidentialListingRequest, Task<UploaderResponse>> body,
            string operationName)
        {
            ArgumentNullException.ThrowIfNull(listing);

            try
            {
                this.logger.LogInformation("{Operation} for listing request {RequestId}", operationName, listing.ResidentialListingRequestID);
                return await body(listing);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure {Operation} for request {RequestId}", operationName, listing.ResidentialListingRequestID);
                return this.CreateFailureResponse(ex);
            }
        }

        /// <summary>
        /// Waits for any known "autosave complete" marker used across markets.
        /// (HAR/DFW: m_lblInputCompletedMessage; SABOR/CTX: css_InputCompleted; ABOR: InputCompleted_divInputCompleted).
        /// </summary>
        protected void WaitForAutoSave(CancellationToken cancellationToken = default)
        {
            var ids = new[]
            {
                "m_lblInputCompletedMessage",
                "css_InputCompleted",
                "InputCompleted_divInputCompleted",
            };

            foreach (var id in ids)
            {
                try
                {
                    this.uploaderClient.WaitUntilElementExists(By.Id(id), TimeSpan.FromMinutes(5), showAlert: true, cancellationToken);
                    this.sleepService.Sleep(400);
                    return;
                }
                catch
                {
                    // Try next selector
                }
            }
        }

        /// <summary>
        /// Clears cookies and other pre-login prep that every market does.
        /// </summary>
        protected void PrepareForLogin() => this.uploaderClient.DeleteAllCookies();

        /// <summary>
        /// Shared credential retrieval across markets.
        /// </summary>
        protected async Task<Dictionary<LoginCredentials, string>> GetMarketCredentials(Guid companyId)
        {
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            return await LoginCommon.GetMarketCredentials(company, credentialsTask);
        }

        protected UploaderResponse CreateFailureResponse(Exception exception)
            => new UploaderResponse
            {
                UploadResult = UploadResult.Failure,
                UploadInformation = this.uploaderClient.UploadInformation,
            };
    }
}
