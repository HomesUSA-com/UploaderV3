namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums.Domain;
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

    public class HarUploadService : IHarUploadService
    {
        private const string LandingPageURL = "https://www.har.com/moa_mls/goMatrix";
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<HarUploadService> logger;

        public HarUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<HarUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.Houston;

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public async Task<LoginResult> Login(Guid companyId)
        {
            var marketInfo = this.options.MarketInfo.Har;
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            this.uploaderClient.DeleteAllCookies();

            var credentials = await LoginCommon.GetMarketCredentials(company, credentialsTask);

            this.uploaderClient.NavigateToUrl(marketInfo.LogoutUrl);
            Thread.Sleep(2000);

            this.uploaderClient.ExecuteScript(" jQuery('#ketch-consent-banner').find('button:first').click(); ");

            // Connect to the login page
            var loginButtonId = "login_btn";
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id(loginButtonId));

            this.uploaderClient.WriteTextbox(By.Id("username"), credentials[LoginCredentials.Username]);
            this.uploaderClient.WriteTextbox(By.Id("password"), credentials[LoginCredentials.Password]);
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript($"jQuery('#{loginButtonId}').click();");

            Thread.Sleep(5000);
            try
            {
                this.uploaderClient.ClickOnElementById("back-to-nexturl");
                Thread.Sleep(3000);
            }
            catch
            {
                this.logger.LogInformation("The redirect popup was not displayed in the login screen.");
            }

            this.uploaderClient.NavigateToUrl(LandingPageURL);
            Thread.Sleep(3000);

            return LoginResult.Logged;
        }

        public UploaderResponse Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Har.LogoutUrl);
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

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                if (listing.IsNewListing)
                {
                    this.NavigateToNewPropertyInput(listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>());
                }
                else
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
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

                    var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();

                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput(housingType);
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    }

                    listing.Longitude = newLongitude;
                    listing.Latitude = newLatitude;
                    this.FillListingInformation(listing);
                    this.FillMapInformation(listing as HarListingRequest);
                    this.FillPropertyInformation(listing as HarListingRequest, housingType);
                    this.FillRoomInformation(listing);
                    this.FillFinancialInformation(listing as HarListingRequest, housingType);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing as HarListingRequest, housingType);

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

            return UploadListing(logIn);

            async Task<UploaderResponse> UploadListing(bool logIn)
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

                    var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();

                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    listing.Longitude = newLongitude;
                    listing.Latitude = newLatitude;
                    this.FillListingInformation(listing, isNotPartialFill: false);
                    this.FillMapInformation(listing as HarListingRequest);
                    this.FillPropertyInformation(listing as HarListingRequest, housingType, isNotPartialFill: false);
                    this.FillFinancialInformation(listing as HarListingRequest, housingType);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing as HarListingRequest, housingType);

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

                    var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    this.GoToPropertyInformationTab();
                    this.UpdateYearBuiltDescription(listing);
                    this.FillCompletionDate(listing);

                    this.GoToRemarksTab(housingType);
                    this.UpdatePublicRemarksInRemarksTab(listing);

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

            return UpdateListingImages();
            async Task<UploaderResponse> UpdateListingImages()
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

                    // Enter Manage Photos
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Photos"), cancellationToken);
                    this.uploaderClient.ClickOnElement(By.LinkText("Manage Photos"));
                    this.DeleteAllImages();
                    await this.ProcessImages(listing, cancellationToken);
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

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Price Change"), cancellationToken);
                    this.uploaderClient.ClickOnElement(By.LinkText("Price Change"));
                    this.uploaderClient.WriteTextbox(By.Id("Input_9"), listing.ListPrice); // List Price

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
                    Thread.Sleep(1000);

                    switch (listing.ListStatus)
                    {
                        case "CLOSD":
                            HandleClosedStatus(listing);
                            break;
                        case "PEND":
                            HandlePendingStatus(listing);
                            break;
                        case "TERM":
                            HandleTerminatedStatus(cancellationToken);
                            break;
                        case "WITH":
                            HandleWithdrawnStatus(listing);
                            break;
                        case "OP":
                            HandleOptionPendingStatus(listing, cancellationToken);
                            break;
                        case "PSHO":
                            HandlePendingContinueToShowStatus(listing);
                            break;
                        case "EXP":
                            HandleExpiredStatus(listing, cancellationToken);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid Status '{listing.ListStatus}' for Houston Listing with Id '{listing.ResidentialListingID}'");
                    }

                    if (autoSave)
                    {
                        this.uploaderClient.WaitUntilElementExists(By.Id("m_lblInputCompletedMessage"), new TimeSpan(0, 5, 0), true, cancellationToken);
                        Thread.Sleep(400);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the listing {requestId}", listing.ResidentialListingRequestID);
                    response.UploadResult = UploadResult.Failure;
                    response.UploadInformation = this.uploaderClient.UploadInformation;
                    return response;
                }

                response.UploadResult = UploadResult.Success;
                return response;
            }

            void HandleClosedStatus(ResidentialListingRequest listing)
            {
                var buttonText = "Change to Sold";
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);

                this.uploaderClient.WriteTextbox(By.Id("Input_74"), listing.SoldPrice); // Sale Price

                if (listing.ContractDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_321"), listing.ContractDate.Value.ToShortDateString()); // Pending Date
                }

                if (listing.ClosedDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_120"), listing.ClosedDate.Value.ToShortDateString()); // Closed Date
                }

                this.uploaderClient.SetSelect(By.Id("Input_119"), "0"); // Coop Sale

                this.uploaderClient.WriteTextbox(By.Id("Input_121"), listing.SellerBuyerCost); // Seller Pd Buyer Clsg Costs
                this.uploaderClient.WriteTextbox(By.Id("Input_123"), listing.RepairsPaidBySeller); // Repair Paid Seller

                this.uploaderClient.SetSelect(By.Id("Input_122"), listing.TitlePaidBy); // Title Paid By
                this.uploaderClient.SetSelect(By.Id("Input_310"), listing.HasBuyerAgent ? "Y" : "N"); // Did Selling Agent Represent Buyer
                this.uploaderClient.SetSelect(By.Id("Input_525"), listing.SoldTerms); // Sold Terms

                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_342"), listing.AgentMarketUniqueId); // Selling Agent MLSID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ");
                }

                if (!string.IsNullOrEmpty(listing.SellTeamID) && this.uploaderClient.IsElementPresent(By.Id("Input_614")))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_614"), listing.SellTeamID); // Selling Team ID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_614_Refresh').value='1';RefreshToSamePage(); ");
                }

                if (!string.IsNullOrEmpty(listing.SellingAgent2ID))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_344"), listing.SellingAgent2ID); // Co Selling Associate MLSID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_344_Refresh').value='1';RefreshToSamePage(); ");
                }

                this.uploaderClient.ScrollDown();
                if (!string.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                {
                    this.uploaderClient.SetSelect(By.Id("Input_124"), "0"); // Buyer Represented by NONMLS Licensed Agent
                    this.uploaderClient.WriteTextbox(By.Id("Input_125"), listing.SellingAgentLicenseNum); // TREC License Number
                }
            }

            void HandlePendingStatus(ResidentialListingRequest listing)
            {
                var buttonText = "Change to Pending";
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);

                if (listing.ContractDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_321"), listing.ContractDate.Value.ToShortDateString()); // Pending Date
                }

                if (listing.EstClosedDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_311"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date
                }

                this.uploaderClient.SetSelect(By.Id("Input_310"), listing.HasBuyerAgent ? "Y" : "N"); // Did Selling Agent Represent Buyer

                this.uploaderClient.ScrollDown();

                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_342"), listing.AgentMarketUniqueId); // Selling Agent MLSID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ");
                }

                if (!string.IsNullOrEmpty(listing.SellTeamID))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_614"), listing.SellTeamID); // Selling Team MLSID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_614_Refresh').value='1';RefreshToSamePage(); ");
                }

                this.uploaderClient.ScrollDown();
                if (!string.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                {
                    this.uploaderClient.SetSelect(By.Id("Input_528"), "0"); // Buyer Represented by NONMLS Licensed Agent
                    this.uploaderClient.WriteTextbox(By.Id("Input_131"), listing.SellingAgentLicenseNum); // TREC License Number
                }
            }

            void HandleTerminatedStatus(CancellationToken cancellationToken)
            {
                var buttonText = "Change to Terminated";
                var currentDate = DateTime.Today.ToString("MM/dd/yyyy");
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Id("Input_522"), currentDate); // terminated date
            }

            void HandleWithdrawnStatus(ResidentialListingRequest listing)
            {
                var buttonText = "Change to Withdrawn";
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Id("Input_113"), listing.OffMarketDate.Value.ToShortDateString()); // Withdrawn Date
            }

            void HandleOptionPendingStatus(ResidentialListingRequest listing, CancellationToken cancellationToken)
            {
                var buttonText = "Change to Option Pending";
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Id("Input_83"), listing.ContractDate.Value.ToShortDateString()); // Pending Date
                this.uploaderClient.WriteTextbox(By.Id("Input_128"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date
                this.uploaderClient.WriteTextbox(By.Id("Input_129"), listing.ExpiredDate.Value.ToShortDateString()); // Option End Date
                this.uploaderClient.SetSelect(By.Id("Input_132"), listing.HasContingencyInfo.BoolToNumericBool()); // Contingent on Sale of Other Property
                this.uploaderClient.SetSelect(By.Id("Input_310"), string.IsNullOrEmpty(listing.AgentMarketUniqueId) ? "N" : "Y"); // Did Selling Agent Represent Buyer

                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_342"), listing.AgentMarketUniqueId); // Selling Associate MLSID
                }
            }

            void HandlePendingContinueToShowStatus(ResidentialListingRequest listing)
            {
                var buttonText = "Change to Pending Continue to Show";
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);

                if (listing.ContractDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_83"), listing.ContractDate.Value.ToShortDateString()); // Pending Date
                }

                if (listing.EstClosedDate != null)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_128"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date
                }

                this.uploaderClient.SetSelect(By.Id("Input_310"), listing.HasBuyerAgent ? "Y" : "N"); // Did Selling Agent Represent Buyer
                this.uploaderClient.SetSelect(By.Id("Input_132"), listing.HasContingencyInfo.BoolToNumericBool()); // Contingent on Sale of Other Property

                this.uploaderClient.ScrollDown();

                if (!string.IsNullOrEmpty(listing.AgentMarketUniqueId))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_342"), listing.AgentMarketUniqueId); // Selling Agent MLSID
                    this.uploaderClient.ExecuteScript(" document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ");
                }

                this.uploaderClient.ScrollDown();
                if (!string.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                {
                    this.uploaderClient.SetSelect(By.Id("Input_130"), "0"); // Buyer Represented by NONMLS Licensed Agent
                    this.uploaderClient.WriteTextbox(By.Id("Input_131"), listing.SellingAgentLicenseNum); // TREC License Number
                }
            }

            void HandleExpiredStatus(ResidentialListingRequest listing, CancellationToken cancellationToken)
            {
                var buttonText = "Change Expiration Date";
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Id("Input_8"), listing.ExpiredDate.Value.ToShortDateString()); // Expiration Date
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
                this.logger.LogInformation("Updating VirtualTour for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);

                this.NavigateToQuickEdit(listing.MLSNum);

                this.GoToManageTourLinks();

                await this.UpdateVirtualTourLinks(listing, cancellationToken);

                UploaderResponse response = new UploaderResponse();
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
                Thread.Sleep(5000);
                this.NavigateToQuickEdit(listing.MLSNum);

                Thread.Sleep(400);

                // Enter OpenHouse
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_dlInputList_ctl02_m_btnSelect"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl02_m_btnSelect"));
                Thread.Sleep(3000);

                this.CleanOpenHouse(cancellationToken);

                if (listing.EnableOpenHouse)
                {
                    this.AddOpenHouses(listing);
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
                        this.NavigateToNewPropertyInput();
                    }

                    this.FillLotListingInformation(listing);
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
            throw new NotImplementedException();
        }

        public Task<UploaderResponse> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UploaderResponse> UpdateLotPrice(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            throw new NotImplementedException();
        }

        public Task<UploaderResponse> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn)
        {
            throw new NotImplementedException();
        }

        public void FillListDate(string listStatus)
        {
            var listDate = GetNewListingDate(listStatus);
            if (listDate.HasValue)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_183"), listDate.Value.ToShortDateString());  // List Date
            }

            DateTime? GetNewListingDate(string listStatus)
            {
                switch (listStatus.ToEnumFromEnumMember<MarketStatuses>())
                {
                    case MarketStatuses.Active:
                        return DateTime.Now;
                    case MarketStatuses.Pending:
                        return DateTime.Now.AddDays((int)ListingDaysOffset.PENDING);
                    case MarketStatuses.OptionPending:
                        return DateTime.Now.AddDays((int)ListingDaysOffset.PENDING);
                    case MarketStatuses.PendingContinueToShow:
                        return DateTime.Now.AddDays((int)ListingDaysOffset.PENDING);
                    case MarketStatuses.Terminated:
                    case MarketStatuses.Expired:
                    case MarketStatuses.Sold:
                        return DateTime.Now.AddDays((int)ListingDaysOffset.SOLD);
                    default:
                        return null;
                }
            }
        }

        private async Task UpdateVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.Houston, cancellationToken);
            var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();
            if (!virtualTours.Any())
            {
                return;
            }

            this.GoToRemarksTab(housingType);

            var firstVirtualTour = virtualTours.FirstOrDefault();
            if (firstVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_341"), firstVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }

            virtualTours = virtualTours.Skip(1).ToList();
            var secondVirtualTour = virtualTours.FirstOrDefault();
            if (secondVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_532"), secondVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }
        }

        private void NavigateToNewPropertyInput(HousingType? housingType = null)
        {
            this.uploaderClient.NavigateToUrl("https://matrix.harmls.com/Matrix/AddEdit/MatrixAddEdit");
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Add new"));
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));

            switch (housingType)
            {
                case HousingType.SingleFamily:
                    WaitAndClick("Single-Family Add/Edit");
                    break;
                case HousingType.CountryHomesAcreage:
                    WaitAndClick("Country Homes/Acreage Add/Edit");
                    break;
                case HousingType.TownhouseCondo:
                    WaitAndClick("Townhouse/Condo Add/Edit");
                    break;
                case HousingType.MultiFamily:
                    WaitAndClick("Multi-Family Add/Edit");
                    break;
                default:
                    WaitAndClick("Lots Add/Edit");
                    break;
            }

            WaitAndClick("Start with a blank Listing");

            void WaitAndClick(string text)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(text));
                this.uploaderClient.ClickOnElement(By.LinkText(text));
            }

            Thread.Sleep(1000);
        }

        private async Task UpdateVirtualTourLinks(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.Houston, cancellationToken);

            if (!virtualTours.Any())
            {
                return;
            }

            var firstVirtualTour = virtualTours.FirstOrDefault();
            if (firstVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_341"), firstVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }

            virtualTours = virtualTours.Skip(1).ToList();
            var secondVirtualTour = virtualTours.FirstOrDefault();
            if (secondVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_532"), secondVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl("https://matrix.harmls.com/Matrix/AddEdit");
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Edit existing"));
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbEditItem"));
            this.uploaderClient.WriteTextbox(By.Id("m_txtSourceCommonID"), value: mlsNumber);
            Thread.Sleep(500);
            this.uploaderClient.ClickOnElement(By.Id("m_lbEdit"));
            Thread.Sleep(1000);
        }

        private void NavigateToEditResidentialForm(string mlsNumber, CancellationToken cancellationToken = default)
        {
            this.NavigateToQuickEdit(mlsNumber);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_dlInputList_ctl00_m_btnSelect"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl00_m_btnSelect"));
            Thread.Sleep(1000);
        }

        private void FillListingInformation(ResidentialListingRequest listing, bool isNotPartialFill = true)
        {
            this.GoToTab("Listing Information");
            SetListPrice(listing.ListPrice);
            SetTaxID(listing.TaxID);

            if (isNotPartialFill)
            {
                SetListingType("EXAGY");
                SetListingDates(listing);
                SetLeaseOption();
                SetPropertyType();
                SetAddressDetails(listing);
                SetLegalDetails(listing);
                SetMasterPlannedCommunity(listing);
                SetLegalSubdivision(listing.LegalSubdivision, listing.Zip);
                SetKeyMap(listing.MapscoMapCoord);
            }

            void SetListPrice(int listPrice)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_182"), listPrice); // List Price
            }

            void SetTaxID(string taxID)
            {
                if (!string.IsNullOrEmpty(taxID) && taxID != "0" && !taxID.Contains("-0000"))
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_174"), taxID); // Tax ID #
                }
                else
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_174"), "N/A"); // Tax ID #
                }
            }

            void SetListingType(string listType)
            {
                this.uploaderClient.SetSelect(By.Id("Input_181"), listType); // List Type
            }

            void SetListingDates(ResidentialListingRequest listing)
            {
                if (listing.IsNewListing)
                {
                    this.FillListDate(listing.ListStatus);

                    var expirationDate = listing.ExpiredDate.HasValue ? listing.ExpiredDate.Value : (listing.SysCreatedOn ?? DateTime.Today).AddYears(1);
                    this.uploaderClient.WriteTextbox(By.Id("Input_184"), expirationDate.ToShortDateString()); // Expiration Date
                }
            }

            void SetLeaseOption()
            {
                this.uploaderClient.SetSelect(By.Id("Input_185"), "0"); // Also For Lease
            }

            void SetPropertyType()
            {
                if (this.uploaderClient.FindElements(By.Id("Input_350")).Any())
                {
                    this.uploaderClient.SetSelect(By.Id("Input_350"), "TOWN"); // Townhouse or Condo
                }
                else
                {
                    this.uploaderClient.SetSelect(By.Id("Input_186"), "0"); // Priced at Lot Value Only
                }
            }

            void SetAddressDetails(ResidentialListingRequest listing)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_156"), listing.StreetNum); // Street Number
                this.uploaderClient.WriteTextbox(By.Id("Input_158"), listing.StreetName); // Street Name
                this.uploaderClient.WriteTextbox(By.Id("Input_160"), listing.UnitNum); // Unit #
                this.uploaderClient.SetSelect(By.Id("Input_159"), listing.StreetType, isElementOptional: true); // Street Type

                if (!string.IsNullOrEmpty(listing.City))
                {
                    this.uploaderClient.FillFieldSingleOption("Input_161", listing.City);
                }

                this.uploaderClient.SetSelect(By.Id("Input_162"), listing.StateCode); // State
                this.uploaderClient.WriteTextbox(By.Id("Input_163"), listing.Zip); // Zip Code
                this.uploaderClient.SetSelect(By.Id("Input_164"), listing.County); // County
            }

            void SetLegalDetails(ResidentialListingRequest listing)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_165"), listing.Subdivision); // Subdivision
                this.uploaderClient.WriteTextbox(By.Id("Input_171"), listing.SectionNum); // Section #
                this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.Legal); // Legal Description
            }

            void SetMasterPlannedCommunity(ResidentialListingRequest listing)
            {
                if (this.uploaderClient.FindElements(By.Id("Input_172")).Any())
                {
                    this.uploaderClient.SetSelect(By.Id("Input_172"), listing.IsPlannedDevelopment.BoolToNumericBool()); // Master Planned Community Y/N
                    this.uploaderClient.FillFieldSingleOption("Input_173", listing.PlannedDevelopment); // Master Planned Community
                }
            }

            void SetLegalSubdivision(string legalSubdivision, string zip)
            {
                if (!string.IsNullOrWhiteSpace(legalSubdivision))
                {
                    var legalsubdivision = legalSubdivision.Contains("OTHER") ? "OTHER - " + zip : legalSubdivision;
                    this.uploaderClient.WriteTextbox(By.Id("Input_320"), legalsubdivision);
                }
            }

            void SetKeyMap(string mapscoMapCoord)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_175"), mapscoMapCoord); // Key Map
            }
        }

        private void FillPropertyInformation(HarListingRequest listing, HousingType housingType, bool isNotPartialFill = true)
        {
            this.GoToPropertyInformationTab();

            this.uploaderClient.WriteTextbox(By.Id("Input_245"), listing.SqFtTotal); // Building SqFt
            if (housingType == HousingType.TownhouseCondo)
            {
                if (isNotPartialFill)
                {
                    this.uploaderClient.SetSelect(By.Id("Input_700"), "BUILD"); // SqFt Source
                    this.uploaderClient.WriteTextbox(By.Id("Input_485"), listing.NumStories); // Number of Building Stories
                    this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.NumStories); // Number of Unit Stories
                    this.uploaderClient.SetMultipleCheckboxById("Input_702", listing.HousingStyleDesc);  // Style
                    this.uploaderClient.SetMultipleCheckboxById("Input_475", listing.WaterDesc); // Water/Sewer Description
                    this.uploaderClient.SetSelect(By.Id("Input_496"), listing.WasherDryerConnection.BoolToNumericBool()); // washer Dryer Connection
                    this.uploaderClient.SetMultipleCheckboxById("Input_369", listing.Appliances); // Appliances
                    this.uploaderClient.SetSelect(By.Id("Input_265"), "0"); // Pool - Area (1, 0)
                }
            }
            else
            {
                this.uploaderClient.SetSelect(By.Id("Input_246"), "BUILD"); // SqFt Source
                if (isNotPartialFill)
                {
                    this.uploaderClient.SetMultipleCheckboxById("Input_241", listing.HousingStyleDesc);  // Style
                    this.uploaderClient.SetMultipleCheckboxById("Input_141", listing.WaterDesc); // Water/Sewer Description
                    this.uploaderClient.SetMultipleCheckboxById("Input_328", listing.WasherConnections); // Washer Dryer Connection
                    this.uploaderClient.SetSelect(By.Id("Input_265"), listing.HasCommunityPool.BoolToNumericBool()); // Pool - Area (1, 0)
                }
            }

            this.UpdateYearBuiltDescription(listing);
            this.uploaderClient.SetSelect(By.Id("Input_244"), "BUILD"); // Year Built Source
            this.uploaderClient.SetSelect(By.Id("Input_247"), "1"); // New Construction (1 , 0)

            if (isNotPartialFill)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.NumStories); // Stories
                this.uploaderClient.WriteTextbox(By.Id("Input_251"), listing.BuilderName);  // Builder Name

                this.FillCompletionDate(listing);

                if (this.uploaderClient.FindElements(By.Id("Input_374")).Any())
                {
                    this.uploaderClient.SetSelect(By.Id("Input_374"), "1"); // House on Property (1 , 0)
                }

                this.uploaderClient.SetSelect(By.Id("Input_142"), listing.HasUtilitiesDescription.BoolToNumericBool()); // Utility District  (1 , 0)
                this.uploaderClient.SetSelect(By.Id("Input_145"), listing.LotSizeSrc, isElementOptional: true); // Lot Size Source
                this.uploaderClient.WriteTextbox(By.Id("Input_143"), listing.LotSizeAcres); // Acres
                this.uploaderClient.SetSelect(By.Id("Input_148"), listing.LotSize); // Acreage
                this.uploaderClient.WriteTextbox(By.Id("Input_144"), listing.LotDim); // Lot Dimensions
                this.uploaderClient.WriteTextbox(By.Id("Input_202"), listing.GarageCapacity); // Garage - Number of Spaces
                this.uploaderClient.SetMultipleCheckboxById("Input_207", listing.AccessInstructionsDesc); // Access -- MLS-51 AccessibilityDesc -> AccessInstructionsDesc
                this.uploaderClient.SetMultipleCheckboxById("Input_203", listing.GarageDesc); // Garage Description

                if (this.uploaderClient.FindElements(By.Id("Input_206"))?.Any() == true)
                {
                    this.uploaderClient.SetMultipleCheckboxById("Input_206", listing.GarageCarpotDesc); // Garage Carpot Description
                }

                if (this.uploaderClient.FindElements(By.Id("Input_152")).Any())
                {
                    this.uploaderClient.SetMultipleCheckboxById("Input_152", listing.RestrictionsDesc); // Restrictions
                }

                if (this.uploaderClient.FindElements(By.Id("Input_356")).Any())
                {
                    this.uploaderClient.SetSelect(By.Id("Input_356"), "ALVLS"); // Unit Level
                }

                this.FindAndSetMultipleCheckboxById(new[] { "Input_329" }, listing.PropSubType); // Property Type
                this.FindAndSetMultipleCheckboxById(new[] { "Input_146", "Input_492" }, listing.LotDesc); // Lot Description
                this.uploaderClient.SetMultipleCheckboxById("Input_150", listing.WaterfrontFeatures); // Waterfront Features
                this.uploaderClient.SetSelect(By.Id("Input_208"), listing.HasMicrowave.BoolToNumericBool()); // Microwave (0, 1)
                this.uploaderClient.SetSelect(By.Id("Input_209"), listing.HasDishwasher.BoolToNumericBool()); // Dishwasher
                this.uploaderClient.SetSelect(By.Id("Input_210"), listing.HasDisposal.BoolToNumericBool()); // Disposal
                this.uploaderClient.WriteTextbox(By.Id("Input_253"), listing.CountertopsDesc); // Countertops
                this.uploaderClient.SetSelect(By.Id("Input_211"), listing.HasCompactor.BoolToNumericBool()); // Compactor (1, 0)
                this.uploaderClient.SetSelect(By.Id("Input_212"), listing.HasIcemaker.BoolToNumericBool()); // Separate Ice Maker
                this.uploaderClient.WriteTextbox(By.Id("Input_255"), listing.NumberFireplaces); // Fireplace - Number
                this.uploaderClient.SetMultipleCheckboxById("Input_256", listing.FireplaceDesc); // Fireplace Description
                this.uploaderClient.SetMultipleCheckboxById("Input_254", listing.FacesDesc); // Front Door Faces
                this.uploaderClient.SetMultipleCheckboxById("Input_269", listing.OvenDesc); // Oven Type
                this.uploaderClient.SetMultipleCheckboxById("Input_270", listing.RangeDesc);  // Stove Type

                if (!string.IsNullOrEmpty(listing.GolfCourseName) && this.uploaderClient.FindElements(By.Id("Input_151")).Any())
                {
                    this.uploaderClient.FillFieldSingleOption("Input_151", listing.GolfCourseName);
                }

                this.uploaderClient.SetSelect(By.Id("Input_263"), listing.HasPool.BoolToNumericBool()); // Pool - Private (1, 0)
                this.uploaderClient.SetMultipleCheckboxById("Input_264", listing.PoolDesc); // Private Pool Description
                this.FindAndSetMultipleCheckboxById(new[] { "Input_252", "Input_493", "Input_495" }, listing.InteriorDesc); // Interior Features
                this.FindAndSetMultipleCheckboxById(new[] { "Input_266" }, listing.FloorsDesc); // Flooring
                this.FindAndSetMultipleCheckboxById(new[] { "Input_259", "Input_472" }, listing.ExteriorDesc); // Exterior Description
                this.FindAndSetMultipleCheckboxById(new[] { "Input_260", "Input_473" }, listing.ConstructionDesc); // Exterior Construction
                this.FindAndSetMultipleCheckboxById(new[] { "Input_261" }, listing.RoofDesc); // Roof Description
                this.FindAndSetMultipleCheckboxById(new[] { "Input_262", "Input_500" }, listing.FoundationDesc); // Foundation Description
                this.FindAndSetMultipleCheckboxById(new[] { "Input_258", "Input_474" }, listing.EnergyDesc); // Energy Features
                this.uploaderClient.SetMultipleCheckboxById("Input_257", listing.GreenCerts); // Green/Energy Certifications
                this.uploaderClient.SetMultipleCheckboxById("Input_139", listing.HeatSystemDesc); // Heating System Description
                this.FindAndSetMultipleCheckboxById(new[] { "Input_140", "Input_506" }, listing.CoolSystemDesc); // Cooling System Description
            }
        }

        private void FindAndSetMultipleCheckboxById(IEnumerable<string> inputIds, string value)
        {
            var inputId = inputIds.FirstOrDefault(inputId => this.uploaderClient.FindElements(By.Id(inputId)).Any());
            if (inputId != null)
            {
                this.uploaderClient.SetMultipleCheckboxById(inputId, value);
            }
        }

        private void FillMapInformation(HarListingRequest listing)
        {
            if (listing.IsNewListing)
            {
                this.GoToTab("Map Information");
                this.uploaderClient.ScrollDown(250);
                this.SetLongitudeAndLatitudeValues(listing);
            }
        }

        private void FillRoomInformation(ResidentialListingRequest listing)
        {
            string tabName = "Rooms";
            this.uploaderClient.ClickOnElement(By.LinkText(tabName));
            Thread.Sleep(200);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            this.uploaderClient.WriteTextbox(By.Id("Input_267"), listing.Beds); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_268"), listing.BathsFull); // Baths - Full
            this.uploaderClient.WriteTextbox(By.Id("Input_196"), listing.BathsHalf); // Baths - Half
            this.uploaderClient.SetMultipleCheckboxById("Input_635", listing.BedroomDescription); // Bedroom Description
            this.uploaderClient.SetMultipleCheckboxById("Input_636", listing.RoomDescription); // Room Description
            this.uploaderClient.SetMultipleCheckboxById("Input_637", listing.BedBathDesc); // Bathroom Description
            this.uploaderClient.SetMultipleCheckboxById("Input_634", listing.KitchenDescription); // Kitchen Description

            if (!listing.IsNewListing)
            {
                int index = 0;
                this.uploaderClient.ScrollDown(200);

                while (this.uploaderClient.FindElements(By.LinkText("Delete")) != null &&
                    this.uploaderClient.FindElements(By.LinkText("Delete")).Count > 1)
                {
                    try
                    {
                        this.uploaderClient.ClickOnElement(By.LinkText("Delete"));
                        Thread.Sleep(400);
                    }
                    catch
                    {
                        this.uploaderClient.ScrollToTop();
                        this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl02_lbPageLink"));
                        this.uploaderClient.ExecuteScript("Subforms['s_191'].deleteRow('_Input_191__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                    }
                }

                this.NavigateToTab(tabName);
            }

            var i = 0;
            foreach (var room in listing.Rooms)
            {
                if (i > 0)
                {
                    this.uploaderClient.ClickOnElement(By.Id("_Input_191_more"));
                    this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(400));
                }

                var roomType = $"_Input_191__REPEAT{i}_187";
                this.uploaderClient.SetSelect(By.Id($"_Input_191__REPEAT{i}_187"), room.RoomType, "Room Type", tabName);
                this.uploaderClient.ResetImplicitWait();
                if (room.HasDimensions)
                {
                    this.uploaderClient.WriteTextbox(By.Id($"_Input_191__REPEAT{i}_189"), room.Dimensions, isElementOptional: true);
                }

                this.uploaderClient.SetSelect(By.Id($"_Input_191__REPEAT{i}_190"), room.Level, "Level", tabName, isElementOptional: true);

                this.uploaderClient.ScrollDownToElementHTML(roomType);
                i++;
            }
        }

        private void FillFinancialInformation(HarListingRequest listing, HousingType housingType)
        {
            this.GoToTab("Financial Information");

            if (housingType != HousingType.CountryHomesAcreage)
            {
                if (housingType == HousingType.TownhouseCondo)
                {
                    this.uploaderClient.SetSelect(By.Id("Input_497"), listing.HasHoa.BoolToNumericBool()); // Mandatory HOA/Mgmt Co (1, 0)
                    this.uploaderClient.WriteTextbox(By.Id("Input_693"), listing.AssocPhone.PhoneFormat(true)); // Mandatory HOA/Mgmt Co Phone
                }
                else
                {
                    this.uploaderClient.SetSelect(By.Id("Input_275"), listing.HasHoa.BoolToNumericBool()); // Mandatory HOA/Mgmt Co (1, 0)
                    this.uploaderClient.WriteTextbox(By.Id("Input_276"), listing.AssocPhone.PhoneFormat(true)); // Mandatory HOA/Mgmt Co Phone
                }

                this.uploaderClient.WriteTextbox(By.Id("Input_278"), listing.AssocName); // Mandatory HOA/Mgmt Co Name
            }

            this.uploaderClient.SetMultipleCheckboxById("Input_280", listing.FinancingProposed);  // Financing Considered
            this.FindAndSetMultipleCheckboxById(new[] { "Input_494", "Input_273", "Input_476" }, listing.Disclosures);  // Disclosures

            bool hasAgentBonus = listing.HasAgentBonus ?? false;
            bool hasBonusWithAmount = listing.HasBonusWithAmount;
            if (!hasAgentBonus || !hasBonusWithAmount)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_723"), null); // Seller May Concessions
            }
            else if (listing.AgentBonusAmountType.Equals("$") && !string.IsNullOrEmpty(listing.AgentBonusAmount))
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_723"), listing.AgentBonusAmount); // Seller May Concessions
            }

            this.uploaderClient.SetSelect(By.Id("Input_674"), listing.IsActiveCommunity.BoolToNumericBool()); // 55+ Active Community
            this.uploaderClient.SetSelect(By.Id("Input_347"), listing.HasOtherFees.BoolToNumericBool()); // Other Mandatory Fees
            this.uploaderClient.WriteTextbox(By.Id("Input_286"), listing.OtherFees); // Other Mandatory Fees Amount
            this.uploaderClient.WriteTextbox(By.Id("Input_285"), listing.OtherFeesInclude); // Other Mandatory Fees Include
            this.uploaderClient.WriteTextbox(By.Id("Input_290"), listing.TaxYear); // Tax Year
            this.uploaderClient.WriteTextbox(By.Id("Input_292"), listing.TaxRate);  // Total Tax Rate
            this.uploaderClient.WriteTextbox(By.Id("Input_293"), listing.ExemptionsDesc); // Exemptions

            if (housingType == HousingType.SingleFamily)
            {
                this.uploaderClient.SetSelect(By.Id("Input_471"), listing.HOA); // Maintenance Fee
            }
            else
            {
                this.uploaderClient.SetSelect(By.Id("Input_281"), listing.HOA.ToEnumFromEnumMember<HoaRequirement>() == HoaRequirement.No ? 0 : 1); // Maintenance Fee (1, 0)
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_282"), listing.AssocFee); // Maintenance Fee Amount
            this.uploaderClient.SetSelect(By.Id("Input_283"), listing.AssocFeeFrequency); // Maintenance Fee Payment Sched
            if (this.uploaderClient.FindElements(By.Id("Input_352")).Any())
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_352", "OTHER"); // Maintenance Fee Includes
            }
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            this.GoToTab("Showing Information");

            this.uploaderClient.WriteTextbox(By.Id("Input_304"), listing.AgentListApptPhone.PhoneFormat(true), isElementOptional: true);  // Appointment Desk Phone
            this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(2000));
            this.uploaderClient.SetSelect(By.Id("Input_303"), "OFFIC");  // Appointment Phone Desc
            this.uploaderClient.WriteTextbox(By.Id("Input_236"), listing.OtherPhone.PhoneFormat(true), isElementOptional: true);  // Agent Alternate Phone
            this.uploaderClient.ResetImplicitWait();
            this.uploaderClient.WriteTextbox(By.Id("Input_136"), listing.Directions.RemoveSlash(), true); // Directions
            this.uploaderClient.SetMultipleCheckboxById("Input_218", listing.ShowingInstructions);  // Showing Instructions
        }

        private void FillRemarks(HarListingRequest listing, HousingType housingType)
        {
            this.GoToRemarksTab(housingType);

            this.UpdatePublicRemarksInRemarksTab(listing);
        }

        private void GoToRemarksTab(HousingType housingType)
        {
            if (housingType == HousingType.TownhouseCondo)
            {
                this.GoToTab("Remarks/Tour Remarks");
            }
            else
            {
                this.GoToTab("Remarks/Tour Links");
            }
        }

        private void GoToPropertyInformationTab()
        {
            this.GoToTab("Property Information");
        }

        private void GoToManageTourLinks()
        {
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Tours Links"));
            this.uploaderClient.FindElement(By.LinkText("Manage Tours Links")).Click();
        }

        private void GoToTab(string tabText)
        {
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ClickOnElement(By.LinkText(tabText));
            Thread.Sleep(500);
        }

        private async Task FillMedia(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping media upload for existing listing {listingId}", listing.ResidentialListingID);
                return;
            }

            // Enter Manage Photos
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lbSaveIncomplete"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_lbSaveIncomplete"));
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lbManagePhotos"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_lbManagePhotos"));

            await this.ProcessImages(listing, cancellationToken);
        }

        private void UpdateYearBuiltDescription(ResidentialListingRequest listing)
        {
            this.uploaderClient.WriteTextbox(By.Id("Input_243"), listing.YearBuilt); // Year Built
            this.uploaderClient.SetSelect(By.Id("Input_248"), listing.YearBuiltDesc); // New Construction Desc
        }

        private void FillCompletionDate(ResidentialListingRequest listing)
        {
            if (listing.BuildCompletionDate.HasValue)
            {
                var yearBuiltDesc = listing.YearBuiltDesc.ToEnumFromEnumMember<ConstructionStage>();
                if (yearBuiltDesc == ConstructionStage.Complete)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_249"), string.Empty, false, true); // Approx Completion Date
                    this.uploaderClient.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
                else if (yearBuiltDesc == ConstructionStage.Incomplete)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_301"), string.Empty, true, true); // Approx Completion Date
                    this.uploaderClient.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
            }
        }

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            var remarks = listing.GetPublicRemarks();
            string baseRemarks = listing.GetAgentRemarksMessage() ?? string.Empty;
            string additionalRemarks = listing.AgentPrivateRemarksAdditional ?? string.Empty;
            var agentRemarks = $"{baseRemarks} {additionalRemarks}";
            this.uploaderClient.WriteTextbox(By.Id("Input_135"), remarks, true); // Public Remarks
            this.uploaderClient.WriteTextbox(By.Id("Input_137"), agentRemarks, true); // Agent Remarks
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            if (listing.UpdateGeocodes)
            {
                this.uploaderClient.WriteTextbox(By.Id("INPUT__93"), listing.Latitude); // latitude
                this.uploaderClient.WriteTextbox(By.Id("INPUT__94"), listing.Longitude); // longitude
            }
            else
            {
                var getLatLongFromAddress = "Get Lat/Long from address";

                if (this.uploaderClient.FindElements(By.LinkText(getLatLongFromAddress)).Any())
                {
                    this.uploaderClient.ClickOnElement(By.LinkText(getLatLongFromAddress));
                    Thread.Sleep(1000);
                }
            }
        }

        private void DeleteAllImages()
        {
            if (this.uploaderClient.FindElements(By.Id("cbxCheckAll")).Any())
            {
                this.uploaderClient.ClickOnElement(By.Id("cbxCheckAll"));
                this.uploaderClient.ClickOnElement(By.Id("m_lbDeleteChecked"));
                Thread.Sleep(1000);
                this.uploaderClient.AcceptAlertWindow();
            }
        }

        [SuppressMessage("SonarLint", "S2583", Justification = "Ignored due to suspected false positive")]
        private async Task ProcessImages(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            var media = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: this.CurrentMarket, token: cancellationToken);
            var imageOrder = 0;
            var imageRow = 0;
            var imageCell = 0;
            const int maxDescriptionLength = 90;
            string mediaFolderName = "Husa.Core.Uploader";
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            var captionImageId = string.Empty;
            var truncatedCaption = string.Empty;
            foreach (var image in media)
            {
                await this.mediaRepository.PrepareImage(image, MarketCode.Houston, cancellationToken, folder);

                if (image.IsBrokenLink)
                {
                    continue;
                }

                captionImageId = $"m_rptPhotoRows_ctl{imageRow:D2}_m_rptPhotoCells_ctl{imageCell:D2}_m_ucPhotoCell_m_tbxDescription";

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_ucImageLoader_m_tblImageLoader"), cancellationToken);

                this.uploaderClient.FindElement(By.Id("m_ucImageLoader_m_tblImageLoader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                this.WaitForElementAndThenDoAction(
                        By.Id(captionImageId),
                        (element) =>
                        {
                            if (!string.IsNullOrEmpty(image.Caption))
                            {
                                truncatedCaption = image.Caption.Length > maxDescriptionLength ? image.Caption.Substring(0, maxDescriptionLength) : image.Caption;
                                this.uploaderClient.ExecuteScript(script: $"jQuery('#{captionImageId}').val('{truncatedCaption.Replace("'", "\\'")}');");
                            }
                        });

                imageOrder++;
                imageCell++;
                if (imageOrder % 5 == 0)
                {
                    imageRow++;
                    imageCell = 0;
                }

                this.uploaderClient.ScrollDown(200);
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
            var elems = this.uploaderClient.FindElements(By.CssSelector("table[id^=_Input_337__del_REPEAT] a"))?.Count(c => c.Displayed);
            if (elems == null || elems == 0)
            {
                return;
            }

            this.uploaderClient.ScrollDown(3000);
            while (elems > 0)
            {
                this.uploaderClient.ScrollDown();
                var elementId = $"_Input_337__del_REPEAT{elems - 1}_";
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
            var index = 0;
            Thread.Sleep(1000);
            var sortedOpenHouses = listing.OpenHouse.OrderBy(openHouse => openHouse.Date).ToList();
            foreach (var openHouse in sortedOpenHouses)
            {
                if (index != 0)
                {
                    this.uploaderClient.ScrollDown();
                    this.uploaderClient.ClickOnElementById(elementId: $"_Input_337_more");
                    Thread.Sleep(1000);
                }

                // Open House Type
                this.uploaderClient.SetSelect(By.Id($"_Input_337__REPEAT{index}_330"), value: "Public");

                // Refreshments
                this.uploaderClient.SetMultipleCheckboxById($"_Input_337__REPEAT{index}_335", openHouse.Refreshments);
                this.uploaderClient.ScrollDown();

                // Date
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_332"), entry: openHouse.Date);

                // From Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_TextBox_333"), entry: openHouse.StartTime.To12Format());
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_337__REPEAT{index}_RadioButtonList_333_{fromTimeTT}", shouldWait: true, waitTime: 5);

                // To Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_TextBox_334"), entry: openHouse.EndTime.To12Format());
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_337__REPEAT{index}_RadioButtonList_334_{endTimeTT}", shouldWait: true, waitTime: 5);

                // Comments
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_339"), entry: openHouse.Comments);

                index++;
            }
        }

        private void FillLotListingInformation(LotListingRequest listing)
        {
            this.GoToTab("Listing Information");
            this.uploaderClient.SetSelect(By.Id("Input_181"), listing.LotListType); // List Type
            this.uploaderClient.WriteTextbox(By.Id("Input_182"), listing.ListPrice); // List Price
            this.FillListDate(listing.ListStatus); // list Date
            this.uploaderClient.WriteTextbox(By.Id("Input_184"), listing.ExpiredDate); // Expired Date
            this.uploaderClient.SetSelect(By.Id("Input_424"), "RESAL"); // type of contract
            this.uploaderClient.WriteTextbox(By.Id("Input_156"), listing.StreetNum); // Street Number
            this.uploaderClient.SetSelect(By.Id("Input_157"), listing.StDirection); // St Direction
            this.uploaderClient.WriteTextbox(By.Id("Input_158"), listing.StreetName); // Street Name
            this.uploaderClient.SetSelect(By.Id("Input_159"), listing.StreetType, isElementOptional: true); // Street Type
            this.uploaderClient.WriteTextbox(By.Id("Input_425"), listing.LotNumber); // Lot #
            this.uploaderClient.FillFieldSingleOption("Input_161", listing.City);
            this.uploaderClient.SetSelect(By.Id("Input_162"), listing.State); // State
            this.uploaderClient.WriteTextbox(By.Id("Input_163"), listing.Zip); // Zip Code
            this.uploaderClient.SetSelect(By.Id("Input_164"), listing.County); // County
            this.uploaderClient.WriteTextbox(By.Id("Input_165"), listing.Subdivision); // Subdivision
            this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.LegalDescription); // Legal Description
            this.uploaderClient.WriteTextbox(By.Id("Input_320"), listing.LegalSubdivision); // Legal Subdivision
            this.uploaderClient.SetSelect(By.Id("Input_172"), listing.HasMasterPlannedCommunity); // Master Planned Community Y/N
            this.uploaderClient.FillFieldSingleOption("Input_173", listing.MasterPlannedCommunity); // Master Planned Community Name
        }

        private void NavigateToTab(string tabName)
        {
            this.uploaderClient.ClickOnElement(By.LinkText(tabName));
            this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(800));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));
            this.uploaderClient.ResetImplicitWait();
        }
    }
}
