namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Dfw.Domain.Enums.Domain;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Core.Services.Common;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
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
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
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
            catch
            {
                this.logger.LogInformation("The redirect popup was not displayed in the login screen.");
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
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                if (!listing.IsNewListing)
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UploadListing();

            async Task<UploadResult> UploadListing()
            {
                this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(2000);

                try
                {
                    var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();

                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput(housingType);
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    }

                    this.FillPropertyInformation(listing as DfwListingRequest);
                    this.FillLocationSchools(listing);
                    this.FillRoomsInformation(listing);
                    this.FillFeaturesInformation(listing);
                    this.FillLotInformation(listing);
                    this.FillUtilitiesInformation(listing);
                    this.FillEnvironmentInformation(listing);
                    this.FillFinancialInformation(listing as DfwListingRequest);
                    this.FillAgentOfficeInformation(listing);
                    this.FillShowingInformation(listing);
                    this.FillRemarksInformation(listing as DfwListingRequest);

                    if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
                    {
                        this.FillStatusInformation(listing);
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingCompletionDate();

            async Task<UploadResult> UpdateListingCompletionDate()
            {
                this.logger.LogInformation("Updating CompletionDate for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                this.GoToPropertyInformationTab();
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_403"));
                this.uploaderClient.SetSelect(By.Id("Input_403"), listing.YearBuiltDesc); // Construction Status
                this.uploaderClient.WriteTextbox(By.Id("Input_231"), listing.YearBuilt); // Year Built

                this.GoToTab("Remarks");
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_263"));
                this.uploaderClient.WriteTextbox(By.Id("Input_263"), listing.GetPublicRemarks()); // Property Description

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
                this.logger.LogInformation("Updating media for the listing {requestId}", listing.ResidentialListingRequestID);
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

        public Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingPrice();

            async Task<UploadResult> UpdateListingPrice()
            {
                this.logger.LogInformation("Updating the price of the listing {requestId} to {listPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Price Change"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Price Change"));
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_77"), cancellationToken);
                this.uploaderClient.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price
                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingStatus();

            async Task<UploadResult> UpdateListingStatus()
            {
                this.logger.LogInformation("Editing the status information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                await this.Login(listing.CompanyId);

                Thread.Sleep(1000);
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
                        this.uploaderClient.WriteTextbox(By.Id("Input_457"), listing.SellerBuyerCost); // Seller Contribution
                        this.uploaderClient.WriteTextbox(By.Id("Input_233"), listing.SqFtTotal); // Living Area

                        this.uploaderClient.WriteTextbox(By.Id("Input_85"), listing.ClosedDate.Value.ToShortDateString()); // Close Date
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.PendingDate.Value.ToShortDateString()); // Purchase Contract Date

                        this.uploaderClient.SetSelect(By.Id("Input_460"), "0"); // Third Party Assistance Program
                        this.uploaderClient.SetSelect(By.Id("Input_234"), listing.SqFtSource); // Living Area Source
                        this.uploaderClient.SetSelect(By.Id("Input_496"), listing.MFinancing); // Buyer Financing
                        //// 1st Term in Years
                        //// 1st Loan Amount
                        //// 1st Interest Rate
                        this.uploaderClient.WriteTextbox(By.Id("Input_467"), listing.MortgageCoSold); // Mortgage Company
                        this.uploaderClient.WriteTextbox(By.Id("Input_468"), listing.TitleCo); // Closing Title Company
                        //// Buyers/SubAgent Texting Allowed
                        //// Buyers/SubAgent2 Texting Allowed
                        this.uploaderClient.WriteTextbox(By.Id("Input_141_displayValue"), listing.SellingAgentLicenseNum ?? "99999999"); // Buyers/SubAgent ID
                        this.uploaderClient.WriteTextbox(By.Id("Input_145_displayValue"), listing.SellingAgent2ID); // Buyers/SubAgent 2 ID
                        this.uploaderClient.WriteTextbox(By.Id("Input_708"), listing.SellTeamID); // Sell Team ID
                        this.uploaderClient.WriteTextbox(By.Id("Input_748"), listing.SellingAgentSupervisor); // Buyer Supervisor ID
                        break;
                    case "PND":
                        var pendingDate = listing.ContractDate == null ? string.Empty : listing.ContractDate.Value.ToShortDateString();
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), pendingDate); // Contract Date
                        break;
                    case "A":
                    case "ACT":
                        var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_5"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_5"), expirationDate);
                        break;
                    case "AC":
                    case "AKO":
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.ContractDate); // Contract Date
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_451")); // Kick Out Information
                        break;
                    case "AOC":
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_94"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.ContractDate); // Contract Date
                        this.uploaderClient.WriteTextbox(By.Id("Input_453"), listing.ExpiredDateOption); // Option Expire Date
                        break;
                    case "CAN":
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_472"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_472"), DateTime.Now.ToShortDateString());
                        break;
                    case "W":
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_476"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_476"), listing.WithdrawnDate); // Withdrawn Date
                        break;
                    case "TOM":
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_474"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_474"), listing.OffMarketDate);
                        break;
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
                this.logger.LogInformation("Updating VirtualTour for the listing {requestId}", listing.ResidentialListingRequestID);
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
                this.logger.LogInformation("Editing the information of Open House for the listing {requestId}", listing.ResidentialListingRequestID);
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
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_dlInputList_ctl02_m_btnSelect"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.Id("m_dlInputList_ctl02_m_btnSelect"));
                Thread.Sleep(3000);

                this.CleanOpenHouse();
                this.AddOpenHouses(listing);
                return UploadResult.Success;
            }
        }

        private async Task UpdateVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            var virtualTours = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.Houston, cancellationToken);
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

        private void NavigateToNewPropertyInput(HousingType? housingType)
        {
            this.uploaderClient.NavigateToUrl(NavigateToUrlNewListing);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Add new"));
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));

            switch (housingType)
            {
                case HousingType.SingleDetached: //// .SingleFamily: REMOVE
                    WaitAndClick("Single-Family Add/Edit");
                    break;
                case HousingType.GardenZeroLotLine: ////.CountryHomesAcreage: REMOVE
                    WaitAndClick("Country Homes/Acreage Add/Edit");
                    break;
                case HousingType.CondoTownhome:
                    WaitAndClick("Townhouse/Condo Add/Edit");
                    break;
                case HousingType.AttachedDuplex: ////.MultiFamily: REMOVE
                    WaitAndClick("Multi-Family Add/Edit");
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

        private void FillPropertyInformation(DfwListingRequest listing)
        {
            this.GoToPropertyInformationTab();

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            this.uploaderClient.SetSelect(By.Id("Input_219"), value: listing.PropSubType);
            this.uploaderClient.SetSelect(By.Id("Input_223"), value: "EXCAG"); // Listing Agreement Type
            this.uploaderClient.SetSelect(By.Id("Input_224"), value: "FS"); // Transaction Type
            this.uploaderClient.WriteTextbox(By.Name("Input_225"), string.Empty); // Lease MLS#

            this.uploaderClient.SetMultipleCheckboxById("Input_220", listing.HousingTypeDesc); // Housing Type
            this.uploaderClient.SetMultipleCheckboxById("Input_226", listing.HousingStyleDesc); // Architectural Style
            this.uploaderClient.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                DateTime listDate = DateTime.Now;
                switch (listing.ListStatus)
                {
                    case "A":
                    case "ACT":
                    case "AC":
                    case "AKO":
                    case "AOC":
                    case "CS":
                        listDate = DateTime.Now;
                        break;
                    case "P":
                    case "PND":
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case "S":
                    case "SLD":
                        listDate = DateTime.Now.AddDays(ListDateSold);
                        break;
                }

                this.uploaderClient.WriteTextbox(By.Id("Input_80"), listDate.Date.ToShortDateString()); // List Date
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_81"), DateTime.Today.AddYears(1).ToShortDateString()); // Expire Date

            this.uploaderClient.WriteTextbox(By.Id("Input_231"), listing.YearBuilt); // Year Built

            this.uploaderClient.SetSelect(By.Id("Input_381"), value: "NO"); // Will Subdivide
            this.uploaderClient.SetMultipleCheckboxById("Input_228", listing.ConstructionDesc); // Construction Material

            this.uploaderClient.SetSelect(By.Id("Input_403"), value: listing.YearBuiltDesc); // Year Built Details/Construc. Status
            this.uploaderClient.WriteTextbox(By.Id("Input_233"), listing.SqFtTotal); // SqFt/Living Area

            this.uploaderClient.SetSelect(By.Id("Input_234"), value: listing.SqFtSource); // SqFt Source
            this.uploaderClient.WriteTextbox(By.Id("Input_235"), string.IsNullOrWhiteSpace(listing.TaxID) ? "NA" : listing.TaxID); // Parcel ID
            this.uploaderClient.SetSelect(By.Id("Input_237"), value: "0"); // Multi Parcel ID YN
            Thread.Sleep(500);
        }

        private void FillLocationSchools(ResidentialListingRequest listing)
        {
            this.GoToTab("Location/Schools");

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.StreetNum); // Street/Box Number
                this.uploaderClient.SetSelect(By.Id("Input_285"), value: listing.County); // County
                this.uploaderClient.SetSelect(By.Id("Input_280"), value: listing.StreetDir); // Street Direction
                this.uploaderClient.WriteTextbox(By.Id("Input_170"), listing.StreetName?.Replace('\'', ' ')); // Street Name

                if (!string.IsNullOrWhiteSpace(listing.StreetType))
                {
                    this.uploaderClient.SetSelect(By.Id("Input_281"), value: listing.StreetType); // Street Type
                }
            }

            this.uploaderClient.SetSelect(By.Id("Input_282"), value: listing.StreetSuffixFQ); // Street Directional Suffix

            this.uploaderClient.WriteTextbox(By.Id("Input_288"), !string.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : string.Empty); // Lot
            this.uploaderClient.WriteTextbox(By.Id("Input_289"), !string.IsNullOrEmpty(listing.Block) ? listing.Block : string.Empty); // Block

            this.uploaderClient.WriteTextbox(By.Id("Input_283"), listing.UnitNum); // Unit #
            this.uploaderClient.WriteTextbox(By.Id("Input_294"), listing.Zip); // Zip
            this.uploaderClient.WriteTextbox(By.Id("Input_295"), string.Empty); // Zip + 4

            this.uploaderClient.WriteTextbox(By.Id("Input_290"), listing.Subdivision); // Subdivision
            this.uploaderClient.WriteTextbox(By.Id("Input_292"), listing.PlannedDevelopment); // Planned Development
            this.uploaderClient.WriteTextbox(By.Id("Input_293"), listing.Legal); // Additional Legal

            this.uploaderClient.ScrollDown(1000);

            this.SetLongitudeAndLatitudeValues(listing);

            Thread.Sleep(500);

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_335"), value: listing.SchoolName1); // Elementary School

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_339"), value: listing.SchoolName2); // Middle School

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_340"), value: listing.SchoolName4); // High School ////.SchoolName3

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_341"), value: listing.SchoolName7); // Intermediate School

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_336"), value: listing.SchoolName5); // Junior School

            Thread.Sleep(500);
            this.uploaderClient.SetSelect(By.Id("Input_337"), value: listing.SchoolName6); // Senior High School
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if ((listing.IsForLease == "Yes" && string.IsNullOrEmpty(listing.MLSNum)) || string.IsNullOrEmpty(listing.MLSNum))
            {
                this.uploaderClient.WriteTextbox(By.Id("INPUT__146"), listing.Latitude); // Latitude
                this.uploaderClient.WriteTextbox(By.Id("INPUT__168"), listing.Longitude); // Longitude
            }
        }

        private void FillRoomsInformation(ResidentialListingRequest listing)
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

        private void FillFeaturesInformation(ResidentialListingRequest listing)
        {
            var tabName = "Features";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetSelect(By.Id("Input_309"), listing.HasHandicapAmenities, "Accessibility Features YN", tabName); // Accessibility Features YN
            this.uploaderClient.SetSelect(By.Id("Input_342"), listing.SMARTFEATURESAPP, "Smart Home Features YN", tabName); // Smart Home Features YN
            this.uploaderClient.WriteTextbox(By.Id("Input_308"), listing.NumberFireplaces); // # Fireplaces
            this.uploaderClient.WriteTextbox(By.Id("Input_301"), listing.CarportCapacity); // # Carport Spaces

            this.uploaderClient.SetMultipleCheckboxById("Input_298", listing.HandicapDesc, "Accessibility Features", tabName); // Accessibility Features
            this.uploaderClient.SetSelect(By.Id("Input_300"), listing.HasPool ? "1" : "0", "Pool on Property", tabName); // Pool on Property
            this.uploaderClient.SetMultipleCheckboxById("Input_297", listing.PoolDesc, "Pool Features", tabName); // Pool Features
            this.uploaderClient.SetMultipleCheckboxById("Input_299", listing.FireplaceDesc, "Fireplaces Features", tabName); // Fireplaces Features
            this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.GarageCapacity); // Garage Spaces
            this.uploaderClient.WriteTextbox(By.Id("Input_303"), listing.GarageCapacity); // # Covered Spaces (Total)

            this.uploaderClient.SetSelect(By.Id("Input_304"), listing.Furnished.ToString(), "Garage YN", tabName);  // Garage YN
            this.uploaderClient.SetMultipleCheckboxById("Input_244", listing.InteriorDesc, "Interior Features", tabName); // Interior Features
            this.uploaderClient.SetSelect(By.Id("Input_310"), "0", "Basement YN", tabName); // Basement YN
            this.uploaderClient.SetMultipleCheckboxById("Input_315", listing.KitchenEquipmentDesc, "Appliances", tabName); // Appliances
            this.uploaderClient.WriteTextbox(By.Id("Input_306"), listing.GarageLength); // Garage Length
            this.uploaderClient.WriteTextbox(By.Id("Input_307"), listing.GarageWidth); // Garage Width
            // Garage Height
            this.uploaderClient.SetMultipleCheckboxById("Input_311", listing.FoundationDesc, "Foundation", tabName); // Foundation
            this.uploaderClient.SetMultipleCheckboxById("Input_296", listing.GarageDesc, "Parking Features", tabName); // Parking Features
            this.uploaderClient.ScrollDown(1000);

            this.uploaderClient.SetMultipleCheckboxById("Input_245", listing.SecurityDesc, "Security Features", tabName); // Security Features
            this.uploaderClient.SetMultipleCheckboxById("Input_317", listing.LaundryLocDesc, "Laundry Features", tabName); // Laundry Features
            this.uploaderClient.SetMultipleCheckboxById("Input_312", listing.RoofDesc, "Roof", tabName); // Roof

            this.uploaderClient.SetMultipleCheckboxById("Input_320", string.Empty, "Special Notes", tabName); // Special Notes
            // Window Features
            this.uploaderClient.SetMultipleCheckboxById("Input_316", listing.ExteriorDesc, "Patio & Porch Features", tabName); // Patio & Porch Features
            this.uploaderClient.SetMultipleCheckboxById("Input_313", listing.FloorsDesc, "Flooring", tabName); // Flooring
            this.uploaderClient.SetMultipleCheckboxById("Input_319", listing.CommonFeatures, "Common Features", tabName); // Community Features
        }

        private void FillLotInformation(ResidentialListingRequest listing)
        {
            var tabName = "Lot Info";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.WriteTextbox(By.Id("Input_247"), listing.LotSizeAcres); // Acres .Acres
            this.uploaderClient.SetSelect(By.Id("Input_248"), "ACRE", "Lot Size Unit", tabName); // Lot Size Unit
            this.uploaderClient.SetMultipleCheckboxById("Input_325", listing.LotDesc, "Lot Features", tabName); // Lot Features
            this.uploaderClient.SetMultipleCheckboxById("Input_326", listing.ExteriorDesc, "Exterior Features", tabName); // Exterior Features
            this.uploaderClient.SetMultipleCheckboxById("Input_327", listing.Restrictions, "Restrictions", tabName); // Restrictions

            this.uploaderClient.WriteTextbox(By.Id("Input_249"), listing.LotDim); // Lot Dimensions
            this.uploaderClient.SetSelect(By.Id("Input_323"), listing.LotSize, "Lot Size/Acreage", tabName); // Lot Size/Acreage

            this.uploaderClient.SetMultipleCheckboxById("Input_353", listing.Easements, "Easements", tabName); // Easements
            this.uploaderClient.SetMultipleCheckboxById("Input_354", listing.FenceDesc, "Type of Fence", tabName); // Type of Fence
            this.uploaderClient.SetMultipleCheckboxById("Input_355", listing.SoilType, "Soil", tabName); // Soil
        }

        private void FillUtilitiesInformation(ResidentialListingRequest listing)
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
                this.uploaderClient.SetSelect(By.Id("Input_364"), listing.MUDDistrict, "MUD District", tabName); // MUD District
            }
            catch (Exception e)
            {
                this.logger.LogInformation("An exception was occur in Utilities, {message}", e.Message);
            }
        }

        private void FillEnvironmentInformation(ResidentialListingRequest listing)
        {
            var tabName = "Environment";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            try
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_329", listing.GreenCerts, "Green Energy Features", tabName); // Green Water Features
            }
            catch (Exception e)
            {
                this.logger.LogInformation("An exception was occur in Environment, {message}", e.Message);
            }

            try
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_332", listing.GreenFeatures, "Green Energy Features", tabName); // Green Energy Features
            }
            catch (Exception e)
            {
                this.logger.LogInformation("An exception was occur in Environment, {message}", e.Message);
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
            this.uploaderClient.SetSelect(By.Id("Input_257"), listing.AssocFeePaid, "HOA Billing Freq", tabName); // HOA Billing Freq
            this.uploaderClient.WriteTextbox(By.Id("Input_382"), listing.AssocFee); // HOA Dues
            this.uploaderClient.WriteTextbox(By.Id("Input_384"), listing.AssocName); // HOA Management Co
            this.uploaderClient.WriteTextbox(By.Id("Input_480"), listing.AssocPhone); // HOA Managemt Co Phone
            this.uploaderClient.SetMultipleCheckboxById("Input_385", listing.AssocFeeIncludes, "HOA Includes", tabName); // HOA Includes
        }

        private void FillAgentOfficeInformation(ResidentialListingRequest listing)
        {
            var tabName = "Agent/Office";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            var agentID = listing.BrokerName + " (" + listing.SellingAgentLicenseNum + ")"; //// .BrokerLicenseNum

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                if (listing.SysCompletedBy == 1295) //// .SysOwnedBy
                {
                    agentID = "Dennis Ciani (0306362)";
                }

                if (listing.SysCompletedBy == 1356) //// .SysOwnedBy
                {
                    agentID = "Ben Caballero (0096651)";
                }

                var aganetName = agentID.ToArray();

                foreach (var charact in aganetName)
                {
                    Thread.Sleep(400);
                    this.uploaderClient.FindElement(By.Id("Input_146_displayValue")).SendKeys(charact.ToString());
                }

                this.uploaderClient.FindElement(By.Id("Input_146_displayValue")).SendKeys(Keys.Tab);
                this.uploaderClient.ExecuteScript("javascript:$('#Input_146_Refresh').val('changed');RefreshToSamePage();");
                Thread.Sleep(1000);

                var supervisorName = "Ben Caballero (0096651)".ToArray();

                if (listing.SysCompletedBy == 1295) //// .SysOwnedBy
                {
                    supervisorName = "Dennis Ciani (0306362)".ToArray();
                }

                foreach (var charact in supervisorName)
                {
                    Thread.Sleep(400);
                    this.uploaderClient.FindElement(By.Id("Input_761_displayValue")).SendKeys(charact.ToString());
                }

                Thread.Sleep(1000);
                this.uploaderClient.FindElement(By.Id("Input_761_displayValue")).SendKeys(Keys.Enter);
                this.uploaderClient.ExecuteScript("javascript:$('#Input_761_Refresh').val('changed');RefreshToSamePage();");
            }

            this.uploaderClient.ScrollDown();
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            var tabName = "Showing";
            this.GoToTab(tabName);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            Thread.Sleep(1000);

            this.uploaderClient.SetMultipleCheckboxById("Input_380", "BUILDE", "Special Listing Conditions", tabName); // Special Listing Conditions
            this.uploaderClient.SetSelect(By.Id("Input_393"), "VACANT", "Occupant Type", tabName); // Occupant Type
            this.uploaderClient.WriteTextbox(By.Id("Input_396"), listing.AltPhoneCommunity); // Occupant Alternate Phone
            this.uploaderClient.SetMultipleCheckboxById("Input_391", listing.Showing, "Showing Requirements", tabName); // Showing Requirements
            this.uploaderClient.SetMultipleCheckboxById("Input_387", listing.ProposedTerms, "Showing Contact Type", tabName); // Showing Contact Type
            this.uploaderClient.WriteTextbox(By.Id("Input_493"), listing.OwnerName); // Owner Name
            this.uploaderClient.SetSelect(By.Id("Input_260"), "NONE", "Keybox Type", tabName); // Lockbox Type
            this.uploaderClient.WriteTextbox(By.Id("Input_388"), "0"); // Key Box Number

            this.uploaderClient.ScrollDown(1000);

            var realtorContactEmail = string.Empty;
            if (!string.IsNullOrEmpty(listing.AgentListEmail)) //// .ContactEmailFromCompany
            {
                realtorContactEmail = listing.AgentListEmail; //// .ContactEmailFromCompany
            }
            else if (!string.IsNullOrEmpty(listing.RealtorContactEmail))
            {
                realtorContactEmail = listing.RealtorContactEmail;
            }

            // UP-78
            realtorContactEmail =
                (!string.IsNullOrWhiteSpace(realtorContactEmail) &&
                listing.ShowingInstructions != null &&
                !listing.ShowingInstructions.RemoveSlash().ToLower().Contains("email contact") &&
                !listing.ShowingInstructions.RemoveSlash().ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : string.Empty;

            var message = listing.ShowingInstructions.RemoveSlash() + realtorContactEmail;

            var builtNote = string.Empty;
            if (listing.YearBuiltDesc == "NCI" &&
                !string.IsNullOrEmpty(message) &&
                !message.Contains("under construction"))
            {
                builtNote = "Home is under construction. For your safety, call appt number for showings. ";
            }

            var apptPhone = listing.AgentListApptPhone;

            if (listing.ListStatus != "CS")
            {
                this.uploaderClient.SetMultiSelect(By.Id("Input_387"), listing.Showing); // Showing
                this.uploaderClient.WriteTextbox(By.Id("Input_390"), !string.IsNullOrEmpty(apptPhone) ? apptPhone : string.Empty); // Appt Phone

                this.uploaderClient.WriteTextbox(By.Id("Input_258"), builtNote + message); // Showing Instructions
            }
            else
            {
                this.uploaderClient.SetSelect(By.Id("Input_260"), null, "Keybox Type", tabName); // Keybox Type
                // call for appoiment
                this.uploaderClient.WriteTextbox(By.Id("Input_390"), string.Empty); //  Appt Phone

                this.uploaderClient.WriteTextbox(By.Id("Input_258"), string.Empty); // Showing Instructions

                this.uploaderClient.SetMultiSelect(By.Id("Input_387"), null); // Showing
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

            if ((listing.ListStatus == "PND" || listing.ListStatus == "SLD") && this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                compSaleText = "M-";
            }

            if (this.uploaderClient.UploadInformation?.IsNewListing != null && this.uploaderClient.UploadInformation.IsNewListing)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_82"), (!string.IsNullOrWhiteSpace(listing.PlanProfileName) ? (compSaleText + listing.PlanProfileName) : (compSaleText + " ")).RemoveSlash());
            }

            // Excludes
            this.uploaderClient.WriteTextbox(By.Id("Input_264"), listing.Excludes.RemoveSlash()); // Excludes
        }

        private void FillStatusInformation(ResidentialListingRequest listing)
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

        private void UpdatePrivateRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            // 1. BonusCheckBox
            // 2. BonusWAmountCheckBox
            // 3. BuyerCheckBox
            var bonusMessage = string.Empty;
            if (listing.HasBonusWithAmount.Equals(true) && listing.BuyerCheckBox.Equals(true)) //// BonusWAmountCheckBox
            {
                bonusMessage = bonusMessage + listing.ApplicationFeePay + "Agent Bonus. Possible Buyer Incentive; ";
            }
            else if (listing.BuyerCheckBox.Equals(true))
            {
                bonusMessage = "Possible Buyer Incentives; ";
            }

            if (listing.HasBonusWithAmount.Equals(true)) ////BonusWAmountCheckBox
            {
                bonusMessage = bonusMessage + listing.ApplicationFeePay + " Agent Bonus; ";
            }

            if (listing.BuyerCheckBox.Equals(true) || listing.HasBonusWithAmount.Equals(true)) ////BonusWAmountCheckBox
            {
                bonusMessage += "ask Builder for details. ";
            }

            // BEGIN UP-73
            var realtorContactEmail = string.Empty;
            if (!string.IsNullOrEmpty(listing.AgentListEmail)) //// EmailRealtorsContact
            {
                realtorContactEmail = listing.AgentListEmail; //// EmailRealtorsContact
            }
            else if (!string.IsNullOrEmpty(listing.RealtorContactEmail))
            {
                realtorContactEmail = listing.RealtorContactEmail;
            }
            //// END UP-73

            // UP-78
            realtorContactEmail =
                (!string.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.PrivateRemarks).ToLower().Contains("email contact") && //// .GetPrivateRemarks(false)
                !(bonusMessage + listing.PrivateRemarks).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : string.Empty; //// .GetPrivateRemarks(false)

            this.uploaderClient.WriteTextbox(By.Id("Input_265"), string.Empty);
            this.uploaderClient.WriteTextbox(By.Id("Input_265"), bonusMessage + listing.PrivateRemarks + realtorContactEmail); //// .GetPrivateRemarks(false)
        }

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_263"));
            this.uploaderClient.WriteTextbox(By.Id("Input_263"), listing.GetPublicRemarks()); // Property Description ////.GetPublicRemarks(status)
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

        private void CleanOpenHouse()
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
        }

        private void AddOpenHouses(ResidentialListingRequest listing)
        {
            var index = 0;
            Thread.Sleep(1000);
            foreach (var openHouse in listing.OpenHouse)
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
                case "A":
                case "ACT":
                    linkText = "Change to Active";
                    return "A";
                case "AC":
                    linkText = "Change to Active Contingent";
                    return "AC";
                case "AKO":
                    linkText = "Change to Active Kick Out";
                    return "AKO";
                case "AOC":
                    linkText = "Change to Active Option Contract";
                    return "AOC";
                case "C":
                case "CAN":
                    linkText = "Change to Cancelled";
                    return "CAN";
                case "X":
                    return "EXP";
                case "P":
                case "PND":
                    linkText = "Change to Pending";
                    return "PND";
                case "S":
                case "SLD":
                    linkText = "Change to Closed";
                    return "SLD";
                case "T":
                case "WS":
                case "HOLD":
                    linkText = "Change to Hold";
                    return "TOM";
                case "W":
                    linkText = "Change to Withdrawn";
                    return "W";
                default:
                    this.logger.LogError(@"The status '" + status + @"' is not configured for DFW");
                    return null;
            }
        }
    }
}
