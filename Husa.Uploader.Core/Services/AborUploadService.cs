namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Extensions;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
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

                    this.FillStatusInformation(listing);
                    this.FillListingInformation(listing);
                    this.FillRooms(listing);
                    this.FillFeatures(listing);
                    this.FillLotEnvironmentUtilityInformation(listing);
                    this.FillFinancialInformation(listing);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing as AborListingRequest);
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
            this.uploaderClient.NavigateToUrl("https://matrix.austinmls.com/Matrix/Input");
            // this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Input"));
            // this.uploaderClient.ClickOnElement(By.LinkText("Input"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Edit existing"));
            this.uploaderClient.ClickOnElement(By.LinkText("Edit existing"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_txtSourceCommonID"));
            this.uploaderClient.WriteTextbox(By.Id("m_txtSourceCommonID"), value: mlsNumber);
            this.uploaderClient.ClickOnElement(By.Id("m_lbEdit")); // "Modify button"
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
            this.uploaderClient.SetMultipleCheckboxById("Input_609", listing.GreenWaterConservation, fieldLabel: "Green Water Conservation", tabName);

            // Utilities
            this.uploaderClient.SetMultipleCheckboxById("Input_610", listing.HeatSystemDesc, "Heat", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_611", listing.CoolSystemDesc, "A/C ", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_612", listing.WaterDesc, "Water/Sewer", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_613", listing.SupOther, " Other Utilities", tabName);
        }

        private void FillFinancialInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Financial Information";
            this.uploaderClient.ExecuteScript(script: " jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Financial")); // Financial

            this.uploaderClient.SetMultipleCheckboxById("Input_614", "ATCLO,FUNDI", fieldLabel: "Possession (Max 7)", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_744", listing.ProposedTerms, fieldLabel: "Acceptable Financing", tabName); // Proposed Terms
            this.uploaderClient.SetMultipleCheckboxById("Input_616", listing.Exemptions, fieldLabel: "Exemptions", tabName); // Exemptions
            this.uploaderClient.WriteTextbox(By.Id("Input_618"), listing.TaxYear); // Tax Year
            this.uploaderClient.WriteTextbox(By.Id("Input_619"), listing.TaxRate); // Tax Rate
            if (!string.IsNullOrEmpty(listing.HOA))
            {
                this.uploaderClient.SetSelect(By.Id("Input_622"), value: listing.HOA, fieldLabel: "HOA Mandatory", tabName); // HOA Mandatory
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_623"), listing.AssocName); // HOA Name
            this.uploaderClient.WriteTextbox(By.Id("Input_624"), listing.AssocFee); // HOA Amount
            this.uploaderClient.SetSelect(By.Id("Input_625"), listing.AssocFeeFrequency, fieldLabel: "HOA Term", tabName); // HOA Term
            this.uploaderClient.WriteTextbox(By.Id("Input_626"), listing.ManagementCompany);  // HOA Mgmt Co
            this.uploaderClient.WriteTextbox(By.Id("Input_627"), listing.AssocPhone);  // HOA Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_628"), listing.HoaWebsite);  // HOA Website
            this.uploaderClient.WriteTextbox(By.Id("Input_629"), listing.AssocTransferFee);  // HOA Transfer fee
            this.uploaderClient.SetMultipleCheckboxById("Input_630", listing.AssocFeeIncludes, fieldLabel: "HOA Fee Includes", tabName);  // HOA Fee Includes
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Showing Information";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Brokerage/Showing Information")); // Brokerage/Showing Information

            // Brokerage
            this.uploaderClient.SetSelect(By.Id("Input_632"), listing.BuyerIncentiveDesc, fieldLabel: "Buyer Agency $ or %", tabName);
            this.uploaderClient.WriteTextbox(By.Id("Input_633"), listing.BuyerIncentive); // Buyer Agency Compensation
            this.uploaderClient.WriteTextbox(By.Id("Input_635"), value: "0"); // Sub Agency Compensation (default hardcode "0")
            this.uploaderClient.SetSelect(By.Id("Input_636"), value: "Pct", fieldLabel: "Sub Agency $ or % ", tabName); // Sub Agency $ or % (default hardcode "%")
            this.uploaderClient.SetSelect(By.Id("Input_637"), listing.ProspectsExempt, "Prospects Exempt", tabName); // Prospects Exempt (default hardcode "No")
            this.uploaderClient.WriteTextbox(By.Id("Input_638"), listing.TitleCo); // Pref Title Company
            this.uploaderClient.WriteTextbox(By.Id("Input_639"), listing.EarnestMoney); // Earnest Money

            // Showing
            this.uploaderClient.SetSelect(By.Id("Input_642"), listing.LockboxTypeDesc, fieldLabel: "Lockbox Type", tabName, isElementOptional: true);
            this.uploaderClient.WriteTextbox(By.Id("Input_648"), listing.AgentListApptPhone, isElementOptional: true);  // Showing Phone
            this.uploaderClient.WriteTextbox(By.Id("Input_649"), listing.OtherPhone, isElementOptional: true);  // Showing Phone #2
            this.uploaderClient.SetMultipleCheckboxById("Input_643", listing.LockboxLocDesc, fieldLabel: "Lockbox Location", tabName);
            this.uploaderClient.SetMultipleCheckboxById("Input_645", listing.Showing, fieldLabel: "Showing Instructions", tabName);

            // Syndication
            this.uploaderClient.SetSelect(By.Id("Input_651"), "1", fieldLabel: "IDX Opt In", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_652"), "1", fieldLabel: "Display on Internet", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_653"), "1", fieldLabel: "Display Address", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_654"), "1", fieldLabel: "Allow AVM", tabName);
            this.uploaderClient.SetSelect(By.Id("Input_655"), "1", fieldLabel: "Allow 3rd Party Comments", tabName);
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
