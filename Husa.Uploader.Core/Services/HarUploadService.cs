namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.Common.Exceptions;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums.Domain;
    using Husa.Uploader.Core.Interfaces;
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

    public class HarUploadService : IHarUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IListingRequestRepository sqlDataLoader;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<HarUploadService> logger;

        public HarUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IListingRequestRepository sqlDataLoader,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<HarUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.sqlDataLoader = sqlDataLoader ?? throw new ArgumentNullException(nameof(sqlDataLoader));
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

            // Connect to the login page
            var loginButtonId = "login_btn";
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id(loginButtonId));

            this.uploaderClient.WriteTextbox(By.Id("username"), credentials[LoginCredentials.Username]);
            this.uploaderClient.WriteTextbox(By.Id("password"), credentials[LoginCredentials.Password]);
            Thread.Sleep(1000);
            this.uploaderClient.ClickOnElementById(loginButtonId);

            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("RedirectingPopup"));
            Thread.Sleep(4000);
            this.uploaderClient.NavigateToUrl("https://www.har.com/moa_mls/goMatrix");
            Thread.Sleep(1000);

            return LoginResult.Logged;
        }

        public UploadResult Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Har.LogoutUrl);
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
                this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(2000);

                try
                {
                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput(listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>());
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    }

                    this.FillListingInformation(listing);
                    this.FillMapInformation(listing as HarListingRequest);
                    this.FillPropertyInformation(listing as HarListingRequest);
                    this.FillRoomInformation(listing);
                    this.FillFinancialInformation(listing as HarListingRequest);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing as HarListingRequest);

                    await this.UpdateVirtualTour(listing, cancellationToken);
                    await this.FillMedia(listing, cancellationToken);
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
                this.logger.LogInformation("Updating CompletionDate for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                this.GoToPropertyInformationTab();
                this.UpdateYearBuiltDescription(listing);

                this.GoToRemarksTab();
                this.UpdatePublicRemarksInRemarksTab(listing);

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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken) ?? throw new NotFoundException<ResidentialListingRequest>(listing.ResidentialListingRequestID);
                this.logger.LogInformation("Updating the price of the listing {requestId} to {listPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Price Change"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Price Change"));
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
                this.logger.LogInformation("Editing the status information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                await this.Login(listing.CompanyId);

                Thread.Sleep(1000);
                this.NavigateToQuickEdit(listing.MLSNum);

                Thread.Sleep(1000);
                var buttonText = string.Empty;
                switch (listing.ListStatus)
                {
                    case "Hold":
                        buttonText = "Change to Hold";
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_528"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_528"), listing.OffMarketDate.Value.ToShortDateString()); // Hold Date
                        this.uploaderClient.WriteTextbox(By.Id("Input_81"), listing.BackOnMarketDate.Value.ToShortDateString()); // Expiration Date
                        break;
                    case "Closed":
                        buttonText = "Change to Closed";
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                        Thread.Sleep(500);
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.PendingDate.Value.ToShortDateString()); // pending date
                        this.uploaderClient.WriteTextbox(By.Id("Input_85"), listing.ClosedDate.Value.ToShortDateString()); // close date
                        this.uploaderClient.SetSelect(By.Id($"Input_524"), value: "EXCL"); // Property Condition at Closing
                        this.uploaderClient.WriteTextbox(By.Id("Input_84"), listing.SoldPrice); // close price
                        this.uploaderClient.WriteTextbox(By.Id("Input_526"), "None"); // closed Comments
                        this.uploaderClient.SetSelect(By.Id($"Input_655"), value: listing.HasContingencyInfo.BoolToNumericBool()); // Property Sale Contingency
                        this.uploaderClient.WriteTextbox(By.Id("Input_517"), listing.SellConcess); // Buyer Clsg Cost Pd By Sell($)
                        this.uploaderClient.SetMultipleCheckboxById("Input_525", listing.SoldTerms, "Buyer Financing", " "); // Buyer Financing
                        this.uploaderClient.WriteTextbox(By.Id("Input_519"), "0"); // Repairs Amount
                        break;
                    case "Pending":
                        buttonText = "Change to Pending";
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_512"));
                        this.uploaderClient.WriteTextbox(By.Id("Input_94"), listing.PendingDate.Value.ToShortDateString()); // Pending Date
                        this.uploaderClient.WriteTextbox(By.Id("Input_515"), listing.EstClosedDate.Value.ToShortDateString()); // Tentative Close Date
                        this.uploaderClient.WriteTextbox(By.Id("Input_81"), listing.ExpiredDate.Value.ToShortDateString()); // Expiration Date
                        this.uploaderClient.SetSelect(By.Id("Input_655"), listing.HasContingencyInfo.BoolToNumericBool()); // Property Sale Contingency YN
                        break;
                    case "TERM":
                        buttonText = "Change to Terminated";
                        string currentDate = DateTime.Today.ToString("MM/dd/yyyy");
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                        Thread.Sleep(500);
                        this.uploaderClient.WriteTextbox(By.Id("Input_522"), currentDate); // terminated date
                        break;
                    case "WITH":
                        buttonText = "Change to Withdrawn";
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(buttonText), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText(buttonText));
                        Thread.Sleep(500);
                        this.uploaderClient.WriteTextbox(By.Id("Input_113"), listing.OffMarketDate.Value.ToShortDateString()); // Withdrawn Date
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid Status '{listing.ListStatus}' for Houston Listing with Id '{listing.ResidentialListingID}'");
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
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                await this.UpdateVirtualTour(listing, cancellationToken);

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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
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

            this.GoToRemarksTab();

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
            }

            WaitAndClick("Start with a blank Listing");

            void WaitAndClick(string text)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText(text));
                this.uploaderClient.ClickOnElement(By.LinkText(text));
            }

            Thread.Sleep(1000);
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

        private void FillListingInformation(ResidentialListingRequest listing)
        {
            this.GoToTab("Listing Information");

            this.uploaderClient.SetSelect(By.Id("Input_181"), "EXAGY"); // List Type
            this.uploaderClient.WriteTextbox(By.Id("Input_182"), listing.ListPrice); // List Price

            if (listing.IsNewListing)
            {
                DateTime? listDate = null;
                switch (listing.ListStatus.ToEnumFromEnumMember<MarketStatuses>())
                {
                    case MarketStatuses.Active:
                        listDate = DateTime.Now;
                        break;
                    case MarketStatuses.Pending:
                    case MarketStatuses.OptionPending:
                    case MarketStatuses.PendingContinueToShow:
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case MarketStatuses.Terminated:
                    case MarketStatuses.Expired:
                    case MarketStatuses.Sold:
                        listDate = DateTime.Now.AddDays(-4);
                        break;
                }

                if (listDate.HasValue)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_183"), listDate.Value.ToShortDateString());  // List Date
                }

                var expirationDate = listing.ExpiredDate.HasValue ? listing.ExpiredDate.Value : (listing.SysCreatedOn ?? DateTime.Today).AddYears(1);
                this.uploaderClient.WriteTextbox(By.Id("Input_184"), expirationDate.ToShortDateString()); // Expiration Date
            }
            else if (listing.ListDate.HasValue)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_183"), listing.ListDate.Value.ToShortDateString()); // List Date
            }

            this.uploaderClient.SetSelect(By.Id("Input_185"), "0"); // Also For Lease
            this.uploaderClient.SetSelect(By.Id("Input_186"), "0"); // Priced at Lot Value Only
            this.uploaderClient.WriteTextbox(By.Id("Input_156"), listing.StreetNum); // Street Number
            this.uploaderClient.WriteTextbox(By.Id("Input_158"), listing.StreetName); // Street Name
            this.uploaderClient.WriteTextbox(By.Id("Input_160"), listing.UnitNum); // Unit #
            this.uploaderClient.SetSelect(By.Id("Input_159"), listing.StreetType, isElementOptional: true); // Street Type

            if (!string.IsNullOrEmpty(listing.City))
            {
                this.uploaderClient.FillFieldSingleOption("Input_161", listing.CityCode);
            }

            this.uploaderClient.SetSelect(By.Id("Input_162"), listing.StateCode); // State
            this.uploaderClient.WriteTextbox(By.Id("Input_163"), listing.Zip); // Zip Code
            this.uploaderClient.SetSelect(By.Id("Input_164"), listing.County); // County
            this.uploaderClient.WriteTextbox(By.Id("Input_165"), listing.Subdivision); // Subdivision
            this.uploaderClient.WriteTextbox(By.Id("Input_171"), listing.SectionNum); // Section #
            this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.Legal); // Legal Description

            if (this.uploaderClient.FindElements(By.Id("Input_172")).Any())
            {
                this.uploaderClient.SetSelect(By.Id("Input_172"), listing.IsPlannedDevelopment.BoolToNumericBool()); // Master Planned Community Y/N
                this.uploaderClient.FillFieldSingleOption("Input_173", listing.PlannedDevelopment); // Master Planned Community
            }

            if (!string.IsNullOrWhiteSpace(listing.LegalSubdivision))
            {
                var legalsubdivision = listing.LegalSubdivision.Contains("OTHER") ? "OTHER - " + listing.Zip : listing.LegalSubdivision;
                this.uploaderClient.WriteTextbox(By.Id("Input_320"), legalsubdivision);
            }

            if (!string.IsNullOrEmpty(listing.TaxID) && listing.TaxID != "0" && !listing.TaxID.Contains("-0000"))
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_174"), listing.TaxID); // Tax ID #
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_174"), "NA"); // Tax ID #
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_175"), listing.MapscoMapPage); // Key Map
        }

        private void FillPropertyInformation(HarListingRequest listing)
        {
            this.GoToPropertyInformationTab();

            this.uploaderClient.WriteTextbox(By.Id("Input_245"), listing.SqFtTotal); // Building SqFt
            this.uploaderClient.SetSelect(By.Id("Input_246"), "BUILD"); // SqFt Source
            this.uploaderClient.SetSelect(By.Id("Input_244"), "BUILD"); // Year Built Source
            this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.NumStories); // Stories
            this.uploaderClient.SetSelect(By.Id("Input_247"), "1"); // New Construction (1 , 0)
            this.uploaderClient.WriteTextbox(By.Id("Input_251"), listing.BuilderName);  // Builder Name
            this.UpdateYearBuiltDescription(listing);

            if (listing.BuildCompletionDate.HasValue)
            {
                var yearBuiltDesc = listing.YearBuiltDesc.ToEnumFromEnumMember<ConstructionStage>();
                if (yearBuiltDesc == ConstructionStage.Complete)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_249"), string.Empty, true, true); // Approx Completion Date
                    this.uploaderClient.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
                else if (yearBuiltDesc == ConstructionStage.Incomplete)
                {
                    this.uploaderClient.WriteTextbox(By.Id("Input_301"), string.Empty, true, true); // Approx Completion Date
                    this.uploaderClient.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
            }

            if (this.uploaderClient.FindElements(By.Id("Input_374")).Any())
            {
                this.uploaderClient.SetSelect(By.Id("Input_374"), "1"); // House on Property (1 , 0)
            }

            this.uploaderClient.SetSelect(By.Id("Input_142"), listing.HasUtilitiesDescription.BoolToNumericBool()); // Utility District  (1 , 0)
            this.uploaderClient.SetSelect(By.Id("Input_145"), listing.LotSizeSrc, isElementOptional: true); // Lot Size Source
            this.uploaderClient.WriteTextbox(By.Id("Input_147"), listing.LotSizeAcres); // Acres
            this.uploaderClient.SetSelect(By.Id("Input_148"), listing.LotSize); // Acreage
            this.uploaderClient.WriteTextbox(By.Id("Input_144"), listing.LotDim); // Lot Dimensions
            this.uploaderClient.WriteTextbox(By.Id("Input_202"), listing.GarageCapacity); // Garage - Number of Spaces
            this.uploaderClient.SetMultipleCheckboxById("Input_207", listing.AccessInstructionsDesc); // Access -- MLS-51 AccessibilityDesc -> AccessInstructionsDesc
            this.uploaderClient.SetMultipleCheckboxById("Input_203", listing.GarageDesc); // Garage Description
            this.uploaderClient.SetMultipleCheckboxById("Input_152", listing.RestrictionsDesc); // Restrictions
            this.FindAndSetMultipleCheckboxById(new[] { "Input_329" }, listing.PropSubType); // Property Type
            this.uploaderClient.SetMultipleCheckboxById("Input_241", listing.HousingStyleDesc);  // Style
            this.FindAndSetMultipleCheckboxById(new[] { "Input_146", "Input_492" }, listing.LotDesc); // Lot Description
            this.uploaderClient.SetMultipleCheckboxById("Input_150", listing.WaterfrontDesc); // Waterfront Features
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
            this.uploaderClient.SetMultipleCheckboxById("Input_328", listing.WasherConnections); // Washer Dryer Connection

            if (!string.IsNullOrEmpty(listing.GolfCourseName) && this.uploaderClient.FindElements(By.Id("Input_151")).Any())
            {
                this.uploaderClient.FillFieldSingleOption("Input_151", listing.GolfCourseName);
            }

            this.uploaderClient.SetSelect(By.Id("Input_265"), listing.HasCommunityPool.BoolToNumericBool()); // Pool - Area (1, 0)
            this.uploaderClient.SetSelect(By.Id("Input_263"), listing.HasPool.BoolToNumericBool()); // Pool - Private (1, 0)
            this.uploaderClient.SetMultipleCheckboxById("Input_264", listing.PoolDesc); // Private Pool Description
            this.FindAndSetMultipleCheckboxById(new[] { "Input_252", "Input_493" }, listing.InteriorDesc); // Interior Features
            this.FindAndSetMultipleCheckboxById(new[] { "Input_266" }, listing.FloorsDesc); // Flooring
            this.FindAndSetMultipleCheckboxById(new[] { "Input_259" }, listing.ExteriorDesc); // Exterior Description
            this.FindAndSetMultipleCheckboxById(new[] { "Input_260" }, listing.ConstructionDesc); // Exterior Construction
            this.FindAndSetMultipleCheckboxById(new[] { "Input_261" }, listing.RoofDesc); // Roof Description
            this.FindAndSetMultipleCheckboxById(new[] { "Input_262", "Input_500" }, listing.FoundationDesc); // Foundation Description
            this.uploaderClient.SetMultipleCheckboxById("Input_258", listing.EnergyDesc); // Energy Features
            this.uploaderClient.SetMultipleCheckboxById("Input_257", listing.GreenCerts); // Green/Energy Certifications
            this.uploaderClient.SetMultipleCheckboxById("Input_139", listing.HeatSystemDesc); // Heating System Description
            this.FindAndSetMultipleCheckboxById(new[] { "Input_140", "Input_506" }, listing.CoolSystemDesc); // Cooling System Description
            this.uploaderClient.SetMultipleCheckboxById("Input_141", listing.WaterDesc); // Water/Sewer Description
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
                this.SetLongitudeAndLatitudeValues(listing);
            }
        }

        private void FillRoomInformation(ResidentialListingRequest listing)
        {
            string tabName = "Rooms";
            this.uploaderClient.ClickOnElement(By.LinkText(tabName));
            Thread.Sleep(200);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            if (!listing.IsNewListing)
            {
                int index = 0;

                while (this.uploaderClient.FindElements(By.LinkText("Delete")) != null &&
                    this.uploaderClient.FindElements(By.LinkText("Delete")).Count > 1)
                {
                    try
                    {
                        this.uploaderClient.ScrollToTop();
                        this.uploaderClient.ClickOnElement(By.LinkText("Delete"));
                        Thread.Sleep(400);
                    }
                    catch
                    {
                        this.uploaderClient.ScrollToTop();
                        this.uploaderClient.ClickOnElement(By.Id("m_rpPageList_ctl02_lbPageLink"));
                        this.uploaderClient.ExecuteScript("Subforms['s_349'].deleteRow('_Input_349__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                    }
                }
            }

            var i = 0;
            foreach (var room in listing.Rooms)
            {
                if (i > 0)
                {
                    this.uploaderClient.ClickOnElement(By.Id("_Input_349_more"));
                    this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(400));
                }

                var roomType = $"_Input_349__REPEAT{i}_345";
                this.uploaderClient.SetSelect(By.Id($"_Input_349__REPEAT{i}_345"), room.RoomType, "Room Type", tabName);
                this.uploaderClient.ResetImplicitWait();
                this.uploaderClient.SetSelect(By.Id($"_Input_349__REPEAT{i}_346"), room.Level, "Level", tabName, isElementOptional: true);
                this.uploaderClient.SetMultipleCheckboxById($"_Input_349__REPEAT{i}_347", room.Features);

                this.uploaderClient.ScrollDownToElementHTML(roomType);
                i++;
            }
        }

        private void FillFinancialInformation(HarListingRequest listing)
        {
            this.GoToTab("Financial Information");

            var housingType = listing.HousingTypeDesc.ToEnumFromEnumMember<HousingType>();
            if (housingType != HousingType.CountryHomesAcreage)
            {
                this.uploaderClient.SetSelect(By.Id("Input_275"), listing.HasHoa.BoolToNumericBool()); // Mandatory HOA/Mgmt Co (1, 0)
                this.uploaderClient.WriteTextbox(By.Id("Input_278"), listing.AssocName); // Mandatory HOA/Mgmt Co Name
                this.uploaderClient.WriteTextbox(By.Id("Input_276"), listing.AssocPhone); // Mandatory HOA/Mgmt Co Phone
            }

            this.uploaderClient.SetMultiSelect(By.Id("Input_280"), listing.FinancingProposed); // Financing Considered
            this.FindAndSetMultipleCheckboxById(new[] { "Input_494", "Input_273" }, listing.Disclosures);  // Disclosures
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
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            this.GoToTab("Showing Information");

            this.uploaderClient.WriteTextbox(By.Id("Input_304"), listing.AgentListApptPhone, isElementOptional: true);  // Appointment Desk Phone
            this.uploaderClient.SetSelect(By.Id("Input_303"), "OFFIC");  // Appointment Phone Desc
            this.uploaderClient.WriteTextbox(By.Id("Input_236"), listing.AltPhoneCommunity, isElementOptional: true);  // Agent Alternate Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_136"), listing.Directions.RemoveSlash(), true); // Directions
            this.uploaderClient.SetMultipleCheckboxById("Input_218", listing.ShowingInstructions);  // Showing Instructions
            this.uploaderClient.WriteTextbox(By.Id("Input_327"), listing.BuyerIncentive.AmountByType(listing.BuyerIncentiveDesc)); // Buyer Agency Compensation
            this.uploaderClient.WriteTextbox(By.Id("Input_226"), "0%"); // Sub Agency Compensation
            this.uploaderClient.WriteTextbox(By.Id("Input_229"), listing.GetAgentBonusAmount()); // Bonus
            this.uploaderClient.WriteTextbox(By.Id("Input_230"), listing.CompBuyBonusExpireDate.HasValue ? listing.CompBuyBonusExpireDate.Value : string.Empty); // Bonus End Date
            this.uploaderClient.SetSelect(By.Id("Input_216"), "0");  // Variable Compensation
        }

        private void FillRemarks(HarListingRequest listing)
        {
            this.GoToRemarksTab();

            this.UpdatePublicRemarksInRemarksTab(listing);

            this.uploaderClient.WriteTextbox(By.Id("Input_137"), string.Empty);
            this.uploaderClient.WriteTextbox(By.Id("Input_137"), listing.GetAgentRemarksMessage(), true); // Agent Remarks
        }

        private void GoToRemarksTab()
        {
            this.GoToTab("Remarks/Tour Links");
        }

        private void GoToPropertyInformationTab()
        {
            this.GoToTab("Property Information");
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

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            var remarks = listing.GetPublicRemarks();
            this.uploaderClient.WriteTextbox(By.Id("Input_135"), remarks, true); // Public Remarks
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            var getLatLongFromAddress = "Get Lat/Long from address";

            if (this.uploaderClient.FindElements(By.LinkText(getLatLongFromAddress)).Any())
            {
                this.uploaderClient.ClickOnElement(By.LinkText(getLatLongFromAddress));
                Thread.Sleep(1000);
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

        private async Task ProcessImages(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            var media = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: this.CurrentMarket, cancellationToken);
            var imageOrder = 0;
            foreach (var image in media)
            {
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_ucImageLoader_m_tblImageLoader"), cancellationToken);

                this.uploaderClient.FindElement(By.Id("m_ucImageLoader_m_tblImageLoader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                Thread.Sleep(3000);
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id($"photoCell_{imageOrder}"), cancellationToken);

                if (!string.IsNullOrEmpty(image.Caption))
                {
                    this.uploaderClient.ExecuteScript($"jQuery('#photoCell_{imageOrder} a')[0].click();");
                    Thread.Sleep(500);
                    this.uploaderClient.ExecuteScript($"jQuery('#m_tbxDescription').val('{image.Caption}');");
                    Thread.Sleep(500);
                    this.uploaderClient.ClickOnElementById("m_ucDetailsView_m_btnSave");
                }

                imageOrder++;
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

                // Date
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_332"), entry: openHouse.Date);

                // From Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_TextBox_333"), entry: openHouse.StartTime.To12Format());
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_337__REPEAT{index}_RadioButtonList_333_{fromTimeTT}");

                // To Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_TextBox_334"), entry: openHouse.EndTime.To12Format());
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_337__REPEAT{index}_RadioButtonList_334_{endTimeTT}");

                // Comments
                this.uploaderClient.WriteTextbox(By.Id($"_Input_337__REPEAT{index}_339"), entry: openHouse.Comments);

                index++;
            }
        }
    }
}
