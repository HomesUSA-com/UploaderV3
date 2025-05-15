namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.MediaService.Domain.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Core.Services.Common;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.LotListing;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public class AborUploadService : IAborUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<AborUploadService> logger;

        public AborUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<AborUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.Austin;

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public async Task<LoginResult> Login(Guid companyId)
        {
            var marketInfo = this.options.MarketInfo.Abor;
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            this.uploaderClient.DeleteAllCookies();

            var credentials = await LoginCommon.GetMarketCredentials(company, credentialsTask);

            this.uploaderClient.NavigateToUrl(marketInfo.LogoutUrl);
            Thread.Sleep(1000);

            // Connect to the login page
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("loginbtn"));

            this.uploaderClient.WriteTextbox(By.Name("username"), credentials[LoginCredentials.Username]);
            this.uploaderClient.WriteTextbox(By.Name("password"), credentials[LoginCredentials.Password]);
            this.uploaderClient.ClickOnElementById("loginbtn");

            Thread.Sleep(2000);

            // Use the same browser page NOT _blank
            Thread.Sleep(2000);

            if (this.uploaderClient.IsElementVisible(By.Id("NewsDetailDismiss")))
            {
                this.uploaderClient.ClickOnElementById("NewsDetailDismiss");
            }

            if (this.uploaderClient.IsElementVisible(By.LinkText("Skip")))
            {
                this.uploaderClient.ClickOnElement(By.LinkText("Skip"), shouldWait: false, waitTime: 0, isElementOptional: true);
            }

            Thread.Sleep(2000);

            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/Default.aspx?c=AAEAAAD*****AQAAAAAAAAARAQAAAEQAAAAGAgAAAAQ4NzU5DUAGAwAAAAVVLMOwWA0CCw))&f=");
            Thread.Sleep(2000);

            return LoginResult.Logged;
        }

        public UploaderResponse Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Abor.LogoutUrl);
            UploaderResponse response = new UploaderResponse();
            response.UploadResult = UploadResult.Success;
            return response;
        }

        public Task<UploaderResponse> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return EditListing();

            async Task<UploaderResponse> EditListing()
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                if (listing.IsNewListing)
                {
                    this.NavigateToNewPropertyInput();
                }
                else
                {
                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                    this.uploaderClient.ClickOnElementById("toc_InputForm_section_9"); // click in tab Listing Information
                    Thread.Sleep(400);
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListing(logIn);

            async Task<UploaderResponse> UploadListing(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();
                var newLatitude = listing.Latitude;
                var newLongitude = listing.Longitude;
                this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput();
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    }

                    listing.Longitude = newLongitude;
                    listing.Latitude = newLatitude;
                    this.FillListingInformation(listing);
                    this.FillGeneralInformation(listing);
                    this.FillAdditionalInformation(listing as AborListingRequest);
                    this.FillRoomInformation(listing);
                    this.FillDocumentsAndUtilities(listing as AborListingRequest);
                    this.FillGreenEnergyInformation();
                    this.FillFinancialInformation(listing as AborListingRequest);
                    this.FillShowingInformation(listing);
                    this.FillAgentOfficeInformation();
                    this.FillRemarks(listing as AborListingRequest);

                    if (listing.IsNewListing)
                    {
                        await this.UpdateVirtualTour(listing, cancellationToken);
                    }

                    await this.FillMedia(listing, cancellationToken);

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("m_lblInputCompletedMessage"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return PartialUploadListing(logIn);

            async Task<UploaderResponse> PartialUploadListing(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                var newLatitude = listing.Latitude;
                var newLongitude = listing.Longitude;
                this.logger.LogInformation("Partial Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    listing.Longitude = newLongitude;
                    listing.Latitude = newLatitude;
                    this.FillListingInformation(listing, isNotPartialFill: false);
                    this.FillGeneralInformation(listing, isNotPartialFill: false);
                    this.FillFinancialInformation(listing as AborListingRequest);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing as AborListingRequest);

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("m_lblInputCompletedMessage"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingCompletionDate(logIn);

            async Task<UploaderResponse> UpdateListingCompletionDate(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating CompletionDate for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                    this.UpdateYearBuiltDescriptionInGeneralTab(listing);
                    this.UpdatePublicRemarksInRemarksTab(listing as AborListingRequest);

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("m_lblInputCompletedMessage"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateImages(ResidentialListingRequest listing, bool logIn = true, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingImages(logIn);
            async Task<UploaderResponse> UpdateListingImages(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating media for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                    // Enter Manage Photos
                    this.uploaderClient.FindElementById("InputForm_nav-photos").Click();
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);
                    Thread.Sleep(2000);
                    this.uploaderClient.FindElementById("InputForm_photos_actionsButton").Click();
                    this.uploaderClient.FindElementById("InputForm_photos_selectAll").Click();
                    Thread.Sleep(1000);
                    this.uploaderClient.FindElementById("InputForm_photos_actionsButton").Click();
                    this.uploaderClient.FindElementById("InputForm_photos_deleteSelected").Click();
                    this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
                    Thread.Sleep(2000);
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputForm_photos_addPhotoBtn"), cancellationToken);
                    this.uploaderClient.FindElementById("InputForm_photos_addPhotoBtn").Click();

                    await this.ProcessImages(listing, cancellationToken);

                    if (logIn)
                    {
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("InputForm_btnSubmit"), cancellationToken);
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingPrice(logIn);

            async Task<UploaderResponse> UpdateListingPrice(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating the price of the listing {requestId} to {listPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(1)').click()");
                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#accordionActionMenu > button:eq(1)').click()");

                    // Listing Information
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_77"));
                    this.uploaderClient.WriteTextbox(By.Name("Input_77"), listing.ListPrice); // List Price

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("InputCompleted_divInputCompleted"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingStatus(logIn);

            async Task<UploaderResponse> UpdateListingStatus(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Editing the status information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(1)').click()");
                    Thread.Sleep(2000);
                    string buttonText = GetButtonTextForStatus(listing.ListStatus);
                    this.uploaderClient.ExecuteScript($"$('button[data-mtx-track-prop-item=\"{buttonText}\"]').click()");
                    Thread.Sleep(1000);
                    HandleListingStatusAsync(listing);

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("m_lblInputCompletedMessage"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }

            string GetButtonTextForStatus(string status)
            {
                return status switch
                {
                    "Canceled" => "Change to Withdrawn",
                    "Hold" => "Change to Hold",
                    "Closed" => "Change to Closed",
                    "Pending" => "Change to Pending",
                    "ActiveUnderContract" => "Change to Active Under Contract",
                    "Active" => "Change to Active",
                    _ => throw new InvalidOperationException($"Invalid Status '{status}' for Austin Listing with Id '{listing.ResidentialListingID}'"),
                };
            }

            void HandleListingStatusAsync(ResidentialListingRequest listing)
            {
                switch (listing.ListStatus)
                {
                    case "Hold":
                        HandleHoldStatusAsync(listing);
                        break;
                    case "Closed":
                        HandleClosedStatusAsync(listing);
                        break;
                    case "Pending":
                        HandlePendingStatusAsync(listing);
                        break;
                    case "ActiveUnderContract":
                        HandleActiveUnderContractStatusAsync(listing);
                        break;
                }
            }

            void HandleHoldStatusAsync(ResidentialListingRequest listing)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_528"));
                this.uploaderClient.WriteTextbox(By.Name("Input_528"), listing.OffMarketDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_81"), listing.BackOnMarketDate.Value.ToShortDateString());
            }

            void HandleClosedStatusAsync(ResidentialListingRequest listing)
            {
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_85"), listing.ClosedDate.Value.ToShortDateString());

                //// Property Condition at Closing
                this.SetSelect("Input_524", value: "EXCL");
                this.uploaderClient.WriteTextbox(By.Name("Input_84"), listing.SoldPrice?.ToString("F0"));
                this.uploaderClient.WriteTextbox(By.Name("Input_526"), "None");

                this.SelectToggleButton("Input_655", listing.HasContingencyInfo); // Property Sale Contingency
                this.uploaderClient.WriteTextbox(By.Name("Input_517"), listing.SellConcess);
                ////Buyer Financing (max 3)
                this.SetMultipleCheckboxById("Input_525", listing.SoldTerms);
                this.uploaderClient.WriteTextbox(By.Name("Input_519"), "0");
                HandleAgentInfoAsync(listing);
            }

            void HandlePendingStatusAsync(ResidentialListingRequest listing)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_512"));
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_515"), listing.EstClosedDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_81"), listing.ExpiredDate.Value.ToShortDateString());
            }

            void HandleActiveUnderContractStatusAsync(ResidentialListingRequest listing)
            {
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());

                if (listing.ClosedDate.HasValue)
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_512"), listing.ClosedDate.Value.ToShortDateString());
                }

                this.uploaderClient.SetSelect(By.Id("Input_655"), value: listing.HasContingencyInfo.BoolToNumericBool());
                this.uploaderClient.SetMultipleCheckboxById("Input_656", listing.ContingencyInfo, "Other Contingency Type", " ");
                this.uploaderClient.WriteTextbox(By.Name("Input_515"), listing.EstClosedDate.Value.ToShortDateString());
            }

            void HandleAgentInfoAsync(ResidentialListingRequest listing)
            {
                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Name("filter_Input_785"), listing.AgentMarketUniqueId);
                }

                if (!string.IsNullOrEmpty(listing.SecondAgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Name("filter_Input_786"), listing.SecondAgentMarketUniqueId);
                }
            }
        }

        public Task<UploaderResponse> UploadVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListingVirtualTour();

            async Task<UploaderResponse> UploadListingVirtualTour()
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating VirtualTour for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                this.NavigateToQuickEdit(listing.MLSNum);
                this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                await this.UpdateVirtualTour(listing, cancellationToken);

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadOpenHouse();

            async Task<UploaderResponse> UploadOpenHouse()
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Editing the information of Open House for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(2000);
                this.NavigateToQuickEdit(listing.MLSNum);

                Thread.Sleep(2000);
                this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(1)').click()");
                Thread.Sleep(2000);

                // Enter OpenHouse
                string buttonText = "Open Houses Input Form";
                this.uploaderClient.ExecuteScript($"$('button[data-mtx-track-prop-item=\"{buttonText}\"]').click()");
                this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                this.CleanOpenHouse();

                if (listing.EnableOpenHouse)
                {
                    this.AddOpenHouses(listing);
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return EditLotListing(logIn);

            async Task<UploaderResponse> EditLotListing(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Editing the information for the lot {requestId}", listing.InternalLotRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.LotListingRequestID, listing.IsNewListing);
                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                    Thread.Sleep(1000);
                }

                try
                {
                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                    this.uploaderClient.ClickOnElementById("toc_InputForm_section_9"); // click in tab Listing Information
                    Thread.Sleep(400);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lot {requestId}", listing.LotListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UploadLot(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListing(logIn);

            async Task<UploaderResponse> UploadListing(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                var newLatitude = listing.Latitude;
                var newLongitude = listing.Longitude;
                this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.LotListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.LotListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewLotPropertyInput();
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    }

                    listing.Longitude = newLongitude;
                    listing.Latitude = newLatitude;
                    this.FillLotListingInformation(listing);
                    this.FillLotGeneralInformation(listing);
                    this.FillLotAdditionalInformation(listing);
                    this.FillLotDocumentsUtilitiesInformation(listing);
                    this.FillLotFinancialInformation(listing);
                    this.FillLotShowingInformation(listing);
                    this.FillLotAgentOfficeInformation();
                    this.FillLotRemarksInformation(listing);

                    if (listing.IsNewListing)
                    {
                        await this.FillLotVirtualTour(listing, cancellationToken);
                    }

                    await this.FillLotMedia(listing, cancellationToken);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lot {requestId}", listing.LotListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateLotStatus(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateLotListingStatus(logIn);

            async Task<UploaderResponse> UpdateLotListingStatus(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Editing the status information for the lot listing {requestId}", listing.LotListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.LotListingRequestID, isNewListing: false);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(1)').click()");
                    Thread.Sleep(2000);
                    string buttonText = GetButtonTextForStatus(listing.ListStatus);
                    this.uploaderClient.ExecuteScript($"$('button[data-mtx-track-prop-item=\"{buttonText}\"]').click()");
                    Thread.Sleep(1000);
                    HandleListingStatusAsync(listing);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.LotListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }

            string GetButtonTextForStatus(string status)
            {
                return status switch
                {
                    "Canceled" => "Change to Withdrawn",
                    "Hold" => "Change to Hold",
                    "Pending" => "Change to Pending",
                    "ActiveUnderContract" => "Change to Active Under Contract",
                    "Closed" => "Change to Closed",
                    "Active" => "Change to Active",
                    _ => throw new InvalidOperationException($"Invalid Status '{status}' for Austin Listing with Id '{listing.LotListingRequestID}'"),
                };
            }

            void HandleListingStatusAsync(LotListingRequest listing)
            {
                switch (listing.ListStatus)
                {
                    case "Canceled":
                        HandleWithdrawnStatusAsync();
                        break;
                    case "Hold":
                        HandleHoldStatusAsync(listing);
                        break;
                    case "Pending":
                        HandlePendingStatusAsync(listing);
                        break;
                    case "ActiveUnderContract":
                        HandleActiveUnderContractStatusAsync(listing);
                        break;
                    case "Closed":
                        HandleClosedStatusAsync(listing);
                        break;
                }
            }

            void HandleWithdrawnStatusAsync()
            {
                var expirationDate = DateTime.Now.ToShortDateString();
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_529"));
                this.uploaderClient.WriteTextbox(By.Name("Input_529"), expirationDate);
            }

            void HandleHoldStatusAsync(LotListingRequest listing)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_528"));
                this.uploaderClient.WriteTextbox(By.Name("Input_528"), listing.OffMarketDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_81"), listing.BackOnMarketDate.Value.ToShortDateString());
            }

            void HandlePendingStatusAsync(LotListingRequest listing)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_515"), listing.EstClosedDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_81"), listing.ExpiredDate.Value.ToShortDateString());
                this.uploaderClient.SetSelect(By.Id("Input_655"), listing.HasContingencyInfo.BoolToNumericBool());
            }

            void HandleActiveUnderContractStatusAsync(LotListingRequest listing)
            {
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());

                if (listing.ClosedDate.HasValue)
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_512"), listing.ClosedDate.Value.ToShortDateString());
                }

                this.uploaderClient.SetSelect(By.Id("Input_655"), value: listing.HasContingencyInfo.BoolToNumericBool());
                this.uploaderClient.SetMultipleCheckboxById("Input_656", listing.ContingencyInfo);
                this.uploaderClient.WriteTextbox(By.Name("Input_515"), listing.EstClosedDate.Value.ToShortDateString());
            }

            void HandleClosedStatusAsync(LotListingRequest listing)
            {
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Name("Input_94"), listing.PendingDate.Value.ToShortDateString());
                this.uploaderClient.WriteTextbox(By.Name("Input_85"), listing.ClosedDate.Value.ToShortDateString());
                this.uploaderClient.SetSelect(By.Id("Input_524"), value: "EXCL");
                this.uploaderClient.WriteTextbox(By.Name("Input_84"), listing.SoldPrice);
                this.uploaderClient.WriteTextbox(By.Name("Input_526"), "None");
                this.uploaderClient.SetSelect(By.Id("Input_655"), value: listing.HasContingencyInfo.BoolToNumericBool());
                this.uploaderClient.WriteTextbox(By.Name("Input_517"), listing.SellConcess);
                this.uploaderClient.SetMultipleCheckboxById("Input_525", listing.SoldTerms, "Buyer Financing", " ");
                this.uploaderClient.WriteTextbox(By.Name("Input_519"), "0");
                HandleAgentInfoAsync(listing);
            }

            void HandleAgentInfoAsync(LotListingRequest listing)
            {
                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_726"), listing.AgentMarketUniqueId);
                    string js = " document.getElementById('Input_726_Refresh').value='1';RefreshToSamePage(); ";
                    this.uploaderClient.ExecuteScript(@js);
                }

                if (!string.IsNullOrEmpty(listing.SecondAgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_727"), listing.SecondAgentMarketUniqueId);
                    string js = " document.getElementById('Input_727_Refresh').value='1';RefreshToSamePage(); ";
                    this.uploaderClient.ExecuteScript(@js);
                }
            }
        }

        public Task<UploaderResponse> UpdateLotPrice(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingPrice(logIn);

            async Task<UploaderResponse> UpdateListingPrice(bool logIn)
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating the price of the lot {requestId} to {listPrice}.", listing.LotListingRequestID, listing.ListPrice);
                this.uploaderClient.InitializeUploadInfo(listing.LotListingRequestID, listing.IsNewListing);

                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(1)').click()");
                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript("$('#accordionActionMenu > button:eq(1)').click()");

                    // Listing Information
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_77"));
                    this.uploaderClient.WriteTextbox(By.Name("Input_77"), listing.ListPrice); // List Price
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lot {requestId}", listing.LotListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        public Task<UploaderResponse> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingImages();
            async Task<UploaderResponse> UpdateListingImages()
            {
                UploaderResponse response = new UploaderResponse();

                this.logger.LogInformation("Updating media for the listing {requestId}", listing.LotListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.LotListingRequestID, listing.IsNewListing);

                try
                {
                    await this.Login(listing.CompanyId);
                    Thread.Sleep(1000);

                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:first').click()");
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);

                    // Enter Manage Photos
                    this.uploaderClient.FindElementById("InputForm_nav-photos").Click();
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputFormnav-inputFormDetail"), cancellationToken);
                    Thread.Sleep(2000);
                    this.uploaderClient.FindElementById("InputForm_photos_actionsButton").Click();
                    this.uploaderClient.FindElementById("InputForm_photos_selectAll").Click();
                    Thread.Sleep(1000);
                    this.uploaderClient.FindElementById("InputForm_photos_actionsButton").Click();
                    this.uploaderClient.FindElementById("InputForm_photos_deleteSelected").Click();
                    this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
                    Thread.Sleep(2000);
                    this.uploaderClient.WaitUntilElementExists(By.Id("InputForm_photos_addPhotoBtn"), cancellationToken);
                    this.uploaderClient.FindElementById("InputForm_photos_addPhotoBtn").Click();

                    await this.ProcessLotImages(listing, cancellationToken);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.LotListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }
        }

        private async Task UpdateVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_13"); // click in tab Remarks/Tours/Internet
            Thread.Sleep(400);

            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.Austin, cancellationToken);

            if (!virtualTours.Any())
            {
                return;
            }

            var firstVirtualTour = virtualTours.FirstOrDefault();
            if (firstVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Name("Input_325"), firstVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }

            virtualTours = virtualTours.Skip(1).ToList();
            var secondVirtualTour = virtualTours.FirstOrDefault();
            if (secondVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Name("Input_324"), secondVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }
        }

        private void NavigateToNewPropertyInput()
        {
            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/AddEdit");
            this.uploaderClient.ClickOnElement(By.Id("Input_CmdAddBtn")); // "Add button"
            this.uploaderClient.WaitUntilElementIsDisplayed(By.XPath("//button[contains(text(), 'Residential Input Form')]"));
            this.uploaderClient.ClickOnElement(By.XPath("//button[contains(text(), 'Residential Input Form')]"));
            Thread.Sleep(2000);
            this.uploaderClient.ScrollDown(1000);
            this.uploaderClient.ClickOnElement(By.Id("btnSkip")); // Skip to blank input form
            this.uploaderClient.WaitUntilElementIsDisplayed(By.XPath("//input[@value='Start Blank Form']"));
            this.uploaderClient.ClickOnElement(By.XPath("//input[@value='Start Blank Form']"));

            Thread.Sleep(1000);
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/AddEdit");
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_FilterSearch"));

            char[] mlsCharacters = mlsNumber.ToArray();
            foreach (var character in mlsCharacters)
            {
                Thread.Sleep(400);
                this.uploaderClient.FindElementById("Input_FilterSearch").SendKeys(character.ToString());
            }

            this.uploaderClient.FindElementById("Input_FilterSearch").SendKeys(Keys.Tab);
            Thread.Sleep(2000);

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ListResultsView"));
        }

        private void NavigateToEditResidentialForm(string mlsNumber, CancellationToken cancellationToken)
        {
            this.NavigateToQuickEdit(mlsNumber);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ListResultsView"), cancellationToken);
            this.uploaderClient.ExecuteScript("$('#ListResultsView > table > tbody > tr > td > button:eq(0)').click()");
            Thread.Sleep(2000);
        }

        private string GetInputPath(string fieldName, string inputType = "input")
        {
            var parts = fieldName.Split(new string[] { "___" }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                return $"//{inputType}[starts-with(@id, '{parts[0]}') and contains(@id, '{parts[1]}')]";
            }
            else
            {
                return $"//{inputType}[starts-with(@id, '{parts[0]}')]";
            }
        }

        private void SetSelect(string inputId, string value)
        {
            inputId = $"filter_{inputId}";
            string inputXPath = this.GetInputPath(inputId);
            var filterInputElement = this.uploaderClient.FindElement(By.XPath(inputXPath), shouldWait: true);

            try
            {
                if (filterInputElement == null)
                {
                    return;
                }

                filterInputElement.Click();
                Thread.Sleep(400);

                string actualFilterId = filterInputElement.GetAttribute("id");
                string baseId = actualFilterId.Replace("filter_", string.Empty);
                string dropdownListId = "listbox_select_" + baseId;
                string optionXPath = $"//ul[@id='{dropdownListId}']/li[@data-mtrx-listbox-item-value='{value}']";
                var optionElement = this.uploaderClient.FindElement(By.XPath(optionXPath), shouldWait: false);
                if (optionElement == null)
                {
                    return;
                }

                this.uploaderClient.ExecuteScript("arguments[0].scrollIntoView(true);", args: optionElement);
                Thread.Sleep(200);

                optionElement.Click();

                this.uploaderClient.ScrollDown(250);
            }
            catch (Exception ex) when (ex is NoSuchElementException || ex is UnexpectedTagNameException)
            {
                this.uploaderClient.ExecuteScript("document.activeElement.blur();", false);
                this.uploaderClient.ExecuteScript("document.body.click();", false);
                string friendlyErrorMessage = $"Tried to select a non-existing option with value '{value}' for input '{inputId}'.";
                this.logger.LogWarning(ex, friendlyErrorMessage);
                this.uploaderClient.UploadInformation.AddError("SetSelect", inputId, value, friendlyErrorMessage, ex.Message);
            }
        }

        private void SetMultipleCheckboxById(string inputId, string csvValues)
        {
            inputId = $"filter_{inputId}";
            string inputXPath = this.GetInputPath(inputId);
            var filterInputElement = this.uploaderClient.FindElement(By.XPath(inputXPath), shouldWait: true);

            if (filterInputElement == null)
            {
                return;
            }

            Thread.Sleep(200);
            filterInputElement.Click();

            string actualFilterId = filterInputElement.GetAttribute("id");

            string baseId = actualFilterId.Replace("filter_", string.Empty);

            string dropdownListId = "listbox_select_" + baseId;
            var selectedCheckboxes = this.uploaderClient.FindElements(
                                By.XPath($"//ul[@id='{dropdownListId}']//input[@type='checkbox' and (@checked or @aria-checked='true')]"));
            try
            {
                foreach (var checkbox in selectedCheckboxes)
                {
                    checkbox.Click();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("{ex}", ex.Message);
            }

            if (selectedCheckboxes.Count > 0)
            {
                Thread.Sleep(1000);
                filterInputElement.Click();
            }

            if (csvValues != null)
            {
                var splitValues = csvValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var val in splitValues)
                {
                    string trimmedValue = val.Trim();
                    try
                    {
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.XPath($"//ul[@id='{dropdownListId}']/li[starts-with(@data-mtrx-listbox-item-value, '{trimmedValue}')]"));
                        var liElement = this.uploaderClient.FindElement(
                            By.XPath($"//ul[@id='{dropdownListId}']/li[starts-with(@data-mtrx-listbox-item-value, '{trimmedValue}')]"),
                            shouldWait: true);
                        if (liElement != null)
                        {
                            this.uploaderClient.ExecuteScript("arguments[0].scrollIntoView(true);", args: liElement);
                            Thread.Sleep(200);

                            var checkbox = liElement.FindElement(By.CssSelector("input[type='checkbox']"));
                            if (!checkbox.Selected)
                            {
                                checkbox.Click();
                            }
                        }
                    }
                    catch (Exception ex) when (ex is NoSuchElementException || ex is UnexpectedTagNameException)
                    {
                        this.uploaderClient.ExecuteScript("document.activeElement.blur();", false);
                        this.uploaderClient.ExecuteScript("document.body.click();", false);
                    }
                }
            }

            this.uploaderClient.ExecuteScript("document.activeElement.blur();");
            this.uploaderClient.ScrollDown(250);
        }

        private void WriteTextbox(string inputName, string value, string inputType = "input")
        {
            string inputXPath = this.GetInputPath(inputName, inputType);
            try
            {
                var inputElement = this.uploaderClient.FindElement(By.XPath(inputXPath), shouldWait: true);
                if (inputElement == null)
                {
                    return;
                }

                this.uploaderClient.ExecuteScript("arguments[0].value = '';", false, inputElement);
                this.uploaderClient.ExecuteScript("arguments[0].value = arguments[1];", false, inputElement, value);

                this.uploaderClient.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", false, inputElement);
                this.uploaderClient.ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", false, inputElement);
                this.uploaderClient.ExecuteScript("arguments[0].dispatchEvent(new Event('blur', { bubbles: true }));", false, inputElement);
                this.uploaderClient.ExecuteScript("arguments[0].dispatchEvent(new Event('keyup', { bubbles: true }));", false, inputElement);

                this.uploaderClient.ExecuteScript("document.activeElement.blur();", false);
                this.uploaderClient.ExecuteScript("document.body.click();", false);
            }
            catch (NoSuchElementException ex)
            {
                this.logger.LogError("{ex.message}", ex);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error to write in {inputName}: {ex}", inputName, ex.Message);
            }
        }

        private void SelectToggleButton(string inputId, bool shouldSelect)
        {
            string valueToSelect = shouldSelect ? "true" : "false";
            var button = this.uploaderClient.FindElement(By.CssSelector($"button[data-mtx-track-prop-id=\"{inputId}\"][data-mtx-track-prop-val=\"{valueToSelect}\"]"));

            if (button != null && button.GetAttribute("aria-selected") != "true")
            {
                button.Click();
            }
        }

        private void FillListingInformation(ResidentialListingRequest listing, bool isNotPartialFill = true)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_9"); // click in tab Listing Information
            Thread.Sleep(400);

            this.ClickIfNotSelected("InputForm_full-form-view-toggle");

            if (!listing.IsNewListing)
            {
                this.ClickIfNotSelected("InputForm_showSources", false);
            }

            // Listing Information
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_77"));
            this.uploaderClient.WriteTextbox(By.Name("Input_77"), listing.ListPrice); // List Price

            if (isNotPartialFill)
            {
                this.SetSelect("Input_179", "EA"); // List Agreement Type
                this.SetSelect("Input_341", "LIMIT"); // Listing Service
                if (listing.ListDate.HasValue)
                {
                    this.WriteTextbox("Input_83", listing.ListDate.Value.AddYears(1).ToShortDateString()); // Expiration Date
                }
                else
                {
                    this.WriteTextbox("Input_83", DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
                }

                this.SetMultipleCheckboxById("Input_180", "STANDARD"); // Special Listing Conditions

                this.SetSelect("Input_181", "OT"); // List Agreement Document*/
                this.WriteTextbox("Input_183", listing.StreetNum); // Street #
                this.WriteTextbox("Input_185", listing.StreetName); // Street Name
                this.SetSelect("Input_186", listing.StreetType); // Street Type (NM)
                this.WriteTextbox("Input_190", !string.IsNullOrEmpty(listing.UnitNum) ? listing.UnitNum : string.Empty); // Unit # (NM)
                this.SetSelect("Input_191", listing.County); // County
                this.SetSelect("Input_192", listing.CityCode); // City
                this.SetSelect("Input_193", listing.State); // State
                this.SetSelect("Input_399", "US"); // Country
                this.WriteTextbox("Input_194", listing.Zip); // ZIP Code
                this.WriteTextbox("Input_196", listing.Subdivision); // Subdivision
                this.WriteTextbox("Input_199", listing.OtherFees); // Tax Lot
                this.WriteTextbox("Input_201", listing.TaxID); // Parcel ID
                this.SelectToggleButton("Input_202", false); // Additional Parcels Y/N)
                this.uploaderClient.ScrollDown(600);
                this.SetSelect("Input_204", listing.MLSArea); // MLS Area
                this.SetMultipleCheckboxById("Input_343", listing.FemaFloodPlain); // FEMA 100 Yr Flood Plain
                this.SetSelect("Input_206", "N"); // ETJ
            }

            this.WriteTextbox("Input_197", listing.Legal, inputType: "textarea"); // Tax Legal Description

            // School Information
            this.uploaderClient.ScrollDown(1000);
            this.SetSelect("Input_207", listing.SchoolDistrict); // School District
            this.SetSelect("Input_209", listing.SchoolName1); // Elementary School
            this.SetSelect("Input_210", listing.SchoolName2); // Middle School
            this.SetSelect("Input_211", listing.HighSchool); // High School
            this.WriteTextbox("Input_212", listing.SchoolName4); // Elementary Other
            this.WriteTextbox("Input_213", listing.SchoolName5); // Middle or Junior Other
            this.WriteTextbox("Input_214", listing.SchoolName6); // High School Other
        }

        private void FillDocumentsAndUtilities(AborListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_36"); // Documents and Utilities

            this.SetMultipleCheckboxById("Input_271", listing.Disclosures); // Disclosures
            this.SetMultipleCheckboxById("Input_278", listing.UtilitiesDesc); // utilities
            this.SetMultipleCheckboxById("Input_272", listing.Documents); // Documents Available
            this.SetMultipleCheckboxById("Input_273", listing.HeatSystemDesc); // Heating
            this.SetMultipleCheckboxById("Input_274", listing.CoolSystemDesc); // Cooling
            this.SetMultipleCheckboxById("Input_275", listing.GreenWaterConservation); // Water Source
            this.SetMultipleCheckboxById("Input_276", listing.WaterDesc); // Sewer
        }

        private void FillGreenEnergyInformation()
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_110"); // Green Energy

            this.SetMultipleCheckboxById("Input_280", "NONE"); // Green Energy Efficient
            this.SetMultipleCheckboxById("Input_281", "None"); // Green Sustainability
        }

        private void FillGeneralInformation(ResidentialListingRequest listing, bool isNotPartialFill = true)
        {
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_10"); // general

            if (isNotPartialFill)
            {
                this.SetSelect("Input_215", listing.PropSubType); // Property Sub Type (1)
                this.SetSelect("Input_216", "FEESIM"); // Ownership Type
                this.SetMultipleCheckboxById("Input_650", listing.NumStories); // Levels

                this.uploaderClient.WriteTextbox(By.Name("Input_220"), listing.NumBedsMainLevel); // Main Level Beds
                this.uploaderClient.WriteTextbox(By.Name("Input_221"), listing.NumBedsOtherLevels); // # Other Level Beds

                this.SetSelect("Input_219", "BUILDER"); // Year Built Source

                this.uploaderClient.WriteTextbox(By.Name("Input_224"), listing.BathsFull); // Full Baths
                this.uploaderClient.WriteTextbox(By.Name("Input_223"), listing.BathsHalf); // Half Bath
                this.uploaderClient.WriteTextbox(By.Name("Input_659"), listing.NumLivingAreas); // Living
                this.uploaderClient.WriteTextbox(By.Name("Input_660"), listing.NumDiningAreas); // Dining
                this.SetSelect("Input_227", "BUILDER"); // Living Area Source

                this.uploaderClient.WriteTextbox(By.Name("Input_717"), listing.GarageCapacity); // # Garage Spaces
                this.uploaderClient.WriteTextbox(By.Name("Input_229"), listing.GarageCapacity); // Parking Total
                this.SetSelect("Input_342", listing.FacesDesc); // Direction Faces

                this.WriteTextbox("Input_242", listing.LotSize); // Lot Size Acres
                this.WriteTextbox("Input_241", listing.LotDim); // Lot Dimensions (Frontage x Depth)
                this.SetMultipleCheckboxById("Input_225", listing.YearBuiltDesc); // Property condition
                this.WriteTextbox("Input_678", listing.OwnerName); // Builder Name
                this.SetMultipleCheckboxById("Input_230", listing.UnitStyleDesc); // Unit Style (5)
                this.SetMultipleCheckboxById("Input_234", listing.ViewDesc); // View (4)
                this.SetMultipleCheckboxById("Input_232", listing.FloorsDesc); // Flooring (4)
                this.SetSelect("Input_235", listing.DistanceToWaterAccess); // Distance to Water Access
                this.SetMultipleCheckboxById("Input_237", listing.WaterfrontFeatures); // Waterfront Description
                this.SetSelect("Input_238", listing.BodyofWater); // Water Body Name
                this.SetMultipleCheckboxById("Input_236", listing.GarageDesc); // Garage Description (4) / Parking Features
                this.SetMultipleCheckboxById("Input_240", listing.RestrictionsDesc); // Restrictions Description (5)
                this.SetMultipleCheckboxById("Input_239", listing.FoundationDesc); // Foundation (2)
                this.SetMultipleCheckboxById("Input_233", listing.RoofDesc); // Roof
                this.SetMultipleCheckboxById("Input_244", listing.LotDesc); // Lot Description (4)
            }

            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ScrollDown(300);
            this.uploaderClient.WriteTextbox(By.Name("Input_218"), listing.YearBuilt); // Year Built
            this.uploaderClient.ScrollDown(300);
            this.uploaderClient.WriteTextbox(By.Name("Input_226"), listing.SqFtTotal); // Living Area
            this.uploaderClient.ScrollDown(300);

            this.SetMultipleCheckboxById("Input_231", listing.ConstructionDesc); // Construction (5)
        }

        private void FillAdditionalInformation(AborListingRequest listing)
        {
            const string masterBedroom = "MSTRBED";
            const string mainLevelRoom = "MAIN";
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_11");
            var hasPrimaryBedroomOnMain = listing.Rooms.Exists(room => room.RoomType == masterBedroom && room.Level == mainLevelRoom);
            if (hasPrimaryBedroomOnMain)
            {
                listing.InteriorDesc = "MSTDW," + listing.InteriorDesc;
            }

            this.SetMultipleCheckboxById("Input_257", listing.InteriorDesc); // Interior Features (12)
            this.SetMultipleCheckboxById("Input_265", listing.PatioAndPorchFeatures); // Patio and Porch Features
            this.WriteTextbox("Input_259", listing.NumberFireplaces); // # of Fireplaces
            this.SetMultipleCheckboxById("Input_260", listing.FireplaceDesc); // Fireplace Description (3)
            this.SetMultipleCheckboxById("Input_264", listing.ExteriorDesc); // Exterior Features (12)
            this.SetMultipleCheckboxById("Input_258", "None"); // Accessibility Features
            this.SetMultipleCheckboxById("Input_249", "None"); // Horse Amenities
            this.SetMultipleCheckboxById("Input_269", "NONE"); // Other Structures
            this.SetMultipleCheckboxById("Input_256", listing.AppliancesDesc); // Appliances / Equipment (12)
            this.SetMultipleCheckboxById("Input_262", "None"); // Private Pool Features (On Property)
            this.SetMultipleCheckboxById("Input_251", listing.GuestAccommodationsDesc); // Guest Accommodations
            this.SetMultipleCheckboxById("Input_267", listing.WindowCoverings); // Window Features
            this.SetMultipleCheckboxById("Input_266", listing.SecurityDesc); // Security Features
            this.SetMultipleCheckboxById("Input_255", listing.LaundryLocDesc); // Laundry Location (3)
            this.SetMultipleCheckboxById("Input_261", listing.FenceDesc); // Fencing (4)
            this.uploaderClient.WriteTextbox(By.Name("Input_252"), listing.NumGuestBeds); // # Guest Beds
            this.uploaderClient.WriteTextbox(By.Name("Input_253"), listing.NumGuestFullBaths); // # Guest Full Baths
            this.uploaderClient.WriteTextbox(By.Name("Input_254"), listing.NumGuestHalfBaths); // # Guest Half Baths

            this.uploaderClient.ScrollDown(400);
            this.SetMultipleCheckboxById("Input_268", listing.CommonFeatures); // Community Features
        }

        private void FillRoomInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_52"); // Room
            this.uploaderClient.ExecuteScript("document.querySelector('[data-mtrx-uifc-field-name=\"RoomType\"]').scrollIntoView({ behavior: 'smooth', block: 'center' });");
            Thread.Sleep(1000);

            int indexInput = 0;
            int nRoomsToDelete = 0;
            int nRoomsUploaded = 0;

            if (!listing.IsNewListing)
            {
                var deleteButtons = this.uploaderClient.FindElements(By.XPath("//button[contains(@onclick, 'mtrxSubForm.DeleteRow') and contains(@onclick, 'Input_761')]"));

                nRoomsToDelete = deleteButtons.Count - 1;

                for (int i = 0; i < nRoomsToDelete; i++)
                {
                    deleteButtons[i].Click();
                    indexInput++;
                }

                indexInput = Math.Max(0, nRoomsToDelete - 1);

                Thread.Sleep(500);

                this.uploaderClient.ClickOnElement(By.XPath("//button[starts-with(@id, 'addBlankRow_Input_761')]"));
                this.uploaderClient.ExecuteScript("document.activeElement.blur();");
            }

            foreach (var room in listing.Rooms)
            {
                this.SetSelect($"_Input_761___REPEAT{indexInput}_345", room.RoomType);
                this.uploaderClient.ResetImplicitWait();
                this.SetSelect($"_Input_761___REPEAT{indexInput}_346", room.Level);
                this.SetMultipleCheckboxById($"_Input_761___REPEAT{indexInput}_347", room.Features);
                this.WriteTextbox($"_Input_761___REPEAT{indexInput}_648", room.Description, inputType: "textarea");
                nRoomsUploaded++;
                if (nRoomsUploaded < listing.Rooms.Count)
                {
                    this.uploaderClient.ClickOnElement(By.XPath("//button[starts-with(@id, 'addBlankRow_Input_761')]"));
                    this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(400));
                }

                if (indexInput > 0 && nRoomsToDelete > 0)
                {
                    indexInput--;
                }
                else
                {
                    if (nRoomsToDelete > 0)
                    {
                        indexInput = nRoomsToDelete;
                        nRoomsToDelete = 0;
                    }

                    indexInput++;
                }
            }
        }

        private void FillFinancialInformation(AborListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_37"); // Financial
            this.SelectToggleButton("Input_282", listing.HasHoa); // Association YN

            if (listing.HasHoa)
            {
                this.WriteTextbox("Input_283", listing.AssocName); // HOA Name
                this.uploaderClient.WriteTextbox(By.Name("Input_285"), listing.AssocFee, true); // HOA Fee
                this.SetSelect("Input_286", listing.HOA); // Association Requirement
                this.SetSelect("Input_287", listing.AssocFeeFrequency); // HOA Frequency
                this.uploaderClient.WriteTextbox(By.Name("Input_288"), listing.AssocTransferFee, true); // HOA Transfer Fee
                this.SetMultipleCheckboxById("Input_290", listing.AssocFeeIncludes); // HOA Fees Include (5)
            }

            this.SetMultipleCheckboxById("Input_291", listing.FinancingProposed); // Acceptable Financing (5)
            this.WriteTextbox("Input_296", "0"); // Estimated Taxes ($)
            this.WriteTextbox("Input_297", listing.TaxYear); // Tax Year
            this.uploaderClient.ScrollDown();
            this.WriteTextbox("Input_293", "0"); // Tax Assessed Value
            this.WriteTextbox("Input_294", listing.TaxRate); // Tax Rate
            this.WriteTextbox("Input_728", listing.TitleCo); // Preferred Title Company
            this.SetMultipleCheckboxById("Input_295", "None"); // Buyer Incentive
            this.SetMultipleCheckboxById("Input_298", listing.ExemptionsDesc); // Tax Exemptions
            this.SetMultipleCheckboxById("Input_299", "Funding"); // Possession
            this.SelectToggleButton("Input_779", (bool)listing.HasAgentBonus); // Seller Contributions YN
            this.SelectToggleButton("Input_353", false); // Intermediary YN
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_38"); // Showing

            this.SetSelect("Input_301", "VCNT"); // Occupant
            this.SetMultipleCheckboxById("Input_305", listing.Showing); // Showing Requirements
            this.WriteTextbox("Input_302", listing.OwnerName); // Owner Name
            this.SetSelect("Input_651", listing.LockboxTypeDesc ?? "None"); // Lockbox Type
            this.WriteTextbox("Input_312", listing.LockboxLocDesc); // Lockbox Serial Number
            this.SetMultipleCheckboxById("Input_720", "OWN"); // Showing Contact Type
            this.WriteTextbox("Input_310", listing.OwnerName);
            this.WriteTextbox("Input_311", listing.AgentListApptPhone); // Showing Contact Phone
            this.WriteTextbox("Input_406", listing.OtherPhone);  // Showing Service Phone
            this.WriteTextbox("Input_308", listing.LockboxTypeDesc == "SRMRKS" ? "See Remarks" : "NA", inputType: "textarea");  // Lockbox Location
            this.WriteTextbox("Input_313", listing.ShowingInstructions, inputType: "textarea"); // Showing Instructions
        }

        private void FillAgentOfficeInformation()
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_12"); // Agent
        }

        private void FillLotAgentOfficeInformation()
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_84"); // Agent/Office
        }

        private void FillRemarks(AborListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_13"); // Remarks

            this.WriteTextbox("Input_320", listing.Directions, inputType: "textarea"); // Directions
            this.uploaderClient.ScrollDown(200);

            this.UpdatePublicRemarksInRemarksTab(listing);

            if (listing.IsNewListing)
            {
                this.SelectToggleButton("Input_329", true); // Internet
                this.uploaderClient.ScrollDown();
                this.SelectToggleButton("Input_330", true); // Internet Automated Valuation Display
                this.SelectToggleButton("Input_331", true); // Internet Consumer Comment
                this.SelectToggleButton("Input_332", true); // Internet Address Display
                this.SetMultipleCheckboxById("Input_333", "AHS,HAR,LISTHUB,REALTOR"); // Listing Will Appear On (4)
            }
        }

        private async Task FillMedia(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping media upload for existing listing {listingId}", listing.ResidentialListingID);
                return;
            }

            this.uploaderClient.ClickOnElementById("InputForm_nav-photos");
            this.ClickIfNotSelected("InputForm_photo-full-info-toggle");
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("InputForm_photos_addPhotoBtn"));
            this.uploaderClient.ClickOnElementById("InputForm_photos_addPhotoBtn");

            this.uploaderClient.ClickOnElement(By.XPath("//button[contains(@onclick, 'mtrxDnDFileUploader.OpenFilesSelector(event)')]"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("InputForm_photos_certificationText"));
            var element = this.uploaderClient.FindElement(By.Id("InputForm_photos_certificationText"));
            this.uploaderClient.ExecuteScript("arguments[0].scrollTop = arguments[0].scrollHeight;", args: element);

            this.ClickIfNotSelected("InputForm_photos_commonAgreement");
            this.ClickIfNotSelected("InputForm_photos_aiAgreement", desiredState: false);

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("InputForm_photos_agreeBtn"));
            this.uploaderClient.ClickOnElementById("InputForm_photos_agreeBtn");
            Thread.Sleep(1000);

            await this.ProcessImages(listing, cancellationToken);
        }

        private void UpdateYearBuiltDescriptionInGeneralTab(ResidentialListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_10"); // click in tab General

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_218"));
            this.uploaderClient.WriteTextbox(By.Name("Input_218"), listing.YearBuilt); // Year Built
            this.SetMultipleCheckboxById("Input_225", listing.YearBuiltDesc); // Year Built Description
        }

        private void UpdatePublicRemarksInRemarksTab(AborListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_13"); // click in tab Listing Information

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_322"));
            if (!listing.DoNotIncludeInfoInPublicRemarks)
            {
                var remarks = listing.GetPublicRemarks();
                this.WriteTextbox("Input_322", remarks, inputType: "textarea"); // Internet / Remarks / Desc. of Property
                this.WriteTextbox("Input_323", remarks, inputType: "textarea"); // Syndication Remarks
            }

            string baseAgentRemarks = listing.GetAgentRemarksMessage() ?? string.Empty;
            string additionalRemarks = listing.AgentPrivateRemarksAdditional ?? string.Empty;
            var agentRemarks = $"{baseAgentRemarks}. {additionalRemarks}";
            this.WriteTextbox("Input_321", agentRemarks, inputType: "textarea");
        }

        [SuppressMessage("SonarLint", "S2583", Justification = "Ignored due to suspected false positive")]
        private async Task ProcessImages(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            var media = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: this.CurrentMarket, token: cancellationToken, mediaType: MediaType.ListingRequest);
            var imageRow = 0;
            string mediaFolderName = "Husa.Core.Uploader";
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            var captionImageId = string.Empty;

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("images_uploader"), cancellationToken);

            foreach (var image in media)
            {
                await this.mediaRepository.PrepareImage(image, MarketCode.Austin, cancellationToken, folder);

                if (image.IsBrokenLink)
                {
                    continue;
                }

                captionImageId = $"InputForm_photos_drag_img_{imageRow}";

                this.uploaderClient.FindElement(By.Id("images_uploader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                this.WaitForElementAndThenDoAction(
                        By.Id(captionImageId),
                        (element) =>
                        {
                            if (!string.IsNullOrEmpty(image.Caption))
                            {
                                this.uploaderClient.ExecuteScript(script: $"jQuery('#{captionImageId} textarea').val('{image.Caption.Replace("'", "\\'")}');");
                            }
                        });

                imageRow++;

                this.uploaderClient.ScrollDown(200);
                Thread.Sleep(400);
            }
        }

        private void WaitForElementAndThenDoAction(By findBy, Action<IWebElement> action)
        {
            try
            {
                this.uploaderClient.WaitForElementToBeVisible(findBy, TimeSpan.FromSeconds(10));
                var element = this.uploaderClient.FindElement(findBy);
                action(element);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Item not found after 10 seconds.");
            }
        }

        private void CleanOpenHouse(CancellationToken cancellationToken = default)
        {
            var elems = this.uploaderClient.FindElements(By.CssSelector("table[id^=_Input_168__del_REPEAT] a"))?.Count(c => c.Displayed);
            if (elems == null || elems == 0)
            {
                return;
            }

            this.uploaderClient.ScrollDown(3000);
            while (elems > 1)
            {
                this.uploaderClient.ScrollDown();
                var elementId = $"_Input_168__del_REPEAT{elems - 1}_";
                this.uploaderClient.ClickOnElementById(elementId);
                elems--;
                Thread.Sleep(300);
            }

            this.uploaderClient.ScrollDown(5000);
            Thread.Sleep(2000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lbSubmit"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_lbSubmit"));
            Thread.Sleep(2000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lblInputCompletedMessage"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_lbContinueEdit"));
            Thread.Sleep(2000);
        }

        private void AddOpenHouses(ResidentialListingRequest listing)
        {
            string inputXPath = "//div[substring(@id, string-length(@id) - string-length('_accordion') + 1) = '_accordion']";
            var filterInputElement = this.uploaderClient.FindElement(By.XPath(inputXPath), shouldWait: true);
            if (filterInputElement == null)
            {
                return;
            }

            var fullyqualifiedNameField = filterInputElement.GetAttribute("id")?.Replace("_accordion", string.Empty);
            var index = 0;
            Thread.Sleep(1000);
            var sortedOpenHouses = listing.OpenHouse.OrderBy(openHouse => openHouse.Date).ToList();
            var type = "PUBLIC";
            foreach (var openHouse in sortedOpenHouses)
            {
                if (index != 0)
                {
                    this.uploaderClient.ScrollDown();
                    this.uploaderClient.ClickOnElementById(elementId: $"addBlankRow_{fullyqualifiedNameField}");
                    Thread.Sleep(1000);
                }

                // Active Status
                this.SetSelect($"_{fullyqualifiedNameField}__REPEAT{index}_165", value: "ACT");

                // Open House Type
                this.SetSelect($"_{fullyqualifiedNameField}__REPEAT{index}_161", value: type);

                // Refreshments
                this.SetMultipleCheckboxById($"_{fullyqualifiedNameField}__REPEAT{index}_652", openHouse.Refreshments);

                // Date
                this.uploaderClient.WriteTextbox(By.Name($"_{fullyqualifiedNameField}__REPEAT{index}_162"), openHouse.Date);

                // From Time
                this.uploaderClient.ExecuteScript(script: $"jQuery('input[id^=timeBox__{fullyqualifiedNameField}__REPEAT{index}_163]').removeAttr('readonly');");
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? " PM" : " AM";
                var fromTime = openHouse.StartTime.To12Format() + fromTimeTT;
                this.uploaderClient.WriteTextbox(By.Name($"timeBox__{fullyqualifiedNameField}__REPEAT{index}_163"), fromTime);

                // To Time
                this.uploaderClient.ExecuteScript(script: $"jQuery('input[id^=timeBox__{fullyqualifiedNameField}__REPEAT{index}_164]').removeAttr('readonly');");
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? " PM" : " AM";
                var toTime = openHouse.EndTime.To12Format() + endTimeTT;
                this.WriteTextbox($"timeBox__{fullyqualifiedNameField}__REPEAT{index}_164", toTime);

                // Comments
                this.WriteTextbox($"_{fullyqualifiedNameField}__REPEAT{index}_167", openHouse.Comments);

                index++;
            }
        }

        private void ClickIfNotSelected(string elementId, bool desiredState = true)
        {
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id(elementId));
            var element = this.uploaderClient.FindElementById(elementId);

            if (element != null && element.Selected != desiredState)
            {
                element.Click();
            }
        }

        private void NavigateToNewLotPropertyInput()
        {
            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/AddEdit");
            this.uploaderClient.ClickOnElement(By.Id("Input_CmdAddBtn")); // "Add button"
            this.uploaderClient.WaitUntilElementIsDisplayed(By.XPath("//button[contains(text(), 'Land (Lot) Input Form')]"));
            this.uploaderClient.ClickOnElement(By.XPath("//button[contains(text(), 'Land (Lot) Input Form')]"));
            Thread.Sleep(2000);
            this.uploaderClient.ScrollDown(1000);
            this.uploaderClient.ClickOnElement(By.Id("btnSkip")); // Skip to blank input form
            this.uploaderClient.WaitUntilElementIsDisplayed(By.XPath("//input[@value='Start Blank Form']"));
            this.uploaderClient.ClickOnElement(By.XPath("//input[@value='Start Blank Form']"));

            Thread.Sleep(1000);
        }

        private void FillLotListingInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_78"); // click in tab Listing (Lot) Information
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("Input_77"));

            // Listing Information
            this.SetSelect("Input_179", "EA"); // List Agreement Type
            this.SetSelect("Input_341", "LIMIT"); // Listing Service
            this.uploaderClient.WriteTextbox(By.Name("Input_77"), listing.ListPrice); // List Price
            if (listing.ListDate.HasValue)
            {
                this.WriteTextbox("Input_83", listing.ListDate.Value.AddYears(1).ToShortDateString()); // Expiration Date
            }
            else
            {
                this.WriteTextbox("Input_83", DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
            }

            this.SetMultipleCheckboxById("Input_180", "STANDARD"); // Special Listing Conditions
            this.SetSelect("Input_181", "A"); // List Agreement Document*/
            this.SelectToggleButton("Input_545", listing.BuilderRestrictions); // Builder Restrictions
            ////this.WriteTextbox("Input_566", listing.); // Zoning Description

            // Location Information
            this.WriteTextbox("Input_183", listing.StreetNum); // Street #
            this.WriteTextbox("Input_185", listing.StreetName); // Street Name
            this.SetSelect("Input_186", listing.StreetType); // Street Type (NM)
            this.WriteTextbox("Input_190", !string.IsNullOrEmpty(listing.UnitNumber) ? listing.UnitNumber : string.Empty); // Unit # (NM)
            this.SetSelect("Input_191", listing.County); // County
            Thread.Sleep(400);
            this.SetSelect("Input_192", listing.CityCode); // City
            this.SetSelect("Input_193", listing.State); // State
            Thread.Sleep(400);
            this.uploaderClient.ScrollDown(600);
            this.SetSelect("Input_399", "US"); // Country
            this.WriteTextbox("Input_194", listing.Zip); // ZIP Code
            this.WriteTextbox("Input_196", listing.Subdivision); // Subdivision
            this.WriteTextbox("Input_197", listing.LegalDescription); // Tax Legal Description
            this.WriteTextbox("Input_199", listing.OtherFees); // Tax Lot
            this.WriteTextbox("Input_201", listing.TaxId); // Parcel ID
            this.SelectToggleButton("Input_202", false); // Additional Parcels Y/N)
            Thread.Sleep(400);
            this.uploaderClient.ScrollDown(1000);
            this.SetSelect("Input_204", listing.MlsArea); // MLS Area
            this.SetMultipleCheckboxById("Input_343", listing.FemaFloodPlain); // FEMA Flood Plain
            this.SetSelect("Input_206", "N"); // ETJ

            // Map
            this.SetLotLongitudeAndLatitudeValues(listing);
            Thread.Sleep(400);
            this.uploaderClient.ScrollDown(1500);

            // School Information
            this.WriteTextbox("Input_207", listing.SchoolDistrict); // School District
            this.WriteTextbox("Input_209", listing.SchoolName1); // School District/Elementary A
            this.WriteTextbox("Input_212", listing.SchoolName4); // Elementary Other
            this.WriteTextbox("Input_210", listing.SchoolName2); // School District/Middle / Intermediate School
            this.WriteTextbox("Input_213", listing.SchoolName5); // Middle or Junior Other
            this.WriteTextbox("Input_211", listing.HighSchool); // School District/9 Grade / High School
            this.WriteTextbox("Input_214", listing.SchoolName6); // High School Other
        }

        private void FillLotGeneralInformation(LotListingRequest listing)
        {
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ClickOnElementById("toc_InputForm_section_79"); // general

            this.SetSelect("Input_461", listing.PropertySubType); // Property Sub Type (1)
            this.SelectToggleButton("Input_445", listing.SurfaceWater); // SurfaceWater YN
            this.SetSelect("Input_235", listing.DistanceToWaterAccess); // Distance To Water Access
            this.WriteTextbox("Input_242", listing.LotSize); // Lot Size Acres
            this.WriteTextbox("Input_241", listing.LotDimension); // Lot Dimensions (Frontage x Depth)
            this.WriteTextbox("Input_352", listing.AlsoListedAs?.ToString()); // Also Listed As (Enter ML #)
            this.SetMultipleCheckboxById("Input_442", listing.TypeOfHomeAllowed); // Type Of Home Allowed
            this.SetMultipleCheckboxById("Input_439", listing.SoilType); // Soil Type
            this.SetMultipleCheckboxById("Input_441", listing.MineralsFeatures); // Minerals
            this.SetMultipleCheckboxById("Input_244", listing.LotDescription); // Lot Features (max 66)
            this.SelectToggleButton("Input_462", listing.LiveStock); // LiveStock
            this.WriteTextbox("Input_433", listing.NumberOfPonds?.ToString()); // Ponds
            this.SetMultipleCheckboxById("Input_237", listing.WaterfrontFeatures); // Waterfront Features
            this.SetSelect("Input_238", listing.WaterBodyName); // Water Body Name
            this.SetMultipleCheckboxById("Input_261", listing.Fencing); // Fencing
            this.WriteTextbox("Input_434", listing.NumberOfWells?.ToString()); // Wells
            this.SetMultipleCheckboxById("Input_234", listing.View); // View (4)
            this.SetMultipleCheckboxById("Input_240", listing.RestrictionsDescription); // Restrictions Description (5)
            this.SelectToggleButton("Input_446", listing.CommercialAllowed); // Commercial Allowed YN
            this.SetMultipleCheckboxById("Input_249", listing.HorseAmenities); // Horse Amenities
            this.SetMultipleCheckboxById("Input_443", listing.RoadSurface); // Road Surface
        }

        private void FillLotAdditionalInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.XPath("//li[@id='toc_InputForm_section_80']")); // Tab Aditional

            this.SetMultipleCheckboxById("Input_713", listing.PropCondition); // Prop Condition
            this.SetMultipleCheckboxById("Input_269", listing.OtherStructures); // Other Structures
            this.SetMultipleCheckboxById("Input_264", listing.ExteriorFeatures); // Exterior Features
            this.SetMultipleCheckboxById("Input_268", listing.NeighborhoodAmenities); // Neighborhood Amenities
        }

        private void FillLotDocumentsUtilitiesInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.XPath("//li[@id='toc_InputForm_section_81']")); // Tab Documents & Utitlities

            this.SetMultipleCheckboxById("Input_271", listing.Disclosures); // Disclosures
            this.SetMultipleCheckboxById("Input_272", listing.DocumentsAvailable); // Documents Available
            this.SetMultipleCheckboxById("Input_278", listing.UtilitiesDescription); // Utilities Description
            this.SelectToggleButton("Input_277", listing.GroundWaterConservDistric); // Ground Water ConservDistric YN
            this.SetMultipleCheckboxById("Input_275", listing.WaterSource); // Water Source
            this.SetMultipleCheckboxById("Input_276", listing.WaterSewer); // Water Sewer
        }

        private void FillLotFinancialInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.XPath("//li[@id='toc_InputForm_section_82']")); // Financial

            this.SelectToggleButton("Input_282", listing.HasHoa); // Association YN

            if (listing.HasHoa)
            {
                this.WriteTextbox("Input_283", listing.HoaName); // HOA Name
                this.WriteTextbox("Input_285", listing.HoaFee); // HOA Fee
                this.SetSelect("Input_286", listing.HOARequirement); // Association Requirement
                this.SetSelect("Input_287", listing.BillingFrequency); // HOA Frequency
                this.SetMultipleCheckboxById("Input_290", listing.HoaIncludes); // HOA Fees Include (5)
            }

            this.SetMultipleCheckboxById("Input_291", listing.AcceptableFinancing); // Acceptable Financing (5)
            this.WriteTextbox("Input_296", !string.IsNullOrEmpty(listing.EstimatedTax?.ToString()) ? listing.EstimatedTax : "0"); // Estimated Taxes ($)
            this.WriteTextbox("Input_297", listing.TaxYear); // Tax Year
            this.WriteTextbox("Input_293", !string.IsNullOrEmpty(listing.TaxAssesedValue?.ToString()) ? listing.TaxAssesedValue : "0"); // Tax Assessed Value
            this.WriteTextbox("Input_294", listing.TaxRate); // Tax Rate
            this.SetMultipleCheckboxById("Input_298", listing.TaxExemptions); // Tax Exemptions
            this.SetMultipleCheckboxById("Input_295", "None"); // Buyer Incentive
            this.SetSelect("Input_554", listing.LandTitleEvidence); // Land Title Evidence
            this.uploaderClient.ScrollDown(400);
            this.WriteTextbox("Input_728", listing.PreferredTitleCompany); // Preferred Title Company
            this.SetMultipleCheckboxById("Input_299", "Funding"); // Possession
            this.SelectToggleButton("Input_779", listing.HasAgentBonus);
        }

        private void FillLotShowingInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.XPath("//li[@id='toc_InputForm_section_83']")); // Showing

            this.WriteTextbox("Input_302", listing.OwnerName); // Owner Name
            this.SetMultipleCheckboxById("Input_305", listing.ShowingRequirements); // Showing Requirements
            this.SetMultipleCheckboxById("Input_720", !string.IsNullOrEmpty(listing.ShowingContactType) ? listing.ShowingContactType : "OWN"); // Showing Contact Type
            this.WriteTextbox("Input_310", listing.OwnerName);
            this.WriteTextbox("Input_311", listing.AgentListApptPhone); // Showing Contact Phone
            this.WriteTextbox("Input_406", listing.ShowingServicePhone);  // Showing Service Phone
            this.WriteTextbox("Input_313", listing.ShowingInstructions); // Showing Instructions
        }

        private void FillLotRemarksInformation(LotListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.XPath("//li[@id='toc_InputForm_section_85']")); // Remarks/Tours/Internet

            this.WriteTextbox("Input_320", listing.Directions); // Directions
            this.uploaderClient.ScrollDown(200);

            this.UpdateLotPublicRemarksInRemarksTab(listing);
        }

        private async Task FillLotVirtualTour(LotListingRequest listing, CancellationToken cancellationToken = default)
        {
            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.LotListingRequestID, market: MarketCode.Austin, cancellationToken);

            if (virtualTours.Any())
            {
                var firstVirtualTour = virtualTours.FirstOrDefault();
                if (firstVirtualTour != null)
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_325"), firstVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
                }

                virtualTours = virtualTours.Skip(1).ToList();
                var secondVirtualTour = virtualTours.FirstOrDefault();
                if (secondVirtualTour != null)
                {
                    this.uploaderClient.WriteTextbox(By.Name("Input_324"), secondVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
                }
            }
        }

        private void SetLotLongitudeAndLatitudeValues(LotListingRequest listing)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            if (listing.UpdateGeocodes)
            {
                this.uploaderClient.WriteTextbox(By.Name("INPUT__146"), value: listing.Latitude); // Latitude
                this.uploaderClient.WriteTextbox(By.Name("INPUT__168"), value: listing.Longitude); // Longitude
            }
            else
            {
                var getLatLongFromAddress = "Get Lat/Long from address";
                if (this.uploaderClient.FindElements(By.LinkText(getLatLongFromAddress))?.Any() == true)
                {
                    this.uploaderClient.ClickOnElement(By.LinkText(getLatLongFromAddress));
                    Thread.Sleep(1000);
                }
            }
        }

        private void UpdateLotPublicRemarksInRemarksTab(LotListingRequest listing)
        {
            var remarks = listing.GetPublicRemarks();
            this.WriteTextbox("Input_321", listing.GetAgentRemarksMessage());
            this.uploaderClient.ScrollDown(2000);
            this.WriteTextbox("Input_322", remarks); // Internet / Remarks / Desc. of Property
            this.WriteTextbox("Input_323", remarks); // Syndication Remarks
        }

        private async Task FillLotMedia(LotListingRequest listing, CancellationToken cancellationToken)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping media upload for existing lot {lotId}", listing.LotListingRequestID);
                return;
            }

            // Enter Manage Photos
            this.uploaderClient.ClickOnElement(By.XPath("//button[contains(text(), 'Save as Incomplete')]"));
            Thread.Sleep(3000);

            this.uploaderClient.ClickOnElement(By.XPath("//button[contains(text(), 'Continue Editing')]"));
            Thread.Sleep(3000);

            this.uploaderClient.FindElementById("InputForm_nav-photos").Click();
            Thread.Sleep(1000);

            await this.ProcessLotImages(listing, cancellationToken);
        }

        [SuppressMessage("SonarLint", "S2583", Justification = "Ignored due to suspected false positive")]
        private async Task ProcessLotImages(LotListingRequest listing, CancellationToken cancellationToken)
        {
            var media = await this.mediaRepository.GetListingImages(listing.LotListingRequestID, market: this.CurrentMarket, token: cancellationToken, mediaType: MediaType.ListingRequest);
            var imageRow = 0;
            string mediaFolderName = "Husa.Core.Uploader";
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            var captionImageId = string.Empty;

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("images_uploader"), cancellationToken);

            foreach (var image in media)
            {
                await this.mediaRepository.PrepareImage(image, MarketCode.Austin, cancellationToken, folder);

                if (image.IsBrokenLink)
                {
                    continue;
                }

                captionImageId = $"InputForm_photos_drag_img_{imageRow}";

                this.uploaderClient.FindElement(By.Id("images_uploader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                this.WaitForElementAndThenDoAction(
                        By.Id(captionImageId),
                        (element) =>
                        {
                            if (!string.IsNullOrEmpty(image.Caption))
                            {
                                this.uploaderClient.ExecuteScript(script: $"jQuery('#{captionImageId} textarea').val('{image.Caption.Replace("'", "\\'")}');");
                            }
                        });

                imageRow++;

                this.uploaderClient.ScrollDown(200);
                Thread.Sleep(400);
            }
        }
    }
}
