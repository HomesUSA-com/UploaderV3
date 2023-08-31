namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Crosscutting.Extensions.Abor;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;

    public class AborUploadService : IAborUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IListingRequestRepository sqlDataLoader;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<AborUploadService> logger;

        public AborUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IListingRequestRepository sqlDataLoader,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<AborUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.sqlDataLoader = sqlDataLoader ?? throw new ArgumentNullException(nameof(sqlDataLoader));
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

        public async Task<LoginResult> Login()
        {
            var marketInfo = this.options.MarketInfo.Abor;

            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            this.uploaderClient.DeleteAllCookies();

            // Connect to the login page
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("loginbtn"));
            var marketCredentials = await credentialsTask;
            this.uploaderClient.WriteTextbox(By.Name("username"), marketCredentials.UserName);
            this.uploaderClient.WriteTextbox(By.Name("password"), marketCredentials.Password);
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

        public UploadResult Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Abor.LogoutUrl);
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
                await this.Login();

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                if (listing.IsNewListing)
                {
                    this.NavigateToNewPropertyInput();
                }
                else
                {
                    this.NavigateToQuickEdit(listing.MLSNum);
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"), cancellationToken);
                    this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));
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
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login();
                Thread.Sleep(5000);

                try
                {
                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput();
                    }
                    else
                    {
                        this.NavigateToQuickEdit(listing.MLSNum);
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));
                    }

                    this.FillListingInformation(listing);
                    this.FillGeneralInformation(listing);
                    this.FillAdditionalInformation(listing as AborListingRequest);
                    this.FillDocumentsAndUtilities(listing as AborListingRequest);
                    this.FillGreenEnergyInformation();
                    this.FillFinancialInformation(listing as AborListingRequest);
                    this.FillShowingInformation(listing);
                    this.FillAgentOfficeInformation(listing);
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
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login();

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));

                this.UpdateYearBuiltDescriptionInGeneralTab(listing);

                this.uploaderClient.ClickOnElement(By.LinkText("Remarks"));

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

                await this.Login();
                this.NavigateToQuickEdit(listing.MLSNum);

                // Enter Manage Photos
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Photos"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Manage Photos"));
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lbSave"), cancellationToken);

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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                await this.Login();
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);

                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));
                this.uploaderClient.ScrollDown();
                this.uploaderClient.WriteTextbox(By.Id("Input_127"), listing.ListPrice); // List Price

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
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                await this.Login();

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                this.NavigateToQuickEdit(listing.MLSNum);

                Thread.Sleep(1500);
                switch (listing.ListStatus)
                {
                    case "CSLD":

                        this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Change to Sold"), cancellationToken);
                        this.uploaderClient.ClickOnElement(By.LinkText("Change to Sold"));
                        Thread.Sleep(500);
                        this.uploaderClient.WriteTextbox(By.Id("Input_74"), listing.SoldPrice);
                        this.uploaderClient.WriteTextbox(By.Id("Input_73"), listing.ClosedDate.HasValue ? listing.ClosedDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        this.uploaderClient.SetSelect(By.Id($"Input_324"), value: listing.Financing);
                        this.uploaderClient.WriteTextbox(By.Id("Input_325"), listing.SellConcess);
                        this.uploaderClient.WriteTextbox(By.Id("Input_83"), listing.ContractDate.HasValue ? listing.ContractDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        // this.uploaderClient.WriteTextbox(By.Id("Input_527"), listing.SoldComments);
                        this.uploaderClient.WriteTextbox(By.Id("Input_321"), listing.AgentMarketUniqueId);
                        this.uploaderClient.ExecuteScript("javascript:document.getElementById('Input_321_Refresh').value='1';RefreshToSamePage();");
                        this.uploaderClient.WriteTextbox(By.Id("Input_323"), listing.SecondAgentMarketUniqueId);
                        this.uploaderClient.ExecuteScript("javascript:document.getElementById('Input_323_Refresh').value='1';RefreshToSamePage();");
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid Status '{listing.ListStatus}' for Austin Listing with Id '{listing.ResidentialListingID}'");
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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket, cancellationToken);
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                await this.Login();
                Thread.Sleep(1000);

                this.uploaderClient.ClickOnElement(By.LinkText(@"Edit Listing Details"), shouldWait: false, waitTime: 0, isElementOptional: false);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.SwitchTo("main");
                this.uploaderClient.SwitchTo("workspace");
                Thread.Sleep(1000);

                this.uploaderClient.ExecuteScript(" SP('7') ");
                Thread.Sleep(2000);

                var virtualTourResponse = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: this.CurrentMarket, cancellationToken);
                var virtualTour = virtualTourResponse.FirstOrDefault();
                if (virtualTour != null)
                {
                    this.uploaderClient.WriteTextbox(
                        findBy: By.Id("VIRTTOUR"),
                        virtualTour.GetUnbrandedUrl());
                }

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
                await this.Login();
                Thread.Sleep(5000);
                this.NavigateToQuickEdit(listing.MLSNum);

                if (!listing.OpenHouse.Any())
                {
                    return UploadResult.Success;
                }

                Thread.Sleep(400);

                // Enter OpenHouse
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Open Houses"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Open Houses"));
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_tdValidate"), cancellationToken);
                Thread.Sleep(3000);

                this.CleanOpenHouse();
                this.AddOpenHouses(listing);
                return UploadResult.Success;
            }
        }

        private void NavigateToNewPropertyInput()
        {
            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/AddEdit");
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Add new"));
            this.uploaderClient.ClickOnElement(By.LinkText("Add new"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"));
            this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.PartialLinkText("Start with a blank Property"));
            this.uploaderClient.ClickOnElement(By.PartialLinkText("Start with a blank Property"));

            Thread.Sleep(1000);
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl("https://matrix.abor.com/Matrix/AddEdit");
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"));
            this.uploaderClient.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), value: mlsNumber);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
            this.uploaderClient.ClickOnElement(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
        }

        private void FillStatusInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.WriteTextbox(By.Id("Input_778"), listing.ExpectedActiveDate, isElementOptional: true);
        }

        private void FillListingInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Listing";
            this.uploaderClient.ClickOnElement(By.LinkText(tabName)); // click in tab Listing Information
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_179"));

            // Listing Information
            this.uploaderClient.SetSelect(By.Id("Input_179"), "EA"); // List Agreement Type
            this.uploaderClient.SetSelect(By.Id("Input_341"), "LIMIT"); // Listing Service
            this.uploaderClient.SetMultipleCheckboxById("Input_180", "STANDARD"); // Special Listing Conditions
            this.uploaderClient.SetSelect(By.Id("Input_181"), "A"); // List Agreement Document
            this.uploaderClient.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price

            if (listing.ListDate.HasValue)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_83"), listing.ListDate.Value.AddYears(1).ToShortDateString()); // Expiration Date
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_83"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
            }

            // Location Information
            this.uploaderClient.WriteTextbox(By.Id("Input_183"), listing.StreetNum); // Street #
            this.uploaderClient.WriteTextbox(By.Id("Input_185"), listing.StreetName); // Street Name
            this.uploaderClient.SetSelect(By.Id("Input_186"), listing.StreetType); // Street Type (NM)
            this.uploaderClient.WriteTextbox(By.Id("Input_190"), !string.IsNullOrEmpty(listing.UnitNum) ? listing.UnitNum : string.Empty); // Unit # (NM)
            this.uploaderClient.SetSelect(By.Id("Input_191"), listing.County); // County
            this.uploaderClient.SetSelectIfExist(By.Id("Input_192"), listing.CityCode); // City
            this.uploaderClient.SetSelect(By.Id("Input_193"), listing.State); // State
            this.uploaderClient.SetSelect(By.Id("Input_399"), "US"); // Country
            this.uploaderClient.WriteTextbox(By.Id("Input_194"), listing.Zip); // ZIP Code
            this.uploaderClient.WriteTextbox(By.Id("Input_196"), listing.Subdivision); // Subdivision
            this.uploaderClient.WriteTextbox(By.Id("Input_197"), listing.Legal); // Tax Legal Description
            this.uploaderClient.WriteTextbox(By.Id("Input_199"), listing.OtherFees); // Tax Lot
            this.uploaderClient.WriteTextbox(By.Id("Input_201"), listing.TaxID); // Parcel ID
            this.uploaderClient.SetSelect(By.Id("Input_202"), "0"); // Additional Parcels Y/N

            this.SetLongitudeAndLatitudeValues(listing);

            this.uploaderClient.ScrollDown(1000);
            this.FillFieldSingleOption("Input_204", listing.MLSArea); // MLS Area
            this.uploaderClient.SetMultipleCheckboxById("Input_343", "N"); // FEMA 100 Yr Flood Plain
            this.uploaderClient.SetSelect(By.Id("Input_206"), "N"); // ETJ

            // School Information
            this.uploaderClient.SetSelectIfExist(By.Id("Input_207"), listing.SchoolDistrict); // School District
            this.uploaderClient.SetSelectIfExist(By.Id("Input_209"), listing.SchoolName1); // School District/Elementary A
            this.uploaderClient.SetSelectIfExist(By.Id("Input_210"), listing.SchoolName2); // School District/Middle / Intermediate School
            this.uploaderClient.SetSelectIfExist(By.Id("Input_211"), listing.SchoolName3); // School District/9 Grade / High School
            this.uploaderClient.WriteTextbox(By.Id("Input_212"), listing.SchoolName4); // Elementary Other
            this.uploaderClient.WriteTextbox(By.Id("Input_213"), listing.SchoolName5); // Middle or Junior Other
            this.uploaderClient.WriteTextbox(By.Id("Input_214"), listing.SchoolName6); // High School Other
        }

        private void FillDocumentsAndUtilities(AborListingRequest listing)
        {
            string tabName = "Documents & Utilities";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText(tabName)); // click in tab DocumentsAndUtilities
            Thread.Sleep(100);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_271"));
            if (listing.IsNewListing)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_271", "None", "Disclosures", tabName);
                this.uploaderClient.SetMultipleCheckboxById("Input_272", "NA", "Documents Available", tabName);
                this.uploaderClient.ScrollToTop();
            }

            this.uploaderClient.SetMultipleCheckboxById("Input_273", listing.HeatSystemDesc, "Heating", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_274", listing.CoolSystemDesc, "Cooling", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_275", listing.GreenWaterConservation, "Water Source", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_276", listing.WaterDesc, "Sewer", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_278", listing.UtilitiesDesc, "Utilities", tabName);
        }

        private void FillGreenEnergyInformation()
        {
            this.uploaderClient.ClickOnElement(By.LinkText("Green Energy"));
            Thread.Sleep(100);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_280"));
            this.uploaderClient.SetMultipleCheckboxById("Input_280", "NONE"); // Green Energy Efficient
            this.uploaderClient.SetMultipleCheckboxById("Input_281", "None"); // Green Sustainability
        }

        private void FillGeneralInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ClickOnElement(By.LinkText("General"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_215"));

            this.uploaderClient.SetSelect(By.Id("Input_215"), listing.PropSubType); // Property Sub Type (1)
            this.uploaderClient.SetSelect(By.Id("Input_216"), "FEESIM"); // Ownership Type
            this.uploaderClient.SetMultipleCheckboxById("Input_650", listing.NumStories); // Levels

            this.uploaderClient.WriteTextbox(By.Id("Input_220"), listing.NumBedsMainLevel); // Main Level Beds
            this.uploaderClient.WriteTextbox(By.Id("Input_221"), listing.NumBedsOtherLevels); // # Other Level Beds
            this.uploaderClient.WriteTextbox(By.Id("Input_218"), listing.YearBuilt); // Year Built

            this.uploaderClient.SetSelect(By.Id("Input_219"), "BUILDER"); // Year Built Source
            this.uploaderClient.SetMultipleCheckboxById("Input_225", listing.YearBuiltDesc); // Year Built Description

            this.uploaderClient.WriteTextbox(By.Id("Input_224"), listing.BathsFull); // Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_223"), listing.BathsHalf); // Half Bath
            this.uploaderClient.WriteTextbox(By.Id("Input_659"), listing.NumLivingAreas); // Living
            this.uploaderClient.WriteTextbox(By.Id("Input_660"), listing.NumDiningAreas); // Dining
            this.uploaderClient.WriteTextbox(By.Id("Input_226"), listing.SqFtTotal); // Living Area
            this.uploaderClient.SetSelect(By.Id("Input_227"), "BUILDER"); // Living Area Source

            this.uploaderClient.WriteTextbox(By.Id("Input_717"), listing.GarageCapacity); // # Garage Spaces
            this.uploaderClient.WriteTextbox(By.Id("Input_229"), listing.GarageCapacity); // Parking Total
            this.uploaderClient.SetSelect(By.Id("Input_342"), listing.FacesDesc); // Direction Faces

            this.uploaderClient.WriteTextbox(By.Id("Input_242"), listing.LotSize); // Lot Size Acres
            this.uploaderClient.WriteTextbox(By.Id("Input_241"), listing.LotDim); // Lot Dimensions (Frontage x Depth)
            this.uploaderClient.SetMultipleCheckboxById("Input_231", listing.ConstructionDesc); // Construction (5)
            this.uploaderClient.SetMultipleCheckboxById("Input_236", listing.GarageDesc); // Garage Description (4) / Parking Features
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ScrollDown(100);

            this.uploaderClient.SetMultipleCheckboxById("Input_239", listing.FoundationDesc); // Foundation (2)
            this.uploaderClient.WriteTextbox(By.Id("Input_678"), listing.OwnerName); // Builder Name
            this.uploaderClient.SetMultipleCheckboxById("Input_230", listing.UnitStyleDesc); // Unit Style (5)
            this.uploaderClient.SetMultipleCheckboxById("Input_234", listing.ViewDesc); // View (4)
            this.uploaderClient.SetSelect(By.Id("Input_235"), listing.DistanceToWaterAccess); // Distance to Water Access
            this.uploaderClient.SetMultipleCheckboxById("Input_237", listing.WaterfrontDesc); // Waterfront Description
            this.uploaderClient.SetSelect(By.Id("Input_238"), listing.BodyofWater); // Water Body Name
            this.uploaderClient.SetMultipleCheckboxById("Input_244", listing.LotDesc); // Lot Description (4)
            this.uploaderClient.SetMultipleCheckboxById("Input_232", listing.FloorsDesc); // Flooring (4)
            this.uploaderClient.SetMultipleCheckboxById("Input_240", listing.RestrictionsDesc); // Restrictions Description (5)
        }

        private void FillFieldSingleOption(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                this.logger.LogInformation("Cannot proceed to fill field {fieldName} with an empty value", fieldName);
                return;
            }

            var mainWindow = this.uploaderClient.WindowHandles.FirstOrDefault(windowHandle => windowHandle == this.uploaderClient.CurrentWindowHandle);
            this.uploaderClient.ExecuteScript(script: $"jQuery('#{fieldName}_TB').focus();");
            this.uploaderClient.ExecuteScript(script: $"jQuery('#{fieldName}_A')[0].click();");

            this.uploaderClient.SwitchToLast();

            Thread.Sleep(400);

            char[] fieldValue = value.ToUpper().ToArray();

            foreach (var charact in fieldValue)
            {
                Thread.Sleep(200);
                this.uploaderClient.FindElement(By.Id("m_txtSearch")).SendKeys(charact.ToString().ToUpper());
            }

            Thread.Sleep(400);
            var selectElement = $"const selected = jQuery('li[title^=\"{value}\"]'); jQuery(selected).focus(); jQuery(selected).click()";
            this.uploaderClient.ExecuteScript(script: selectElement);
            Thread.Sleep(400);

            this.uploaderClient.ExecuteScript("javascript:LBI_Popup.selectItem(true);");
            this.uploaderClient.SwitchTo().Window(mainWindow);
        }

        private void FillRooms(ResidentialListingRequest listing)
        {
            const string tabName = "Rooms";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Rooms")); // click in tab Listing Information

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer")); // Look if the footer elements has been loaded
            this.uploaderClient.WriteTextbox(By.Id("Input_193"), listing.Beds); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_194"), listing.BathsFull); // Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_195"), listing.BathsHalf); // Half Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_196"), listing.NumLivingAreas, isElementOptional: true); // Living Areas
            this.uploaderClient.WriteTextbox(By.Id("Input_197"), listing.NumDiningAreas, isElementOptional: true); // Dining Areas
            this.uploaderClient.WriteTextbox(By.Id("Input_555"), listing.NumberFireplaces, isElementOptional: true); // Fireplaces

            if (!this.uploaderClient.UploadInformation.IsNewListing)
            {
                var elems = this.uploaderClient.FindElements(By.CssSelector("table[id^=_Input_556__del_REPEAT] a"));

                foreach (var elem in elems.Where(c => c.Displayed))
                {
                    elem.Click();
                }
            }

            var i = 0;
            foreach (var room in listing.Rooms)
            {
                if (i > 0)
                {
                    this.uploaderClient.ClickOnElement(By.Id("_Input_556_more"));
                    this.uploaderClient.SetImplicitWait(TimeSpan.FromMilliseconds(400));
                }

                var roomType = $"_Input_556__REPEAT{i}_190";
                this.uploaderClient.SetSelect(By.Id($"_Input_556__REPEAT{i}_190"), room.RoomType, "Room Type", tabName);
                this.uploaderClient.ResetImplicitWait();
                this.uploaderClient.SetSelect(By.Id($"_Input_556__REPEAT{i}_491"), room.Level, "Level", tabName, isElementOptional: true);

                if (room.HasDimensions)
                {
                    this.uploaderClient.WriteTextbox(By.Id($"_Input_556__REPEAT{i}_191"), room.Dimensions, isElementOptional: true);
                }

                this.uploaderClient.ScrollDownToElementHTML(roomType);
                i++;
            }
        }

        private void FillAdditionalInformation(AborListingRequest listing)
        {
            var tabName = "Additional";
            this.uploaderClient.ClickOnElement(By.LinkText(tabName));
            Thread.Sleep(800);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer"));

            if (listing.Bed1Level == "MAIN" && !listing.InteriorDesc.Contains("MSTDW"))
            {
                listing.InteriorDesc = "MSTDW," + listing.InteriorDesc;
            }

            this.uploaderClient.SetMultipleCheckboxById("Input_257", listing.InteriorDesc, "Interior Features", tabName); // Interior Features (12)
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.SetMultipleCheckboxById("Input_264", listing.ExteriorDesc, "Exterior Features", tabName); // Exterior Features (12)
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.SetMultipleCheckboxById("Input_256", listing.AppliancesDesc, "Appliances / Equipment", tabName); // Appliances / Equipment (12)
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.SetMultipleCheckboxById("Input_267", listing.WindowCoverings, "Window Features", tabName); // Window Features
            this.uploaderClient.SetMultipleCheckboxById("Input_266", listing.SecurityDesc, "Security Features", tabName); // Security Features
            this.uploaderClient.ScrollDown(200);
            this.uploaderClient.SetMultipleCheckboxById("Input_258", "None", "Accessibility Features", tabName); // Accessibility Features
            this.uploaderClient.SetMultipleCheckboxById("Input_265", listing.PatioAndPorchFeatures, "Patio and Porch Features", tabName); // Patio and Porch Features
            this.uploaderClient.SetMultipleCheckboxById("Input_255", listing.LaundryLocDesc, "Laundry Location", tabName); // Laundry Location (3)
            this.uploaderClient.SetMultipleCheckboxById("Input_262", "None", "Private Pool Features (On Property)", tabName); // Private Pool Features (On Property)
            this.uploaderClient.WriteTextbox(By.Id("Input_259"), listing.NumberFireplaces); // # of Fireplaces
            this.uploaderClient.SetMultipleCheckboxById("Input_249", "None", "Horse Amenities", tabName); // Horse Amenities
            this.uploaderClient.SetMultipleCheckboxById("Input_260", listing.FireplaceDesc, "Fireplace Description", tabName); // Fireplace Description (3)
            this.uploaderClient.SetMultipleCheckboxById("Input_261", listing.FenceDesc, "Fencing", tabName); // Fencing (4)

            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ScrollDown(200);
            this.uploaderClient.SetMultipleCheckboxById("Input_269", "NONE", "Other Structures", tabName); // Other Structures
            this.uploaderClient.SetMultipleCheckboxById("Input_251", listing.GuestAccommodationsDesc, "Guest Accommodations", tabName); // Guest Accommodations

            this.uploaderClient.WriteTextbox(By.Id("Input_252"), listing.NumGuestBeds); // # Guest Beds
            this.uploaderClient.WriteTextbox(By.Id("Input_253"), listing.NumGuestFullBaths); // # Guest Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_254"), listing.NumGuestHalfBaths); // # Guest Half Baths

            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ScrollDown(400);
            this.uploaderClient.SetMultipleCheckboxById("Input_268", listing.CommonFeatures, "Community Features", tabName); // Community Features
        }

        private void FillFeatures(ResidentialListingRequest listing)
        {
            string tabName = "Features";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Features")); // click in tab Features

            // this.uploaderClient.SetMultipleCheckboxById("Input_156", "TRADI", "Style", tabName); // Style (default hardcode "Traditional")
            this.uploaderClient.SetMultipleCheckboxById("Input_557", listing.HousingStyleDesc, "Style (Max 20)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_558", listing.FoundationDesc, "Foundation (Max 7)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_563", listing.NumStories, "# Stories", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_564", listing.AtticRoom, "Attic", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_559", listing.RoofDesc, "Roof", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_561", listing.ExteriorDesc, "Construction/Exterior (Max 18)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_562", listing.FireplaceDesc, "Fireplace (Max 19)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_565", listing.FloorsDesc, "Flooring (Max 6)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_566", listing.KitchenDesc, "Kitchen Features", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_567", listing.LaundryFacilityDesc, "Laundry (Max 17)", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_568"), listing.GarageCarportDesc, "Garage", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_569"), listing.CarportDesc, "Carport", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_573", listing.Bed1Desc, "Master Bedroom Desc", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_570", listing.GarageDesc, "Garage/Carport Desc (Max 23)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_571", listing.InclusionsDesc, "Interior Features (Max 37)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_572", listing.AppliancesDesc, "Appliances/Equipment (Max 13)", tabName);
        }

        private void FillLotEnvironmentUtilityInformation(ResidentialListingRequest listing)
        {
            string tabName = "Lot/Environment/Utility";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Lot/Environment/Utility")); // Lot/Environment/Utility

            this.uploaderClient.WriteTextbox(By.Id("Input_576"), listing.LotDim); // Lot Dimensions
            this.uploaderClient.SetMultipleCheckboxById("Input_581", listing.FenceDesc, "Fencing (Max 12)", tabName);
            this.uploaderClient.WriteTextbox(By.Id("Input_577"), listing.LotSize); // Apx Acreage
            this.uploaderClient.SetSelect(By.Id("Input_582"), listing.WaterfrontYN, fieldLabel: "Waterfront", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_585", listing.WaterfrontFeatures, "Water Features", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_578"), value: "0", fieldLabel: "Manufactured Allowed", tabName); // (default hardcode "No")
            this.uploaderClient.SetSelect(By.Id("Input_583"), listing.IsGatedCommunity, fieldLabel: "Gated Community", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_592"), listing.HasSprinklerSys, fieldLabel: "Sprinkler System", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_593", listing.SprinklerSysDesc, fieldLabel: "Sprinkler System Desc", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_584", listing.PoolDesc, fieldLabel: "Pool (Max 2)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_595", listing.RestrictionsDesc, fieldLabel: "Restrictions Type", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_591"), listing.HasWaterAccess, fieldLabel: "Water Access", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_590", listing.WaterAccessDesc, fieldLabel: "Water Access Type", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_596", listing.ExteriorFeatures, fieldLabel: "Exterior Features (Max 32)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_597", listing.TopoLandDescription, fieldLabel: "Topo/Land Desc (Max 33)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_599", listing.CommonFeatures, fieldLabel: "Neighborhood Amenitites (Max 19)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_600", listing.RoadFrontageDesc, fieldLabel: "Access/Road Surface", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_586"), value: "0", fieldLabel: "Spa/Hot Tub", tabName); // (default hardcode "No")

            // Environment / Energy
            this.uploaderClient.SetSelect(By.Id("Input_601"), listing.UpgradedEnergyFeatures, fieldLabel: "Upgraded Energy Features", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_602"), listing.EES, fieldLabel: "EES Features", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_605", listing.GreenIndoorAirQuality, fieldLabel: "Green Indoor Air Quality", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_604", listing.GreenCerts, fieldLabel: "Green Building Verification", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_606", listing.EESFeatures, fieldLabel: "Green Energy Efficient", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_608", listing.EnergyDesc, fieldLabel: "Green Verification Source", tabName);

            // Utilities
            this.uploaderClient.SetMultipleCheckboxById("Input_613", listing.SupOther, " Other Utilities", tabName);
        }

        private void FillFinancialInformation(AborListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.LinkText("Financial"));
            Thread.Sleep(200);
            this.uploaderClient.WaitUntilElementExists(By.Id("ctl02_m_divFooterContainer"));

            this.uploaderClient.SetSelect(By.Id("Input_282"), listing.HasHoa ? "1" : "0"); // Association YN

            if (listing.HasHoa)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_283"), listing.AssocName, true); // HOA Name
                this.uploaderClient.WriteTextbox(By.Id("Input_285"), listing.AssocFee, true); // HOA Fee
                this.uploaderClient.SetSelect(By.Id("Input_286"), listing.HOA, true); // Association Requirement
                this.uploaderClient.SetSelect(By.Id("Input_287"), listing.AssocFeeFrequency, true); // HOA Frequency
                this.uploaderClient.WriteTextbox(By.Id("Input_288"), listing.AssocTransferFee, true); // HOA Transfer Fee
                this.uploaderClient.SetMultipleCheckboxById("Input_290", listing.AssocFeeIncludes); // HOA Fees Include (5)
            }

            this.uploaderClient.SetMultipleCheckboxById("Input_291", listing.FinancingProposed); // Acceptable Financing (5)
            this.uploaderClient.WriteTextbox(By.Id("Input_296"), "0"); // Estimated Taxes ($)
            this.uploaderClient.WriteTextbox(By.Id("Input_297"), listing.TaxYear); // Tax Year

            this.uploaderClient.WriteTextbox(By.Id("Input_294"), listing.TaxRate, true); // Tax Rate
            this.uploaderClient.WriteTextbox(By.Id("Input_293"), "0", true); // Tax Assessed Value
            this.uploaderClient.SetMultipleCheckboxById("Input_295", "None"); // Buyer Incentive
            this.uploaderClient.SetMultipleCheckboxById("Input_298", listing.ExemptionsDesc); // Tax Exemptions
            this.uploaderClient.WriteTextbox(By.Id("Input_728"), listing.TitleCo); // Preferred Title Company
            this.uploaderClient.ScrollDown(400);
            this.uploaderClient.SetMultipleCheckboxById("Input_299", "Funding"); // Possession
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.LinkText("Showing"));
            Thread.Sleep(100);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_301"));

            this.uploaderClient.SetSelect(By.Id("Input_301"), "VCNT"); // Occupant
            this.uploaderClient.WriteTextbox(By.Id("Input_302"), listing.OwnerName); // Owner Name
            this.uploaderClient.SetMultipleCheckboxById("Input_305", listing.Showing); // Showing Requirements
            this.uploaderClient.SetSelect(By.Id("Input_651"), listing.LockboxTypeDesc ?? "None"); // Lockbox Type
            this.uploaderClient.SetMultipleCheckboxById("Input_720", "OWN"); // Showing Contact Type
            this.uploaderClient.WriteTextbox(By.Id("Input_310"), listing.OwnerName);
            this.uploaderClient.WriteTextbox(By.Id("Input_311"), listing.AgentListApptPhone, true); // Showing Contact Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_406"), listing.OtherPhone, true);  // Showing Service Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_313"), listing.ShowingInstructions); // Showing Instructions
        }

        private void FillAgentOfficeInformation(ResidentialListingRequest listing)
        {
            this.uploaderClient.ClickOnElement(By.LinkText("Agent/Office"));
            Thread.Sleep(100);
            this.uploaderClient.WaitUntilElementExists(By.Id("ctl02_m_divFooterContainer"));

            this.uploaderClient.SetSelect(By.Id("Input_315"), "Percent"); // Sub Agency Compensation Type
            this.uploaderClient.WriteTextbox(By.Id("Input_314"), "0.0"); // Sub Agency Compensation

            this.uploaderClient.SetSelect(By.Id("Input_316"), listing.BuyerIncentiveDesc.ToCommissionType(), true); // Buyer Agency Compensation Type
            this.uploaderClient.WriteTextbox(By.Id("Input_510"), listing.BuyerIncentive, true);

            if (listing.HasBonusWithAmount)
            {
                this.uploaderClient.SetSelect(By.Id("Input_318"), listing.AgentBonusAmountType.ToCommissionType(), true); // Bonus to BA
                this.uploaderClient.WriteTextbox(By.Id("Input_317"), listing.AgentBonusAmount); // Bonus to BA Amount
            }

            this.uploaderClient.SetSelect(By.Id("Input_319"), "0", true); // Dual Variable Compensation
            this.uploaderClient.SetSelect(By.Id("Input_353"), "0", true); // Intermediary
        }

        private void FillRemarks(AborListingRequest listing)
        {
            const string tabName = "Remarks";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");
            this.uploaderClient.ClickOnElement(By.LinkText(tabName)); // Financial Information
            this.UpdatePublicRemarksInRemarksTab(listing); // Public Remarks

            this.uploaderClient.WriteTextbox(By.Id("Input_141"), listing.GetPrivateRemarks()); // Agent Remarks
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

        private void UpdateYearBuiltDescriptionInGeneralTab(ResidentialListingRequest listing)
        {
            const string tabName = "General";
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_185"));
            this.uploaderClient.ScrollDown();
            this.uploaderClient.SetSelect(By.Id("Input_184"), listing.YearBuiltDesc, fieldLabel: "Construction Status", tabName); // Construction Status
            this.uploaderClient.WriteTextbox(By.Id("Input_185"), listing.YearBuilt); // Year Built
        }

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            Thread.Sleep(400);
            this.uploaderClient.WriteTextbox(By.Id("Input_140"), listing.GetPublicRemarks()); // Internet / Remarks / Desc. of Property
            this.uploaderClient.WriteTextbox(By.Id("Input_142"), listing.Directions); // Syndication Remarks
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            this.uploaderClient.WriteTextbox(By.Id("INPUT__146"), value: listing.Latitude); // Latitude
            this.uploaderClient.WriteTextbox(By.Id("INPUT__168"), value: listing.Longitude); // Longitude
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
            var elems = this.uploaderClient.FindElements(By.CssSelector("table[id^=_Input_349__del_REPEAT] a")).Count(c => c.Displayed);
            this.uploaderClient.ScrollDown(3000);
            while (elems > 1)
            {
                this.uploaderClient.ScrollDown();
                var elementId = $"_Input_349__del_REPEAT{elems - 1}_";
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
                    this.uploaderClient.ClickOnElementById(elementId: $"_Input_349_more");
                    Thread.Sleep(1000);
                }

                // Date
                this.uploaderClient.WriteTextbox(By.Id($"_Input_349__REPEAT{index}_342"), entry: openHouse.Date);

                // From Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_349__REPEAT{index}_TextBox_344"), entry: openHouse.StartTime.To12Format());
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_349__REPEAT{index}_RadioButtonList_344_{fromTimeTT}");

                // To Time
                this.uploaderClient.WriteTextbox(By.Id($"_Input_349__REPEAT{index}_TextBox_345"), entry: openHouse.EndTime.To12Format());
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? 1 : 0;
                this.uploaderClient.ClickOnElementById($"_Input_349__REPEAT{index}_RadioButtonList_345_{endTimeTT}");

                // Active Status
                this.uploaderClient.SetSelect(By.Id($"_Input_349__REPEAT{index}_346"), value: Convert.ToInt32(openHouse.Active));

                // Open House Type
                var type = openHouse.Type.ToString().ToUpperInvariant();
                this.uploaderClient.SetSelect(By.Id($"_Input_349__REPEAT{index}_347"), value: type);

                // Comments
                this.uploaderClient.WriteTextbox(By.Id($"_Input_349__REPEAT{index}_348"), entry: openHouse.Comments);

                index++;
            }
        }
    }
}
