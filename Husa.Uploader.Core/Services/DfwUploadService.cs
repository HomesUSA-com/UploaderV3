namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
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

    public class DfwUploadService : IDfwUploadService
    {
        private const string LandingPageURL = "https://matrix.ntreis.net/";
        private const string EditUrl = "https://ntrdd.mlsmatrix.com/Matrix/Input";
        private const string NavigateToUrlNewListing = "https://ntrdd.mlsmatrix.com/Matrix/Input";
        private const int ListDateSold = 4;
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<DfwUploadService> logger;

        public DfwUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<DfwUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.DFW;
        public string MultiFamilyPropType => ListPropertyType.ResidencialIncome.ToStringFromEnumMember();

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public async Task<LoginResult> Login(Guid companyId)
        {
            var marketInfo = this.options.MarketInfo.Dfw;
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            this.uploaderClient.DeleteAllCookies();

            var credentials = await LoginCommon.GetMarketCredentials(company, credentialsTask);

            // Connect to the login page
            var loginButtonId = "loginbtn";
            this.uploaderClient.NavigateToUrl(marketInfo.LogoutUrl);
            Thread.Sleep(1000);
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id(loginButtonId));
            this.uploaderClient.WriteTextbox(By.Name("username"), credentials[LoginCredentials.Username]);
            this.uploaderClient.WriteTextbox(By.Name("password"), credentials[LoginCredentials.Password]);
            Thread.Sleep(1000);
            this.uploaderClient.ClickOnElementById(loginButtonId);

            this.uploaderClient.ExecuteScript(" $('.tour-backdrop').remove();$('#step-0').remove();");

            Thread.Sleep(1000);
            try
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("RedirectingPopup"));
                Thread.Sleep(4000);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "The redirect popup was not displayed in the login screen., {Message}", e.Message);
            }

            this.uploaderClient.NavigateToUrl(LandingPageURL);
            Thread.Sleep(1000);

            return LoginResult.Logged;
        }

        public UploadResult Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Dfw.LogoutUrl);
            return UploadResult.Success;
        }

        public Task<UploadResult> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return EditListing();

            async Task<UploadResult> EditListing()
            {
                this.logger.LogInformation("Editing the information for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);

                this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListing(logIn);

            async Task<UploadResult> UploadListing(bool logIn)
            {
                this.logger.LogInformation("Uploading the information for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                Thread.Sleep(2000);

                try
                {
                    NavigateToForm(listing);
                    FillListingDetails(listing);

                    if (listing.IsNewListing)
                    {
                        await this.UpdateVirtualTour(listing, cancellationToken);
                    }

                    await this.FillMedia(listing, cancellationToken);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the listing {RequestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }

            void NavigateToForm(ResidentialListingRequest listing)
            {
                if (listing.IsNewListing)
                {
                    this.NavigateToNewPropertyInput(listing.PropType);
                }
                else
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                }
            }

            void FillListingDetails(ResidentialListingRequest listing)
            {
                this.FillPropertyInformation(listing as DfwListingRequest);
                this.FillLocationSchools(listing as DfwListingRequest);

                if (listing.PropType == this.MultiFamilyPropType)
                {
                    this.FillUnitsInformation(listing as DfwListingRequest); // Multi Family
                }
                else
                {
                    this.FillRoomsInformation(listing as DfwListingRequest); // Single Family
                }

                this.FillFeaturesInformation(listing as DfwListingRequest);
                this.FillLotInformation(listing as DfwListingRequest);
                this.FillUtilitiesInformation(listing as DfwListingRequest);
                this.FillEnvironmentInformation(listing as DfwListingRequest);
                this.FillFinancialInformation(listing as DfwListingRequest);
                this.FillAgentOfficeInformation();
                this.FillShowingInformation(listing as DfwListingRequest);
                this.FillRemarksInformation(listing as DfwListingRequest);

                if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
                {
                    this.FillStatusInformation(listing as DfwListingRequest);
                }
            }
        }

        public Task<UploadResult> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return PartialUploadListing(logIn);

            async Task<UploadResult> PartialUploadListing(bool logIn)
            {
                this.logger.LogInformation("Partial Uploading the information for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                Thread.Sleep(2000);

                try
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    this.FillPropertyInformation(listing as DfwListingRequest, isNotPartialFill: false);
                    this.FillLocationSchools(listing as DfwListingRequest, isNotPartialFill: false);
                    this.FillFinancialInformation(listing as DfwListingRequest);
                    this.FillAgentOfficeInformation();
                    this.FillShowingInformation(listing as DfwListingRequest, isNotPartialFill: false);
                    this.FillRemarksInformation(listing as DfwListingRequest);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingCompletionDate(logIn);

            async Task<UploadResult> UpdateListingCompletionDate(bool logIn)
            {
                this.logger.LogInformation("Updating CompletionDate for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                try
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    this.GoToPropertyInformationTab();
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_403"));
                    this.uploaderClient.SetSelect(By.Id("Input_403"), listing.YearBuiltDesc); // Construction Status
                    this.uploaderClient.WriteTextbox(By.Id("Input_231"), listing.YearBuilt); // Year Built

                    this.GoToTab("Remarks");
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_263"));
                    this.uploaderClient.WriteTextbox(By.Id("Input_263"), listing.GetPublicRemarks()); // Property Description

                    this.UpdatePrivateRemarksInRemarksTab(listing as DfwListingRequest);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateImages(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingImages();
            async Task<UploadResult> UpdateListingImages()
            {
                this.logger.LogInformation("Updating media for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                await this.Login(listing.CompanyId);
                this.NavigateToQuickEdit(listing.MLSNum);

                // Enter Manage Photos
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Photos"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Manage Photos"));
                this.DeleteAllImages();
                await this.ProcessImages(listing, cancellationToken);
                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingPrice();

            async Task<UploadResult> UpdateListingPrice()
            {
                this.logger.LogInformation("Updating the price of the listing {RequestId} to {ListPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);

                try
                {
                    this.NavigateToQuickEdit(listing.MLSNum);

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Price Change"), cancellationToken);
                    this.uploaderClient.ClickOnElement(By.LinkText("Price Change"));
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_77"), cancellationToken);
                    this.uploaderClient.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingStatus(logIn);

            async Task<UploadResult> UpdateListingStatus(bool logIn)
            {
                this.logger.LogInformation("Editing the status information for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                    Thread.Sleep(1000);
                }

                try
                {
                    this.NavigateToQuickEdit(listing.MLSNum);

                    Thread.Sleep(1000);
                    var linkText = string.Empty;
                    var transformedStatus = this.TransformStatus(listing.ListStatus, ref linkText);

                    if (transformedStatus == null)
                    {
                        return UploadResult.Failure;
                    }

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.PartialLinkText(linkText));
                    this.uploaderClient.ClickOnElement(By.PartialLinkText(linkText));
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("stickypush"));

                    switch (transformedStatus)
                    {
                        case "SLD":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_84"));

                            this.uploaderClient.WriteTextbox(By.Id("Input_84"), listing.SoldPrice); // Close Price
                            this.uploaderClient.WriteTextbox(By.Id("Input_457"), listing.SellConcess); // Seller Contribution
                            this.uploaderClient.WriteTextbox(By.Id("Input_233"), listing.SqFtTotal); // Living Area

                            this.uploaderClient.WriteTextbox(By.Id("Input_85"), listing.ClosedDate.Value.ToShortDateString()); // Close Date
                            this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.PendingDate.Value.ToShortDateString()); // Purchase Contract Date

                            this.uploaderClient.SetSelect(By.Id("Input_460"), "0"); // Third Party Assistance Program
                            this.uploaderClient.SetSelect(By.Id("Input_496"), listing.SoldTerms); // Buyer Financing
                            this.uploaderClient.ScrollDownPosition(100);
                            this.uploaderClient.WriteTextbox(By.Id("Input_467"), listing.MortgageCoSold); // Mortgage Company
                            this.uploaderClient.WriteTextbox(By.Id("Input_468"), listing.TitleCo); // Closing Title Company

                            this.uploaderClient.SetSelect(By.Id("Input_234"), listing.SqFtSource); // Living Area Source
                            this.uploaderClient.SetSelect(By.Id("Input_496"), listing.MFinancing); // Buyer Financing
                                                                                                   //// 1st Term in Years
                                                                                                   //// 1st Loan Amount
                                                                                                   //// 1st Interest Rate
                            this.uploaderClient.SetSelect(By.Id("Input_624"), listing.HasBuyerAgent.BoolToNumericBool());  // Buyers/SubAgent
                            this.uploaderClient.SetSelect(By.Id("Input_625"), listing.HasSecondBuyerAgent.BoolToNumericBool());  // Buyers/SubAgent2
                            this.uploaderClient.WriteTextbox(By.Id("Input_141_displayValue"), listing.SellingAgentLicenseNum ?? "99999999"); // Buyers/SubAgent ID
                            this.uploaderClient.ExecuteScript(" document.getElementById('Input_141_Refresh').value='1';RefreshToSamePage(); ");
                            this.uploaderClient.WriteTextbox(By.Id("Input_145_displayValue"), listing.SecondAgentMarketUniqueId); // Buyers/SubAgent 2 ID
                            this.uploaderClient.ExecuteScript(" document.getElementById('Input_145_Refresh').value='1';RefreshToSamePage(); ");
                            break;
                        case "PND":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.PendingDate.Value.ToShortDateString()); // Purchase Contract Date

                            break;
                        case "ACT":
                            var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_5"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_5"), expirationDate);
                            break;
                        case "AC":
                        case "AKO":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.ContractDate.Value.ToShortDateString()); // Contract Date
                            this.uploaderClient.WriteTextbox(By.Id("Input_451"), listing.ContingencyInfo); // Contingency Info
                            break;
                        case "AOC":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.ContractDate.Value.ToShortDateString()); // Purchase Contract Date
                            this.uploaderClient.WriteTextbox(By.Id("Input_453"), listing.EstClosedDate.Value.ToShortDateString()); // Option Expire Date
                            break;
                        case "CAN":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_472"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_472"), DateTime.Now.ToShortDateString()); // Cancelled Date
                            break;
                        case "TOM":
                            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_474"));
                            this.uploaderClient.WriteTextbox(By.Id("Input_474"), listing.OffMarketDate);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {RequestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UploadVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListingVirtualTour();

            async Task<UploadResult> UploadListingVirtualTour()
            {
                this.logger.LogInformation("Updating VirtualTour for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);

                this.NavigateToQuickEdit(listing.MLSNum);

                this.GoToManageTourLinks();

                await this.UpdateVirtualTourLinks(listing, cancellationToken);

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadOpenHouse();

            async Task<UploadResult> UploadOpenHouse()
            {
                this.logger.LogInformation("Editing the information of Open House for the listing {RequestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(5000);
                this.NavigateToQuickEdit(listing.MLSNum);

                if (!listing.OpenHouse.Any())
                {
                    return UploadResult.Success;
                }

                Thread.Sleep(400);

                // Enter OpenHouse
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_dlInputList_ctl03_m_btnSelect"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl03_m_btnSelect"));
                Thread.Sleep(3000);

                this.CleanOpenHouse(cancellationToken);
                Thread.Sleep(2000);

                if (listing.EnableOpenHouse)
                {
                    if (listing.ListStatus != MarketStatuses.Pending.ToStringFromEnumMember()
                        || (listing.ListStatus == MarketStatuses.Pending.ToStringFromEnumMember()
                        && listing.AllowPendingList))
                    {
                        this.AddOpenHouses(listing);
                        Thread.Sleep(2000);
                    }
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UploadLot(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            throw new NotImplementedException();
        }

        public Task<UploadResult> UpdateLotStatus(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.DFW, cancellationToken);
            if (!virtualTours.Any())
            {
                return;
            }

            this.GoToTab("Virtual Tours/URLs");

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

        private void NavigateToNewPropertyInput(string propType)
        {
            this.uploaderClient.NavigateToUrl(NavigateToUrlNewListing);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Add new"));
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));
            Thread.Sleep(1000);

            if (propType == this.MultiFamilyPropType)
            {
                this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl01_m_btnSelect")); // Multi Family
            }
            else
            {
                this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Single Family
            }

            Thread.Sleep(1000);
            WaitAndClick("Start with a blank Property");

            void WaitAndClick(string text)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.PartialLinkText(text));
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
                this.uploaderClient.WriteTextbox(By.Id("Input_697"), firstVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }

            virtualTours = virtualTours.Skip(1).ToList();
            var secondVirtualTour = virtualTours.FirstOrDefault();
            if (secondVirtualTour != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_698"), secondVirtualTour.MediaUri.AbsoluteUri); // Virtual Tour URL Unbranded
            }
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl(EditUrl);
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Edit existing"));
            this.uploaderClient.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), value: mlsNumber);
            Thread.Sleep(500);
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
            Thread.Sleep(1000);
        }

        private void NavigateToEditResidentialForm(string mlsNumber, CancellationToken cancellationToken = default)
        {
            this.NavigateToQuickEdit(mlsNumber);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl00_m_btnSelect"));
            Thread.Sleep(1000);
        }

        private void FillPropertyInformation(DfwListingRequest listing, bool isNotPartialFill = true)
        {
            this.GoToPropertyInformationTab();
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            // Common Single/Multi Family Fields
            this.uploaderClient.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price
            this.uploaderClient.SetSelect(By.Id("Input_403"), value: listing.YearBuiltDesc); // Year Built Details/Construc. Status
            this.uploaderClient.WriteTextbox(By.Id("Input_231"), listing.YearBuilt); // Year Built

            if (isNotPartialFill)
            {
                this.uploaderClient.SetSelect(By.Id("Input_223"), value: "EXCAG"); // Listing Agreement Type
                this.uploaderClient.SetSelect(By.Id("Input_230"), value: listing.AdultCommunity.BoolToNumericBool()); // Adult Community YN
                this.uploaderClient.SetSelect(By.Id("Input_381"), value: "NO"); // Will Subdivide
                this.uploaderClient.SetMultipleCheckboxById("Input_228", listing.ConstructionDesc); // Construction Material

                if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
                {
                    var listDate = DateTime.Now;
                    switch (listing.ListStatus)
                    {
                        case "ACT":
                        case "AC":
                        case "AKO":
                        case "AOC":
                        case "CSN":
                            listDate = DateTime.Now;
                            break;
                        case "PND":
                            listDate = DateTime.Now.AddDays(-2);
                            break;
                        case "SLD":
                            listDate = DateTime.Now.AddDays(ListDateSold);
                            break;
                    }

                    this.uploaderClient.WriteTextbox(By.Id("Input_80"), listDate.Date.ToShortDateString()); // List Date
                }

                this.uploaderClient.WriteTextbox(By.Id("Input_81"), DateTime.Today.AddYears(1).ToShortDateString()); // Expire Date
                this.uploaderClient.WriteTextbox(By.Id("Input_235"), string.IsNullOrWhiteSpace(listing.TaxID) ? "NA" : listing.TaxID); // Parcel Id
                this.uploaderClient.SetSelect(By.Id("Input_237"), value: "0"); // Multi Parcel ID YN
            }

            if (listing.PropType == this.MultiFamilyPropType)
            {
                // Exclusive Multi family Fields
                if (isNotPartialFill)
                {
                    this.uploaderClient.SetSelect(By.Id("Input_503"), value: listing.PropSubType);
                    this.uploaderClient.WriteTextbox(By.Id("Input_533"), listing.SqFtTotal); // Building Area
                    this.uploaderClient.SetSelect(By.Id("Input_534"), value: listing.SqFtSource); // Building Area Source
                    this.uploaderClient.SetSelect(By.Id("Input_539"), listing.NumStories); // Levels
                    this.uploaderClient.WriteTextbox(By.Id("Input_523"), listing.GarageCapacity); // Garage Spaces
                }
            }
            else
            {
                // Exclusive Single family Fields
                this.uploaderClient.WriteTextbox(By.Id("Input_233"), listing.SqFtTotal); // SqFt/Living Area
                this.uploaderClient.SetSelect(By.Id("Input_234"), value: listing.SqFtSource); // SqFt Source

                if (isNotPartialFill)
                {
                    this.uploaderClient.SetSelect(By.Id("Input_219"), value: listing.PropSubType);
                    this.uploaderClient.SetSelect(By.Id("Input_222"), listing.HasPropertyAttached.BoolToNumericBool()); // Property Attached YN
                    this.uploaderClient.SetSelect(By.Id("Input_224"), value: "FS"); // Transaction Type
                    this.uploaderClient.WriteTextbox(By.Name("Input_225"), string.Empty); // Lease MLS#
                    this.uploaderClient.SetMultipleCheckboxById("Input_220", listing.HousingTypeDesc); // Housing Type
                    this.uploaderClient.SetMultipleCheckboxById("Input_226", listing.HousingStyleDesc); // Architectural Style
                }
            }

            Thread.Sleep(500);
        }

        private void FillLocationSchools(DfwListingRequest listing, bool isNotPartialFill = true)
        {
            this.GoToTab("Location/Schools");

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            if (isNotPartialFill)
            {
                if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.StreetNum); // Street/Box Number
                    this.uploaderClient.SetSelect(By.Id("Input_280"), value: listing.StreetDir); // Street Direction
                    this.uploaderClient.WriteTextbox(By.Id("Input_170"), listing.StreetName?.Replace('\'', ' ')); // Street Name
                    if (!string.IsNullOrWhiteSpace(listing.StreetType))
                    {
                        this.uploaderClient.SetSelect(By.Id("Input_281"), value: listing.StreetType); // Street Type
                    }

                    this.uploaderClient.SetSelect(By.Id("Input_285"), value: listing.County); // County
                    this.uploaderClient.SetSelect(By.Id("Input_284"), value: listing.CityCode); // City Code
                    this.uploaderClient.WriteTextbox(By.Id("Input_283"), listing.UnitNum); // Unit #
                }

                this.uploaderClient.SetSelect(By.Id("Input_282"), value: listing.StreetSuffixFQ); // Street Directional Suffix

                this.uploaderClient.WriteTextbox(By.Id("Input_294"), listing.Zip); // Zip
                this.uploaderClient.WriteTextbox(By.Id("Input_295"), string.Empty); // Zip + 4
                this.uploaderClient.WriteTextbox(By.Id("Input_288"), !string.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : string.Empty); // Lot
                this.uploaderClient.WriteTextbox(By.Id("Input_289"), !string.IsNullOrEmpty(listing.Block) ? listing.Block : string.Empty); // Block

                this.uploaderClient.WriteTextbox(By.Id("Input_290"), listing.Subdivision); // Subdivision
                this.uploaderClient.WriteTextbox(By.Id("Input_292"), listing.PlannedDevelopment); // Planned Development
                this.uploaderClient.WriteTextbox(By.Id("Input_293"), listing.Legal); // Additional Legal

                this.uploaderClient.ScrollDown(1000);
            }

            this.SetLongitudeAndLatitudeValues(listing);

            try
            {
                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_334"), value: listing.SchoolDistrict); // School District

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_335"), value: listing.SchoolName1); // Elementary School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_336"), value: listing.SchoolName5); // Junior School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_337"), value: listing.SchoolName6); // Senior High School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_338"), value: listing.SchoolName4); // Primary School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_339"), value: listing.SchoolName2); // Middle School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_340"), value: listing.HighSchool); // High School

                Thread.Sleep(500);
                this.uploaderClient.SetSelect(By.Id("Input_341"), value: listing.SchoolName7); // Intermediate School
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "An exception was occur in School, {Message}", e.Message);
            }
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if ((listing.IsForLease == "Yes" && string.IsNullOrEmpty(listing.MLSNum)) || string.IsNullOrEmpty(listing.MLSNum))
            {
                this.uploaderClient.WriteTextbox(By.Id("INPUT__146"), listing.Latitude); // Latitude
                this.uploaderClient.WriteTextbox(By.Id("INPUT__168"), listing.Longitude); // Longitude
            }
        }

        private void FillRoomsInformation(DfwListingRequest listing)
        {
            var tabName = "Rooms";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Id("Input_243"), listing.Beds); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_398"), listing.BathsFull); // Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_399"), listing.BathsHalf); // Half Baths

            this.uploaderClient.SetSelect(By.Id("Input_539"), listing.NumStories, "# Levels", tabName); // # Levels
            this.uploaderClient.WriteTextbox(By.Id("Input_401"), listing.NumLivingAreas); // # Living Areas
            this.uploaderClient.WriteTextbox(By.Id("Input_402"), listing.NumDiningAreas); // # Dining Areas

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && !this.uploaderClient.UploadInformation.IsNewListing)
            {
                int index = 0;

                while (this.uploaderClient.FindElements(By.LinkText("Delete")) != null &&
                    this.uploaderClient.FindElements(By.LinkText("Delete")).Count > 1)
                {
                    try
                    {
                        this.uploaderClient.ScrollToTop();
                        this.uploaderClient.ClickOnElement(By.LinkText("Delete"));
                    }
                    catch
                    {
                        this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl04_lbPageLink"));
                        this.uploaderClient.ExecuteScript("Subforms['s_442'].deleteRow('_Input_442__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                    }
                }
            }

            this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl04_lbPageLink"));
            Thread.Sleep(400);

            var i = 0;

            foreach (var room in listing.Rooms)
            {
                if (i > 0)
                {
                    this.uploaderClient.ScrollDown(10000);
                    Thread.Sleep(400);
                    this.uploaderClient.ClickOnElement(By.Id("_Input_442_more"));
                    Thread.Sleep(400);
                }

                this.uploaderClient.SetSelect(By.Id("_Input_442__REPEAT" + i + "_440"), room.RoomType, "Name", tabName, true); // FieldName
                Thread.Sleep(400);
                this.uploaderClient.ScrollDown();
                this.uploaderClient.SetSelect(By.Id("_Input_442__REPEAT" + i + "_443"), room.Level, "Level", tabName, true);
                Thread.Sleep(400);
                this.uploaderClient.ScrollDown();
                this.uploaderClient.WriteTextbox(By.Id("_Input_442__REPEAT" + i + "_444"), room.Length, true);
                Thread.Sleep(400);
                this.uploaderClient.ScrollDown();
                this.uploaderClient.WriteTextbox(By.Id("_Input_442__REPEAT" + i + "_445"), room.Width, true);
                Thread.Sleep(400);
                this.uploaderClient.ScrollDown();
                this.uploaderClient.SetMultipleCheckboxById("_Input_442__REPEAT" + i + "_441", room.Features, "Features", tabName);
                Thread.Sleep(400);
                this.uploaderClient.ScrollDown();

                i++;
            }
        }

        private void FillUnitsInformation(DfwListingRequest listing)
        {
            var tabName = "Units";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Id("Input_243"), listing.Beds); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_398"), listing.BathsFull); // Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_399"), listing.BathsHalf); // Half Baths

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && !this.uploaderClient.UploadInformation.IsNewListing)
            {
                int index = 0;

                while (this.uploaderClient.FindElements(By.LinkText("Delete")) != null &&
                    this.uploaderClient.FindElements(By.LinkText("Delete")).Count > 1)
                {
                    try
                    {
                        this.uploaderClient.ScrollToTop();
                        this.uploaderClient.ClickOnElement(By.LinkText("Delete"));
                    }
                    catch
                    {
                        this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl04_lbPageLink"));
                        this.uploaderClient.ExecuteScript("Subforms['s_518'].deleteRow('_Input_518__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                    }
                }
            }

            this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl04_lbPageLink"));
            Thread.Sleep(400);

            var i = 0;

            foreach (var room in listing.Rooms)
            {
                if (i > 0)
                {
                    this.uploaderClient.ScrollDown(10000);
                    Thread.Sleep(400);
                    this.uploaderClient.ClickOnElement(By.Id("_Input_518_more"));
                    Thread.Sleep(400);
                }

                this.uploaderClient.SetSelect(By.Id("_Input_518__REPEAT" + i + "_566"), room.RoomType, "Type", tabName, false); // FieldName
                Thread.Sleep(400);

                i++;
            }
        }

        private void FillFeaturesInformation(DfwListingRequest listing)
        {
            var tabName = "Features";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetSelect(By.Id("Input_309"), listing.HasAccessibility.BoolToNumericBool(), "Accessibility Features YN", tabName); // Accessibility Features YN
            this.uploaderClient.SetSelect(By.Id("Input_342"), listing.IsSmartHome.BoolToNumericBool(), "Smart Home Features YN", tabName); // Smart Home Features YN
            this.uploaderClient.WriteTextbox(By.Id("Input_308"), listing.NumberFireplaces); // # Fireplaces
            this.uploaderClient.WriteTextbox(By.Id("Input_301"), listing.CarportCapacity); // # Carport Spaces

            this.uploaderClient.SetMultipleCheckboxById("Input_298", listing.AccessibilityDesc, "Accessibility Features", tabName); // Accessibility Features
            this.uploaderClient.SetSelect(By.Id("Input_300"), listing.HasPool.BoolToNumericBool(), "Pool on Property", tabName); // Pool on Property
            this.uploaderClient.SetMultipleCheckboxById("Input_297", listing.PoolDesc, "Pool Features", tabName); // Pool Features
            this.uploaderClient.SetMultipleCheckboxById("Input_299", listing.FireplaceDesc, "Fireplaces Features", tabName); // Fireplaces Features
            this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.GarageCapacity); // Garage Spaces
            this.uploaderClient.WriteTextbox(By.Id("Input_303"), listing.GarageCapacity); // # Covered Spaces (Total)

            this.uploaderClient.SetSelect(By.Id("Input_304"), listing.HasGarage.BoolToNumericBool(), "Garage YN", tabName);  // Garage YN
            this.uploaderClient.SetSelect(By.Id("Input_305"), listing.HasAttachedGarage.BoolToNumericBool(), "Garage YN", tabName);  // Attached Garage YN
            this.uploaderClient.SetMultipleCheckboxById("Input_244", listing.InteriorDesc, "Interior Features", tabName); // Interior Features
            this.uploaderClient.SetSelect(By.Id("Input_310"), "0", "Basement YN", tabName); // Basement YN
            this.uploaderClient.SetMultipleCheckboxById("Input_315", listing.AppliancesDesc, "Appliances", tabName); // Appliances
            this.uploaderClient.WriteTextbox(By.Id("Input_306"), listing.GarageLength); // Garage Length
            this.uploaderClient.WriteTextbox(By.Id("Input_307"), listing.GarageWidth); // Garage Width
            // Garage Height
            this.uploaderClient.SetMultipleCheckboxById("Input_311", listing.FoundationDesc, "Foundation", tabName); // Foundation
            this.uploaderClient.SetMultipleCheckboxById("Input_296", listing.GarageDesc, "Parking Features", tabName); // Parking Features
            this.uploaderClient.ScrollDown(1000);

            this.uploaderClient.SetMultipleCheckboxById("Input_245", listing.SecurityDesc, "Security Features", tabName); // Security Features
            if (listing.PropType != this.MultiFamilyPropType)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_317", listing.LaundryFacilityDesc, "Laundry Features", tabName); // Laundry Features
            }

            this.uploaderClient.SetMultipleCheckboxById("Input_312", listing.RoofDesc, "Roof", tabName); // Roof

            this.uploaderClient.SetMultipleCheckboxById("Input_320", string.Empty, "Special Notes", tabName); // Special Notes
            // Window Features
            this.uploaderClient.SetMultipleCheckboxById("Input_316", listing.PatioAndPorchFeatures, "Patio & Porch Features", tabName); // Patio & Porch Features
            this.uploaderClient.SetMultipleCheckboxById("Input_313", listing.FloorsDesc, "Flooring", tabName); // Flooring
            this.uploaderClient.SetMultipleCheckboxById("Input_319", listing.CommonFeatures, "Common Features", tabName); // Community Features
        }

        private void FillLotInformation(DfwListingRequest listing)
        {
            var tabName = "Lot Info";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.WriteTextbox(By.Id("Input_247"), listing.LotSizeAcres); // Acres
            this.uploaderClient.SetSelect(By.Id("Input_248"), "ACRE", "Lot Size Unit", tabName); // Lot Size Unit
            this.uploaderClient.SetMultipleCheckboxById("Input_325", listing.LotDesc, "Lot Features", tabName); // Lot Features
            this.uploaderClient.SetMultipleCheckboxById("Input_326", listing.ExteriorDesc, "Exterior Features", tabName); // Exterior Features
            this.uploaderClient.SetMultipleCheckboxById("Input_327", listing.Restrictions, "Restrictions", tabName); // Restrictions
            this.uploaderClient.SetMultipleCheckboxById("Input_354", listing.FenceDesc, "Type of Fence", tabName); // Type of Fence

            this.uploaderClient.WriteTextbox(By.Id("Input_249"), listing.LotDim); // Lot Dimensions
            this.uploaderClient.SetSelect(By.Id("Input_323"), listing.LotSize, "Lot Size/Acreage", tabName); // Lot Size/Acreage

            if (listing.PropType != this.MultiFamilyPropType)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_353", listing.Easements, "Easements", tabName); // Easements
                this.uploaderClient.SetMultipleCheckboxById("Input_355", listing.SoilType, "Soil", tabName); // Soil
            }
        }

        private void FillUtilitiesInformation(DfwListingRequest listing)
        {
            var tabName = "Utilities";

            try
            {
                this.GoToTab(tabName);
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

                Thread.Sleep(1000);

                this.uploaderClient.SetMultipleCheckboxById("Input_252", listing.UtilitiesDesc, "Street/Utilities", tabName); // Street/Utilities
                this.uploaderClient.SetMultipleCheckboxById("Input_362", listing.HeatSystemDesc, "Heating", tabName); // Heating
                this.uploaderClient.SetMultipleCheckboxById("Input_363", listing.CoolSystemDesc, "Cooling", tabName); // Cooling
                this.uploaderClient.SetSelect(By.Id("Input_364"), listing.HasMudDistrict.BoolToNumericBool(), "MUD District", tabName); // MUD District
                this.uploaderClient.SetSelect(By.Id("Input_436"), listing.IsSpecialTaxingAuthority.BoolToNumericBool(), "Special Taxing Authority YN", tabName); // Special Taxing Authority YN
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "An exception was occur in Utilities, {Message}", e.Message);
            }
        }

        private void FillEnvironmentInformation(DfwListingRequest listing)
        {
            var tabName = "Environment";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            try
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_329", listing.WaterDesc, "Green Water Features", tabName); // Green Water Features
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "An exception was occur in Environment, {Message}", e.Message);
            }

            try
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_332", listing.GreenFeatures, "Green Energy Features", tabName); // Green Energy Features
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "An exception was occur in Environment, {Message}", e.Message);
            }
        }

        private void FillFinancialInformation(DfwListingRequest listing)
        {
            var tabName = "Financial";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetMultipleCheckboxById("Input_254", listing.FinancingProposed, "Listing Terms", tabName); // Listing Terms
            this.uploaderClient.SetMultipleCheckboxById("Input_255", "CLOFUN", "Posession", tabName); // Posession
            this.uploaderClient.SetSelect(By.Id("Input_365"), "TRASCL", "Loan Type", tabName); // Loan Type
            this.uploaderClient.SetSelect(By.Id("Input_374"), "0", "Second  Mortgage YN", tabName); // Second Mortgage YN
            this.uploaderClient.WriteTextbox(By.Id("Input_377"), listing.TitleCo); // Title Company-Preferred
            this.uploaderClient.WriteTextbox(By.Id("Input_378"), listing.TitleCoPhone); // Title Company Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_379"), listing.TitleCoLocation); // Title Company Location

            this.uploaderClient.ScrollToTop();

            tabName = "HOA";
            this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl16_lbPageLink"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            this.uploaderClient.SetSelect(By.Id("Input_383"), listing.HOA, "HOA", tabName); // HOA
            this.uploaderClient.SetSelect(By.Id("Input_257"), listing.HoaTerm, "HOA Billing Freq", tabName); // HOA Billing Freq
            this.uploaderClient.WriteTextbox(By.Id("Input_382"), listing.AssocFee); // HOA Dues
            this.uploaderClient.WriteTextbox(By.Id("Input_384"), listing.AssocName); // HOA Management Co
            this.uploaderClient.WriteTextbox(By.Id("Input_480"), listing.AssocPhone); // HOA Managemt Co Phone
            this.uploaderClient.SetMultipleCheckboxById("Input_385", listing.AssocFeeIncludes, "HOA Includes", tabName); // HOA Includes
        }

        private void FillAgentOfficeInformation()
        {
            var tabName = "Agent/Office";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                var agentName = this.options.MarketInfo.Dfw.AgentId;
                if (!string.IsNullOrEmpty(agentName))
                {
                    foreach (var charact in agentName)
                    {
                        Thread.Sleep(400);
                        this.uploaderClient.FindElement(By.Id("Input_146_displayValue")).SendKeys(charact.ToString());
                    }

                    this.uploaderClient.FindElement(By.Id("Input_146_displayValue")).SendKeys(Keys.Tab);
                    this.uploaderClient.ExecuteScript("javascript:$('#Input_146_Refresh').val('changed');RefreshToSamePage();");
                    Thread.Sleep(1000);
                }

                var supervisorName = this.options.MarketInfo.Dfw.SupervisorId;
                if (!string.IsNullOrEmpty(supervisorName))
                {
                    foreach (var charact in supervisorName)
                    {
                        Thread.Sleep(400);
                        this.uploaderClient.FindElement(By.Id("Input_761_displayValue")).SendKeys(charact.ToString());
                    }

                    Thread.Sleep(1000);
                    this.uploaderClient.FindElement(By.Id("Input_761_displayValue")).SendKeys(Keys.Enter);
                    this.uploaderClient.ExecuteScript("javascript:$('#Input_761_Refresh').val('changed');RefreshToSamePage();");
                }
            }
        }

        private void FillShowingInformation(DfwListingRequest listing, bool isNotPartialFill = true)
        {
            var tabName = "Showing";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.WriteTextbox(By.Id("Input_396"), listing.AltPhoneCommunity); // Occupant Alternate Phone

            if (isNotPartialFill)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_380", "BUILDE", "Special Listing Conditions", tabName); // Special Listing Conditions
                this.uploaderClient.SetSelect(By.Id("Input_393"), "VACANT", "Occupant Type", tabName); // Occupant Type
                this.uploaderClient.SetMultipleCheckboxById("Input_391", listing.Showing, "Showing Requirements", tabName); // Showing Requirements
                this.uploaderClient.SetMultipleCheckboxById("Input_387", listing.ShowingContactType, "Showing Contact Type", tabName); // Showing Contact Type
                this.uploaderClient.WriteTextbox(By.Id("Input_493"), listing.OwnerName); // Owner Name
                this.uploaderClient.SetSelect(By.Id("Input_260"), "NONE", "Keybox Type", tabName); // Lockbox Type
                this.uploaderClient.WriteTextbox(By.Id("Input_388"), "0"); // Key Box Number
            }

            this.uploaderClient.ScrollDown(1000);

            var apptPhone = listing.AgentListApptPhone;

            if (listing.ListStatus != MarketStatuses.ComingSoon.ToStringFromEnumMember())
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_390"), !string.IsNullOrEmpty(apptPhone) ? apptPhone : string.Empty); // Appt Phone
                this.uploaderClient.WriteTextbox(By.Id("Input_258"), listing.ShowingInstructions); // Showing Instructions

                if (isNotPartialFill)
                {
                    this.uploaderClient.SetMultiSelect(By.Id("Input_387"), listing.ShowingContactType); // Showing Contact Type
                }
            }
            else
            {
                this.uploaderClient.SetSelect(By.Id("Input_260"), null, "Keybox Type", tabName); // Keybox Type
                // call for appoiment
                this.uploaderClient.WriteTextbox(By.Id("Input_390"), string.Empty); //  Appt Phone
                this.uploaderClient.WriteTextbox(By.Id("Input_258"), string.Empty); // Showing Instructions

                if (isNotPartialFill)
                {
                    this.uploaderClient.SetMultiSelect(By.Id("Input_387"), null); // Showing
                }
            }
        }

        private void FillRemarksInformation(DfwListingRequest listing)
        {
            var tabName = "Remarks";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetSelect(By.Id("Input_261"), "1", "Allow Address Display", tabName); // Allow Address Display
            this.uploaderClient.SetSelect(By.Id("Input_416"), "1", "Allow AVM", tabName); // Allow AVM
            this.uploaderClient.SetSelect(By.Id("Input_417"), "1", "Allow Internet Display", tabName); // Allow Internet Display
            this.uploaderClient.SetSelect(By.Id("Input_415"), "1", "Allow Comments/Reviews", tabName); // Allow Comments/Reviews

            this.UpdatePrivateRemarksInRemarksTab(listing);

            // Direction
            var direction = listing.Directions;
            if (!string.IsNullOrEmpty(direction))
            {
                direction = direction.RemoveSlash();
                int dirLen = direction.Length;
                if (direction.ElementAt(dirLen - 1) == '.')
                {
                    direction = direction.Remove(dirLen - 1);
                }
                else
                {
                    direction = direction + ".";
                }
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_262"), direction); // Allow Comments/Reviews

            this.UpdatePublicRemarksInRemarksTab(listing);

            // Intra Office
            var compSaleText = string.Empty;

            if ((listing.ListStatus == MarketStatuses.Pending.ToStringFromEnumMember()
                || listing.ListStatus == MarketStatuses.Sold.ToStringFromEnumMember())
                && this.uploaderClient.UploadInformation?.IsNewListing != null
                && this.uploaderClient.UploadInformation.IsNewListing)
            {
                compSaleText = "M-";
            }

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_266"), (!string.IsNullOrWhiteSpace(listing.PlanProfileName) ? (compSaleText + listing.PlanProfileName) : (compSaleText + " ")).RemoveSlash());
            }

            // Excludes
            this.uploaderClient.WriteTextbox(By.Id("Input_264"), listing.Excludes.RemoveSlash()); // Excludes
        }

        private void FillStatusInformation(DfwListingRequest listing)
        {
            var tabName = "Status";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetSelect(By.Id("Input_478"), listing.ListStatus, "Status", tabName); // Status
        }

        private void GoToPropertyInformationTab()
        {
            this.GoToTab("Property Info");
        }

        private void GoToManageTourLinks()
        {
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Virtual Tours/URLs"));
            this.uploaderClient.FindElement(By.LinkText("Virtual Tours/URLs")).Click();
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
                this.logger.LogInformation("Skipping media upload for existing listing {ListingId}", listing.ResidentialListingID);
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

        private void UpdatePrivateRemarksInRemarksTab(DfwListingRequest listing)
        {
            this.uploaderClient.WriteTextbox(By.Id("Input_265"), string.Empty);
            this.uploaderClient.WriteTextbox(By.Id("Input_265"), listing.GetAgentRemarksMessage(listing.YearBuiltDesc));
        }

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_263"));
            this.uploaderClient.WriteTextbox(By.Id("Input_263"), listing.GetPublicRemarks());
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
            var media = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: this.CurrentMarket, cancellationToken);
            var imageOrder = 0;
            var imageRow = 0;
            var imageCell = 0;
            var mediaFolderName = "Husa.Core.Uploader";
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            var captionImageId = string.Empty;
            foreach (var image in media)
            {
                captionImageId = $"m_rptPhotoRows_ctl{imageRow:D2}_m_rptPhotoCells_ctl{imageCell:D2}_m_ucPhotoCell_m_tbxDescription";

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_ucImageLoader_m_tblImageLoader"), cancellationToken);

                await this.mediaRepository.PrepareImage(image, MarketCode.Houston, cancellationToken, folder);
                this.uploaderClient.FindElement(By.Id("m_ucImageLoader_m_tblImageLoader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                this.WaitForElementAndThenDoAction(
                        By.Id(captionImageId),
                        (element) =>
                        {
                            if (!string.IsNullOrEmpty(image.Caption))
                            {
                                this.uploaderClient.ExecuteScript(script: $"jQuery('#{captionImageId}').val('{image.Caption.Replace("'", "\\'")}');");
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
            Thread.Sleep(1000);
            int countDeleteButtons = this.uploaderClient.FindElements(By.LinkText("Delete")).Count;
            for (int i = 0; i < countDeleteButtons; i++)
            {
                try
                {
                    this.uploaderClient.ScrollToTop();
                    try
                    {
                        this.uploaderClient.ExecuteScript("Subforms['s_168'].deleteRow('_Input_168__del_REPEAT" + i + "_');");
                    }
                    catch (Exception e)
                    {
                        this.logger.LogInformation(e, "The delete open house object was not displayed in the login screen., {Message}", e.Message);
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogInformation(e, "The delete open house link was not displayed in the login screen., {Message}", e.Message);
                }
            }

            this.uploaderClient.ScrollDown(5000);
            Thread.Sleep(2000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lbSubmit"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.Id("m_lbSubmit"));
            Thread.Sleep(2000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lblInputCompletedMessage"));
            this.uploaderClient.ClickOnElement(By.Id("m_lbContinueEdit"));
            Thread.Sleep(2000);
        }

        private void AddOpenHouses(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            var index = 0;
            var sortedOpenHouses = listing.OpenHouse.OrderBy(openHouse => openHouse.Date).ToList();
            foreach (var openHouse in sortedOpenHouses)
            {
                if (index != 0)
                {
                    this.uploaderClient.ScrollDown();
                    this.uploaderClient.ClickOnElementById(elementId: $"_Input_168_more");
                    Thread.Sleep(1000);
                    this.uploaderClient.ScrollDown();
                }

                // Open House Type
                this.uploaderClient.SetSelect(By.Id($"_Input_168__REPEAT{index}_161"), value: "PUBLIC");

                // Date
                this.uploaderClient.WriteTextbox(By.Id($"_Input_168__REPEAT{index}_162"), entry: openHouse.Date);

                // From Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_168__REPEAT{index}_TextBox_163"), entry: openHouse.StartTime.To12Format());
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_168__REPEAT{index}_RadioButtonList_163_{fromTimeTT}", shouldWait: true, waitTime: 5);

                // To Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_168__REPEAT{index}_TextBox_164"), entry: openHouse.EndTime.To12Format());
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_168__REPEAT{index}_RadioButtonList_164_{endTimeTT}", shouldWait: true, waitTime: 5);

                // Open House Status
                this.uploaderClient.SetSelect(By.Id($"_Input_168__REPEAT{index}_165"), value: openHouse.Active ? "ACT" : "END");

                // Refreshments
                this.uploaderClient.WriteTextbox(By.Id($"_Input_168__REPEAT{index}_166"), entry: openHouse.Refreshments);

                // Comments
                this.uploaderClient.WriteTextbox(By.Id($"_Input_168__REPEAT{index}_167"), entry: openHouse.Comments);

                index++;
            }
        }

        private void NavigateToTab(string tabName)
        {
            this.uploaderClient.ClickOnElement(By.LinkText(tabName));
            this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(800));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));
            this.uploaderClient.ResetImplicitWait();
        }

        private string TransformStatus(string status, ref string linkText)
        {
            switch (status)
            {
                case "ACT":
                    linkText = "Change to Active";
                    return "ACT";
                case "AC":
                    linkText = "Change to Active Contingent";
                    return "AC";
                case "AKO":
                    linkText = "Change to Active Kick Out";
                    return "AKO";
                case "AOC":
                    linkText = "Change to Active Option Contract";
                    return "AOC";
                case "CAN":
                    linkText = "Change to Cancelled";
                    return "CAN";
                case "PND":
                    linkText = "Change to Pending";
                    return "PND";
                case "SLD":
                    linkText = "Change to Closed";
                    return "SLD";
                case "HOLD":
                    linkText = "Change to Hold";
                    return "TOM";
                default:
                    this.logger.LogInformation("The status '{Status} ' is not configured for DFW", status);
                    return null;
            }
        }
    }
}
