namespace Husa.Uploader.Core.Services
{
    using System.Text.RegularExpressions;
    using System.Threading;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;

    public class CtxUploadService : ICtxUploadService
    {
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly ISqlDataLoader sqlDataLoader;
        private readonly ApplicationOptions options;
        private readonly ILogger<CtxUploadService> logger;

        public CtxUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            ISqlDataLoader sqlDataLoader,
            ILogger<CtxUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.sqlDataLoader = sqlDataLoader ?? throw new ArgumentNullException(nameof(sqlDataLoader));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.CTX;

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public LoginResult Login()
        {
            var marketInfo = this.options.MarketInfo.Ctx;
            this.uploaderClient.DeleteAllCookies();

            // Connect to the login page
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("loginbtn"));

            this.uploaderClient.WriteTextbox(By.Name("username"), marketInfo.Username);
            this.uploaderClient.WriteTextbox(By.Name("password"), marketInfo.Password);
            this.uploaderClient.ClickOnElementById("loginbtn");

            Thread.Sleep(2000);

            // Use the same browser page NOT _blank
            Thread.Sleep(2000);

            if (this.uploaderClient.IsElementVisible(By.Id("NewsDetailDismiss")))
            {
                this.uploaderClient.ClickOnElementById("NewsDetailDismiss");
            }

            this.uploaderClient.ClickOnElement(By.LinkText("Skip"), shouldWait: false, waitTime: 0, isElementOptional: true);
            Thread.Sleep(2000);

            this.uploaderClient.NavigateToUrl("https://matrix.ctxmls.com/Matrix/Default.aspx?c=AAEAAAD*****AQAAAAAAAAARAQAAAFIAAAAGAgAAAAQ4OTQwDRsGAwAAAAQLPCUSDTUL&f=");
            Thread.Sleep(2000);

            return LoginResult.Logged;
        }

        public UploadResult Logout()
        {
            this.uploaderClient.NavigateToUrl(this.options.MarketInfo.Ctx.LogoutUrl);
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
                this.Login();

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
                listing = await this.sqlDataLoader.GetListingRequest(listing.ResidentialListingRequestID, this.CurrentMarket,  cancellationToken);
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                this.Login();
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
                    this.FillRooms(listing);
                    this.FillFeatures(listing);
                    this.FillLotEnvironmentUtilityInformation(listing);
                    this.FillFinancialInformation(listing);
                    this.FillShowingInformation(listing);
                    this.FillRemarks(listing);
                    // try { this.uploaderClient.Click(By.LinkText("Status")); } catch { }
                }
                catch
                {
                    return UploadResult.Failure;
                }

                if (listing.IsNewListing)
                {
                    await this.ProcessImages(listing, cancellationToken);
                    // FinalizeInsert(listing);
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
                this.Login();

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
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                this.Login();
                this.NavigateToQuickEdit(listing.MLSNum);

                // Enter Manage Photos
                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Photos"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Manage Photos"));

                // Prepare Media
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

                this.Login();
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

                this.Login();

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl03_m_divFooterContainer"), cancellationToken);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"), cancellationToken);
                this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));

                // Login(driver, listing);
                // QuickEdit(driver, listing);
                ////  var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1);
                // Thread.Sleep(1500);
                // switch (listing.ListStatus)
                // {
                //    #region Sold
                //    case "S":

                // driver.Click(By.PartialLinkText("Change to Sold"));
                //        Thread.Sleep(500);
                //        driver.WriteTextbox(By.Id("Input_1029"), listing.PendingDate);
                //        driver.WriteTextbox(By.Id("Input_1023"), listing.ClosedDate);
                //        driver.WriteTextbox(By.Id("Input_1034"), listing.SalesPrice);
                //        /*QLIST-85*/
                //        //driver.WriteTextbox(By.Id("Input_1032"), listing.RepairsAmount);
                //        driver.WriteTextbox(By.Id("Input_1032"), "0"); //RepairsAmount

                // //driver.WriteTextbox(By.Id("Input_1314"), listing.Loan1Amount);
                //        driver.WriteTextbox(By.Id("Input_1315"), listing.BuyersClsgCostPdbySell);
                //        driver.WriteTextbox(By.Id("Input_1033"), listing.SellerPoints);
                //        driver.WriteTextbox(By.Id("Input_1026"), listing.BuyerPoints);
                //        driver.WriteTextbox(By.Id("Input_1037"), "0");
                //        /*QLIST-85*/
                //        //driver.SetSelect(By.Id("Input_1142"), listing.PropConditionSale);
                //        driver.SetSelect(By.Id("Input_1142"), "EXCL"); //Condition Sale
                //        //driver.WriteTextbox(By.Id("Input_1036"), listing.SoldComments);

                // driver.WriteTextbox(By.Id("Input_1043"), listing.SellingAgentLicenseNum ?? "NONMBR");
                //        driver.SetMultipleCheckboxById("Input_1035", listing.SoldTerms);

                // break;

                // #endregion

                // #region Pending
                //    case "P":
                //        driver.Click(By.PartialLinkText("Change to Pending"));
                //        Thread.Sleep(500);
                //        driver.WriteTextbox(By.Id("Input_1056"), listing.PendingDate);
                //        driver.WriteTextbox(By.Id("Input_1063"), listing.EstClosedDate);
                //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                // break;
                //    #endregion

                // #region Pending Taking Backups

                // case "PB":
                //        driver.Click(By.LinkText("Change to Pending - Taking Backups"));

                // driver.WriteTextbox(By.Id("Input_1067"), listing.PendingDate);
                //        driver.WriteTextbox(By.Id("Input_1352"), listing.EstClosedDate);
                //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);
                //        break;

                // #endregion

                // #region Active

                // case "A":
                //        driver.Click(By.LinkText("Change to Active"));
                //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                // break;
                //    #endregion

                // #region Active Contingent

                // case "AC":
                //        driver.Click(By.LinkText("Change to Active Contingent"));
                //        Thread.Sleep(500);
                //        driver.WriteTextbox(By.Id("Input_1006"), listing.ContingencyDate);
                //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                // break;
                //    #endregion

                // #region Temporarily Off Market

                // case "T":
                //        driver.Click(By.LinkText("Change to Temporarily Off Market"));
                //        Thread.Sleep(500);
                //        driver.WriteTextbox(By.Id("Input_1018"), DateTime.Now.ToShortDateString());
                //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                // break;
                //    #endregion

                // #region Withdrawn

                // case "W":
                //        driver.Click(By.PartialLinkText("Change to Withdrawn"));
                //        Thread.Sleep(500);
                //        driver.WriteTextbox(By.Id("Input_1020"), DateTime.Now.ToShortDateString());

                // break;
                //    #endregion

                // default:
                //        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Austin Listing with Id '" + listing.ResidentialListingID + "'");
                // }
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

                this.Login();
                Thread.Sleep(1000);

                this.uploaderClient.ClickOnElement(By.LinkText(@"Edit Listing Details"), shouldWait: false, waitTime: 0, isElementOptional: false);
                this.NavigateToQuickEdit(listing.MLSNum);

                this.uploaderClient.SwitchTo("main");
                this.uploaderClient.SwitchTo("workspace");
                Thread.Sleep(1000);

                this.uploaderClient.ExecuteScript(" SP('7') ");
                Thread.Sleep(2000);

                var virtualTourResponse = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.SanAntonio, cancellationToken);
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

        private static List<string> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            var listRoomTypes = new List<string>();

            // Atrium
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMATR"))
            {
                listRoomTypes.Add("Atrium");
            }

            // Basement
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("BSMNT"))
            {
                listRoomTypes.Add("Basement");
            }

            // Rooms
            int numBedrooms = 0;
            if (listing.NumBedsMainLevel.HasValue)
            {
                numBedrooms = listing.NumBedsMainLevel.Value;
            }
            else if (listing.NumBedsOtherLevels.HasValue)
            {
                numBedrooms = listing.NumBedsOtherLevels.Value;
            }

            if (numBedrooms >= 1)
            {
                listRoomTypes.Add("BEDRO");
            }

            if (numBedrooms >= 2)
            {
                listRoomTypes.Add("Bedroom_II");
            }

            if (numBedrooms >= 3)
            {
                listRoomTypes.Add("Bedroom_III");
            }

            if (numBedrooms >= 4)
            {
                listRoomTypes.Add("Bedroom_IV");
            }

            // Bonus Room

            // Breakfast Room
            if (!string.IsNullOrEmpty(listing.DiningRoomDesc) && listing.DiningRoomDesc.Contains("DBRKA"))
            {
                listRoomTypes.Add("BKFRO");
            }

            // Converted Garage
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMCGR"))
            {
                listRoomTypes.Add("Converted_Garage");
            }

            // Dining Room
            if (!string.IsNullOrEmpty(listing.DiningRoomDesc) && (listing.DiningRoomDesc.Contains("DINL") || listing.DiningRoomDesc.Contains("DFMRM")))
            {
                listRoomTypes.Add("DININ");
            }

            // Entry Foyer
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMFYR"))
            {
                listRoomTypes.Add("Entry_Forer");
            }

            // Family Room
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMFAM"))
            {
                listRoomTypes.Add("FAMIL");
            }

            // Game Room
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMGAM"))
            {
                listRoomTypes.Add("GAMER");
            }

            // Great Room
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMGRT"))
            {
                listRoomTypes.Add("GreatRoom");
            }

            // Gym - Gym

            // Kitchen
            listRoomTypes.Add("KITCH");

            // Library
            if (!string.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMLIB"))
            {
                listRoomTypes.Add("Library");
            }

            // Living Room
            listRoomTypes.Add("LIVIN");

            // 'Living_Room_II', 'Living Room II',
            // 'Loft', 'Loft',
            // 'Master_Bath', 'Master Bath',
            // 'Master_Bath_II', 'Master Bath II',
            // 'MSTRB', 'Master Bedroom',
            // 'Master_Bedroom_II', 'Master Bedroom II',
            // 'MediaRoom', 'Media Room',
            // 'OFFIC', 'Office',
            // 'OTHER', 'Other',
            // 'Other_Room', 'Other Room',
            // 'Other_Room_II', 'Other Room II',
            // 'Other_Room_III', 'Other Room III',
            // 'SaunaRoom', 'Sauna Room',
            // 'UTILI', 'Utility\/Laundry ',
            // 'Wine', 'Wine',
            // 'WORKS', 'Workshop        ',
            // 'GuestHse', 'Guest House'
            return listRoomTypes;
        }

        private void NavigateToNewPropertyInput()
        {
            this.uploaderClient.NavigateToUrl("https://matrix.ctxmls.com/Matrix/Input");
            // this.uploaderClient.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Input")));
            // this.uploaderClient.Click(By.LinkText("Input"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Add new"));
            this.uploaderClient.ClickOnElement(By.LinkText("Add new"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Residential Input Form"));
            this.uploaderClient.ClickOnElement(By.LinkText("Residential Input Form"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.PartialLinkText("Start with a blank Listing"));
            this.uploaderClient.ClickOnElement(By.PartialLinkText("Start with a blank Listing"));

            Thread.Sleep(1000);
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl("https://matrix.ctxmls.com/Matrix/Input");
            // this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Input"));
            // this.uploaderClient.ClickOnElement(By.LinkText("Input"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Edit existing"));
            this.uploaderClient.ClickOnElement(By.LinkText("Edit existing"));
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_txtSourceCommonID"));
            this.uploaderClient.WriteTextbox(By.Id("m_txtSourceCommonID"), value: mlsNumber);
            this.uploaderClient.ClickOnElement(By.Id("m_lbEdit")); // "Modify button"
        }

        private void FillListingInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Listing Information";
            this.uploaderClient.ClickOnElement(By.LinkText("Listing Information")); // click in tab Listing Information
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("Input_107"));

            this.uploaderClient.WriteTextbox(By.Id("Input_107"), listing.StreetNum); // Street Number
            this.uploaderClient.SetSelect(By.Id("Input_108"), listing.StreetDir, fieldLabel: "St Direction", tabName); // St Direction
            this.uploaderClient.WriteTextbox(By.Id("Input_110"), listing.StreetName); // Street Name
            this.uploaderClient.SetSelect(By.Id("Input_109"), listing.StreetType, fieldLabel: "Street Type", tabName); // Street Type
            this.uploaderClient.WriteTextbox(By.Id("Input_111"), listing.UnitNum); // Unit #
            this.uploaderClient.SetSelect(By.Id("Input_112"), listing.CityCode, fieldLabel: "City", tabName); // City
            this.uploaderClient.SetSelect(By.Id("Input_113"), listing.StateCode, fieldLabel: "State", tabName); // State
            this.uploaderClient.SetSelect(By.Id("Input_123"), value: "YES", fieldLabel: "In City Limits", tabName); // In City Limits
            string ctxEtj = listing.InExtraTerritorialJurisdiction ? "1" : "0";
            this.uploaderClient.SetSelect(By.Id("Input_124"), ctxEtj, fieldLabel: "ETJ", tabName); // ETJ
            this.uploaderClient.WriteTextbox(By.Id("Input_114"), listing.Zip); // Zip Code
            this.uploaderClient.SetSelect(By.Id("Input_115"), listing.County, "County", tabName); // County
            this.uploaderClient.WriteTextbox(By.Id("Input_396"), listing.Subdivision); // Subdivision
            this.uploaderClient.WriteTextbox(By.Id("Input_528"), listing.Legal); // Legal Description
            this.uploaderClient.WriteTextbox(By.Id("Input_529"), listing.TaxID); // Property ID
            this.uploaderClient.WriteTextbox(By.Id("Input_766"), listing.GeographicID); // Geo ID
            this.uploaderClient.SetSelect(By.Id("Input_530"), value: "NO", fieldLabel: "FEMA Flood Plain", tabName); // FEMA Flood Plain
            this.uploaderClient.SetSelect(By.Id("Input_531"), value: "NO", fieldLabel: "Residential Flooded", tabName); // Residential Flooded
            this.uploaderClient.WriteTextbox(By.Id("Input_532"), listing.LotNum); // Lot
            this.uploaderClient.WriteTextbox(By.Id("Input_533"), listing.Block); // Block

            this.uploaderClient.SetSelect(By.Id("Input_535_TB"), listing.SchoolDistrict.ToUpper(), fieldLabel: "School District", tabName); // School District

            this.FillFieldSingleOption("Input_535", listing.SchoolDistrict.ToUpper());

            this.uploaderClient.SetSelect(By.Id("Input_658"), listing.SchoolName1.ToUpper(), fieldLabel: "Elementary School", tabName); // Elementary School
            this.uploaderClient.SetSelect(By.Id("Input_659"), listing.SchoolName2.ToUpper(), fieldLabel: "Middle School", tabName); // Middle School
            this.uploaderClient.SetSelect(By.Id("Input_660"), listing.SchoolName3.ToUpper(), fieldLabel: "High School", tabName); // High School

            // this.uploaderClient.WriteTextbox(By.Id("Input_125"), listing.MapscoMapCoord); // Map Grid
            // this.uploaderClient.WriteTextbox(By.Id("Input_126"), listing.MapscoMapPage); // Map Source
            this.SetLongitudeAndLatitudeValues(listing);
            this.uploaderClient.WriteTextbox(By.Id("Input_127"), listing.ListPrice); // List Price
            this.uploaderClient.WriteTextbox(By.Id("Input_133"), listing.OwnerName); // Owner Legal Name
            this.uploaderClient.SetSelect(By.Id("Input_137"), "0", "Also For Rent", tabName); // Also For Rent

            // 'CNDMI',
            // 'Condominium',
            // 'Garden_Patio_Home',
            // 'Garden\/Patio Home',
            // 'MNFHO',
            // 'Manufactured Home',
            // 'Modular',
            // 'Modular',
            // 'OTH',
            // 'Other',
            // 'SFM',
            // 'Single Family',
            // 'TNX',
            // 'Townhouse'
            var propSubType = listing.Category switch
            {
                "CM" or "LA" or "CL" or "MF" or "RR" or "FR" => "OTH",
                "CO" => "CNDMI",
                "RE" => "SFM",
                _ => string.Empty,
            };
            this.uploaderClient.SetSelect(By.Id("Input_539"), propSubType, "Property Type", tabName); // Property Type

            if (this.uploaderClient.UploadInformation.IsNewListing)
            {
                DateTime listDate = DateTime.Now;
                if (listing.ListStatus == "P" || listing.ListStatus == "PO")
                {
                    listDate = DateTime.Now.AddDays(-2);
                }
                else if (listing.ListStatus == "S")
                {
                    listDate = DateTime.Now.AddDays(-4);
                }

                this.uploaderClient.WriteTextbox(By.Id("Input_129"), listDate.ToShortDateString()); // List Date
            }

            if (listing.ListDate != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_130"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Id("Input_130"), listing.ExpiredDate != null ? ((DateTime)listing.ExpiredDate).ToShortDateString() : string.Empty); // Expiration Date
            }

            this.uploaderClient.SetSelect(By.Id("Input_544"), "NA", "First Right Refusal Option", tabName); // First Right Refusal Option (default hardcode "N/A")

            // this.uploaderClient.SetSelect(By.Id("Input_132"), "1", "Owner LREA", tabName); // Owner LREA (default hardcode "Yes")

            // this.uploaderClient.WriteTextbox(By.Id("Input_134"), listing.AgentListApptPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")); // Owner Phone
            // this.uploaderClient.WriteTextbox(By.Id("Input_134"), listing.OwnerPhone.Remove('(').Remove(')').Remove('-').Trim()); // Owner Phone
            // this.uploaderClient.SetSelect(By.Id("Input_135"), "NO", "Short Sale", tabName); // Short Sale
            // this.uploaderClient.SetSelect(By.Id("Input_136"), "0", "Foreclosure", tabName); // Foreclosure

            // this.uploaderClient.WriteTextbox(By.Id("Input_138"), ??? ); // Additional MLS #
            // this.uploaderClient.WriteTextbox(By.Id("Input_139"), listing.TaxRate); // Total Tax Rate
            this.uploaderClient.SetSelect(By.Id("Input_531"), "0", "Res Flooded", tabName); // Res Flooded

            string constructionStatus = string.Empty;
            switch (listing.YearBuiltDesc)
            {
                case "C":
                    constructionStatus = "COMPL";
                    break;
                case "I":
                    constructionStatus = "TOBEB";
                    break;
            }

            this.uploaderClient.SetSelect(By.Id("Input_547"), constructionStatus, "Construction Status", tabName); // Construction Status
            this.uploaderClient.WriteTextbox(By.Id("Input_548"), listing.OwnerName); // Builder Name
            this.uploaderClient.WriteTextbox(By.Id("Input_549"), listing.BuildCompletionDate); // Estimated Completion Date
            this.uploaderClient.WriteTextbox(By.Id("Input_553"), listing.YearBuilt); // Year Built
            this.uploaderClient.SetSelect(By.Id("Input_186"), "OWNSE", "Year Built Source", tabName); // Year Built Source (default hardcode "Owner/Seller")

            this.uploaderClient.WriteTextbox(By.Id("Input_550"), listing.SqFtTotal); // Total SqFt
            this.uploaderClient.SetSelect(By.Id("Input_551"), "BUILD", "Source SqFt", tabName); // Source SqFt

            // this.uploaderClient.SetMultipleCheckboxById("Input_551", listing, "Documents on File (Max 25)", tabName); //Documents on File (Max 25)
            // this.uploaderClient.SetMultipleCheckboxById("Input_219", listing.RestrictionsDesc); // Documents On File
            var putOptionsRestrictionsDesc = new List<string>();
            var optionsRestrictionsDesc = string.IsNullOrWhiteSpace(listing.RestrictionsDesc) ? new List<string>() : listing.RestrictionsDesc.Split(',').ToList();
            foreach (string option in optionsRestrictionsDesc)
            {
                switch (option)
                {
                    case "ADLT55":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ADLT62":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "BLDGST":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "BLDGSZ":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "CITRS":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "CVNANT":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "DEEDRS":
                        putOptionsRestrictionsDesc.Add("DEEDR");
                        break;
                    case "DVLMPT":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ENVRO":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ESMNT":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "LEASE":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "LMTDVH":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "LVSTCK":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "RSLIM":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "RUNKN":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ZONE":
                        // putOptionsRestrictionsDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // 'AERIA','Aerial Photos',
            // 'APPRA','Appraisal',
            // 'BOUND','Boundary Survey',
            // 'ENGIN','Engineer\'s Report',
            // 'FIELD','Field Notes',
            // 'FLODC','Flood Certification',
            // 'FLOOR','Floor Plans',
            // 'INHOM','Inspection-Home',
            // 'INPES','Inspection-Pest',
            // 'INSPT','Inspection-Septic',
            // 'LEADB','Lead Based Paint Addendum',
            // 'NONE','None',
            // 'ONSSE','On-Site Sewer Disclosure',
            // 'OTHSE','Other-See Remarks',
            // 'PLAT','Plat',
            // 'SELLE','Seller\'s Disclosure',
            // 'SPECI','Special Contract\/Addendum Required',
            // 'SBDRE','Subdivision Restrictions',
            // 'SURVE','Survey',
            // 'TOPOM','Topo Map'

            // if (putOptionsRestrictionsDesc.Count > 0)
            // {
            //    this.uploaderClient.SetMultipleCheckboxById("Input_219", string.Join(",", putOptionsRestrictionsDesc.ToArray())); // Documents On File
            // }
            this.uploaderClient.SetMultipleCheckboxById("Input_554", "NONE", "Documents On File", tabName); // Documents On File
        }

        private void FillFieldSingleOption(string fieldName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var mainWindow = this.uploaderClient.WindowHandles.FirstOrDefault(c => c == this.uploaderClient.CurrentWindowHandle);

                this.uploaderClient.ExecuteScript("jQuery('#" + fieldName + "_TB').focus();");
                this.uploaderClient.ExecuteScript("jQuery('#" + fieldName + "_A')[0].click();");
                this.uploaderClient.SwitchTo().Window(this.uploaderClient.WindowHandles.Last());
                Thread.Sleep(400);

                char[] fieldValue = value.ToArray();

                foreach (var charact in fieldValue)
                {
                    Thread.Sleep(400);
                    this.uploaderClient.FindElement(By.Id("m_txtSearch")).SendKeys(charact.ToString().ToUpper());
                }

                Thread.Sleep(400);

                this.uploaderClient.ExecuteScript("jQuery(\"li[title=^'" + value + "'])");
                Thread.Sleep(400);

                this.uploaderClient.ExecuteScript("let btnSave = jQuery(\"#cboxClose > button\")[0]; jQuery(btnSave).focus(); jQuery(btnSave).click();");

                this.uploaderClient.SwitchTo().Window(mainWindow);
            }
        }

        private void FillRooms(ResidentialListingRequest listing)
        {
            const string tabName = "Rooms";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Rooms")); // click in tab Listing Information

            // this.uploaderClient.Click(By.Id("m_rpPageList_ctl02_lbPageLink")); // Tab: Input | Subtab: Rooms
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("ctl02_m_divFooterContainer")); // Look if the footer elements has been loaded
            // Thread.Sleep(2000);

            // this.uploaderClient.WriteTextbox(By.Id("Input_193"), (listing.NumBedsMainLevel != null ? listing.NumBedsMainLevel : 0) + (listing.NumBedsOtherLevels != null ? listing.NumBedsOtherLevels : 0)); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_193"), listing.Beds); // Bedrooms
            this.uploaderClient.WriteTextbox(By.Id("Input_194"), listing.BathsFull); // Full Baths
            this.uploaderClient.WriteTextbox(By.Id("Input_195"), listing.BathsHalf); // Half Baths

            // this.uploaderClient.WriteTextbox(By.Id("Input_260"), listing.NumStories); // # Stories

            // this.uploaderClient.WriteTextbox(By.Id("Input_196"), listing.NumLivingAreas); // # Living Areas

            // this.uploaderClient.WriteTextbox(By.Id("Input_197"), listing.NumDiningAreas); // # Dining Areas
            List<string> optionsGarageDesc = string.IsNullOrWhiteSpace(listing.ParkingDesc) ? new List<string>() : listing.ParkingDesc.Split(',').ToList();
            foreach (string option in optionsGarageDesc)
            {
                switch (option)
                {
                    case "NONE":
                        // this.uploaderClient.SetSelect(By.Id("Input_198"), "0", "Garage/Carport", tabName); // Garage/Carport
                        break;
                }
            }

            // Market options that doesn't match
            // 'OTHSE','Other-See Remarks',
            // 'NONE','None'

            // if (optionsGarageDesc.Count > 0)
            // {
            //    this.uploaderClient.SetSelect(By.Id("Input_198"), "1", "Garage/Carport", tabName); // Garage/Carport
            // }

            // var guestHouse = listing.CTXGuestHouse ? "1" : "0";
            // this.uploaderClient.SetSelect(By.Id("Input_199"), guestHouse, "Guest House", tabName); // Garage/Carport
            if (!this.uploaderClient.UploadInformation.IsNewListing)
            {
                var elems = this.uploaderClient.FindElements(By.CssSelector("table[id^=_Input_556__del_REPEAT] a"));

                foreach (var elem in elems.Where(c => c.Displayed))
                {
                    elem.Click();
                }
            }

            var roomTypes = ReadRoomAndFeatures(listing);

            // this.uploaderClient.Click(By.Id("m_rpPageList_ctl04_lbPageLink"));
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Rooms")); // click in tab Listing Information
            Thread.Sleep(400);

            var i = 0;

            // foreach (var roomType in roomTypes.Where(c => c.IsValid()))
            foreach (var roomType in roomTypes)
            {
                if (i > 0)
                {
                    this.uploaderClient.ClickOnElement(By.Id("_Input_556_more"));
                    Thread.Sleep(400);
                }

                // 'Atrium', 'Atrium',
                // 'Basement', 'Basement',
                // 'BEDRO', 'Bedroom',
                // 'Bedroom_II', 'Bedroom II',
                // 'Bedroom_III', 'Bedroom III',
                // 'Bedroom_IV', 'Bedroom IV',
                // 'BonusRoom', 'Bonus Room',
                // 'BKFRO', 'Breakfast Room',
                // 'Converted_Garage', 'Converted Garage',
                // 'DININ', 'Dining Room',
                // 'Entry_Forer', 'Entry\/Foyer',
                // 'FAMIL', 'Family Room',
                // 'GAMER', 'Game Room',
                // 'GreatRoom', 'Great Room',
                // 'Gym', 'Gym',
                // 'KITCH', 'Kitchen',
                // 'Library', 'Library',
                // 'LIVIN', 'Living Room',
                // 'Living_Room_II', 'Living Room II',
                // 'Loft', 'Loft',
                // 'Master_Bath', 'Master Bath',
                // 'Master_Bath_II', 'Master Bath II',
                // 'MSTRB', 'Master Bedroom',
                // 'Master_Bedroom_II', 'Master Bedroom II',
                // 'MediaRoom', 'Media Room',
                // 'OFFIC', 'Office',
                // 'OTHER', 'Other',
                // 'Other_Room', 'Other Room',
                // 'Other_Room_II', 'Other Room II',
                // 'Other_Room_III', 'Other Room III',
                // 'SaunaRoom', 'Sauna Room',
                // 'UTILI', 'Utility\/Laundry ',
                // 'Wine', 'Wine',
                // 'WORKS', 'Workshop        ',
                // 'GuestHse', 'Guest House'
                this.uploaderClient.SetSelect(By.Id("_Input_190__REPEAT" + i + "_190"), roomType, "Name", tabName, true); // FieldName
                Thread.Sleep(400);
                // this.uploaderClient.ScrollDown();
                // this.uploaderClient.SetSelect(By.Id("_Input_190__REPEAT" + i + "_491"), roomType.Level, true);
                // Thread.Sleep(400);
                // this.uploaderClient.ScrollDown();
                // this.uploaderClient.WriteTextbox(By.Id("_Input_190__REPEAT" + i + "_191"), roomType.Length, true);
                // Thread.Sleep(400);
                // this.uploaderClient.ScrollDown();
                i++;
            }
        }

        private void FillFeatures(ResidentialListingRequest listing)
        {
            string tabName = "Features";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Features")); // click in tab Features

            // this.uploaderClient.SetMultipleCheckboxById("Input_156", "TRADI", "Style", tabName); // Style (default hardcode "Traditional")
            this.uploaderClient.SetMultipleCheckboxById("Input_557", listing.HousingTypeDesc, "Style", tabName); // Style

            // this.uploaderClient.SetMultipleCheckboxById("Input_159", listing.FoundationDesc); // Foundation
            var putFoundation = new List<string>();
            var optionsFoundation = string.IsNullOrWhiteSpace(listing.FoundationDesc) ? new List<string>() : listing.FoundationDesc.Split(',').ToList();
            foreach (string option in optionsFoundation)
            {
                switch (option)
                {
                    case "BSMNT":
                        putFoundation.Add("BASEM");
                        break;
                    case "CEDAR":
                        putFoundation.Add("CEDAR");
                        break;
                    case "OTHER":
                        putFoundation.Add("OTHSE");
                        break;
                    case "SLAB":
                        putFoundation.Add("SLAB");
                        break;
                }
            }

            // Market options that doesn't match
            // 'BASEM','Basement',
            // 'CEDAR','Cedar Post',
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            // 'PIER','Pier',
            if (putFoundation.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_558", string.Join(",", putFoundation.ToArray()), "Foundation", tabName); // Foundation
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_161", listing.NumStories); // # Stories
            var putOptionsNumStories = new List<string>();
            var optionsNumStories = string.IsNullOrWhiteSpace(listing.NumStories) ? new List<string>() : listing.NumStories.Split(',').ToList();
            foreach (string option in optionsNumStories)
            {
                switch (option)
                {
                    case "1":
                        putOptionsNumStories.Add("ONE");
                        break;
                    case "2":
                        putOptionsNumStories.Add("TWO");
                        break;
                    case "3":
                        putOptionsNumStories.Add("THREE");
                        break;
                    case "M":
                        putOptionsNumStories.Add("SPLIT");
                        break;
                }
            }

            // Market options that doesn't match
            // 'OTHSE','Other-See Remarks',
            // 'NONE','None'
            if (putOptionsNumStories.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_563", string.Join(",", putOptionsNumStories.ToArray()), "# Stories", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_559", listing.RoofDesc); // Roof / Attic
            var putOptionsRoofDesc = new List<string>();
            var optionsRoofDesc = string.IsNullOrWhiteSpace(listing.RoofDesc) ? new List<string>() : listing.RoofDesc.Split(',').ToList();
            foreach (string option in optionsRoofDesc)
            {
                switch (option)
                {
                    case "BLTUP":
                        putOptionsRoofDesc.Add("GRAVE");
                        break;
                    case "CLAY":
                        // putOptionsRoofDesc.Add("");
                        break;
                    case "COMP":
                        putOptionsRoofDesc.Add("SHNGC");
                        break;
                    case "CONCR":
                        putOptionsRoofDesc.Add("CONCR");
                        break;
                    case "FLAT":
                        putOptionsRoofDesc.Add("FLAT");
                        break;
                    case "HVCMP":
                        // putOptionsRoofDesc.Add("");
                        break;
                    case "METAL":
                        putOptionsRoofDesc.Add("METAL");
                        break;
                    case "NA":
                        // putOptionsRoofDesc.Add("");
                        break;
                    case "OTHER":
                        putOptionsRoofDesc.Add("OTHRO");
                        break;
                    case "SLATE":
                        putOptionsRoofDesc.Add("SLATE");
                        break;
                    case "TILE":
                        putOptionsRoofDesc.Add("TILE");
                        break;
                    case "WOOD":
                        putOptionsRoofDesc.Add("WOOD");
                        break;
                    case "WDSHN":
                        // putOptionsRoofDesc.Add("");
                        break;
                }
            }

            if (putOptionsRoofDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_559", string.Join(",", putOptionsRoofDesc.ToArray()), "Roof / Attic", tabName);
            }

            var putOptionsConstructionDesc = new List<string>();
            var optionsConstructionDesc = string.IsNullOrWhiteSpace(listing.ConstructionDesc) ? new List<string>() : listing.ConstructionDesc.Split(',').ToList();
            foreach (string option in optionsConstructionDesc)
            {
                switch (option)
                {
                    case "1SIDEMASONRY":
                        putOptionsConstructionDesc.Add("1SIDE");
                        break;
                    case "2SIDEMASONRY":
                        // putOptionsConstructionDesc.Add("");
                        break;
                    case "3SDMS":
                        putOptionsConstructionDesc.Add("3SIDE");
                        break;
                    case "4SDMS":
                        putOptionsConstructionDesc.Add("4SIDE");
                        break;
                    case "ALUMN":
                        putOptionsConstructionDesc.Add("Alluminum_Siding");
                        break;
                    case "ASBSH":
                        putOptionsConstructionDesc.Add("ASBES");
                        break;
                    case "BRICK":
                        putOptionsConstructionDesc.Add("BRICK");
                        break;
                    case "CMTFB":
                        putOptionsConstructionDesc.Add("CONCR");
                        break;
                    case "LOG":
                        putOptionsConstructionDesc.Add("LOG");
                        break;
                    case "MANSONRYSTEEL":
                        putOptionsConstructionDesc.Add("MASON");
                        break;
                    case "METAL":
                        putOptionsConstructionDesc.Add("Metal_Structure");
                        break;
                    case "OTHER":
                        putOptionsConstructionDesc.Add("OTHSE");
                        break;
                    case "ROCKSTONE":
                        putOptionsConstructionDesc.Add("ROCKS");
                        break;
                    case "STCCO":
                        putOptionsConstructionDesc.Add("STUCC");
                        break;
                    case "VINYL":
                        putOptionsConstructionDesc.Add("VINYL");
                        break;
                    case "WOOD":
                        putOptionsConstructionDesc.Add("WOOD");
                        break;
                }
            }

            if (putOptionsConstructionDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_561", string.Join(",", putOptionsConstructionDesc.ToArray()), "Construction/Exterior", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_163", listing.FireplaceDesc); // Fireplace
            var putOptionsFireplaceDesc = new List<string>();
            var optionsFireplaceDesc = string.IsNullOrWhiteSpace(listing.FireplaceDesc) ? new List<string>() : listing.FireplaceDesc.Split(',').ToList();
            foreach (string option in optionsFireplaceDesc)
            {
                switch (option)
                {
                    case "DINRM":
                        // putOptionsFireplaceDesc.Add("");
                        break;
                    case "FAMRM":
                        putOptionsFireplaceDesc.Add("GREAT");
                        break;
                    case "GAMERM":
                        putOptionsFireplaceDesc.Add("GAMER");
                        break;
                    case "GAS":
                        // putOptionsFireplaceDesc.Add("");
                        break;
                    case "LGINC":
                        putOptionsFireplaceDesc.Add("GASLO");
                        break;
                    case "GASSTARTER":
                        putOptionsFireplaceDesc.Add("GASST");
                        break;
                    case "GLASSENCLSCREEN":
                        putOptionsFireplaceDesc.Add("GLASS");
                        break;
                    case "HEATILATOR":
                        putOptionsFireplaceDesc.Add("HEATI");
                        break;
                    case "KITCHEN":
                        putOptionsFireplaceDesc.Add("KITCH");
                        break;
                    case "LIVRM":
                        putOptionsFireplaceDesc.Add("LIVIN");
                        break;
                    case "MSTBD":
                        putOptionsFireplaceDesc.Add("BEDRO");
                        break;
                    case "MOCK":
                        // putOptionsFireplaceDesc.Add("");
                        break;
                    case "NA":
                        putOptionsFireplaceDesc.Add("NONE");
                        break;
                    case "ONE":
                        putOptionsFireplaceDesc.Add("ONE");
                        break;
                    case "OTHER":
                        putOptionsFireplaceDesc.Add("OTHSE");
                        break;
                    case "PREFAB":
                        putOptionsFireplaceDesc.Add("PREFA");
                        break;
                    case "STONEROCKBRICK":
                        putOptionsFireplaceDesc.Add("STONE");
                        break;
                    case "STOVEINSERT":
                        putOptionsFireplaceDesc.Add("STOVE");
                        break;
                    case "3+":
                        putOptionsFireplaceDesc.Add("THREE");
                        break;
                    case "TWO":
                        putOptionsFireplaceDesc.Add("TWO");
                        break;
                    case "WDBRN":
                        putOptionsFireplaceDesc.Add("WOODB");
                        break;
                }
            }

            // Market options that doesn't match
            // 'ELECT','Electric',
            // 'GASST','Gas Starter',
            // 'GLASS','Glass\/Enclosed Screen',
            // 'NONE','None',
            // 'ONE','One',
            // 'OTHSE','Other-See Remarks',
            // 'PREFA','Prefab',
            // 'STONE','Stone\/Rock\/Brick',
            // 'STOVE','Stove Insert',
            // 'THREE','Three+',
            // 'TWO','Two',
            if (putOptionsFireplaceDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_562", string.Join(",", putOptionsFireplaceDesc.ToArray()), "Fireplace", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_160", listing.FloorsDesc); // Flooring
            var putOptionsFloorsDesc = new List<string>();
            var optionsFloorsDesc = string.IsNullOrWhiteSpace(listing.FloorsDesc) ? new List<string>() : listing.FloorsDesc.Split(',').ToList();
            foreach (string option in optionsFloorsDesc)
            {
                switch (option)
                {
                    case "BRICK":
                        putOptionsFloorsDesc.Add("BRICK");
                        break;
                    case "CRPT":
                        putOptionsFloorsDesc.Add("CARPE");
                        break;
                    case "CTILE":
                        // putOptionsFloorsDesc.Add("");
                        break;
                    case "LMNAT":
                        putOptionsFloorsDesc.Add("LAMIN");
                        break;
                    case "LNLEM":
                        // putOptionsFloorsDesc.Add("");
                        break;
                    case "MRBLE":
                        putOptionsFloorsDesc.Add("MARBL");
                        break;
                    case "OTHER":
                        putOptionsFloorsDesc.Add("OTHSE");
                        break;
                    case "PRQUT":
                        putOptionsFloorsDesc.Add("PARQU");
                        break;
                    case "STILE":
                        // putOptionsFloorsDesc.Add("");
                        break;
                    case "SLATE":
                        putOptionsFloorsDesc.Add("SLATE");
                        break;
                    case "STDCT":
                        // putOptionsFloorsDesc.Add("");
                        break;
                    case "STONE":
                        putOptionsFloorsDesc.Add("STONE");
                        break;
                    case "TERRAZZO":
                        putOptionsFloorsDesc.Add("TERRA");
                        break;
                    case "UNSTAINEDCONCRETE":
                        // putOptionsFloorsDesc.Add("");
                        break;
                    case "VINYL":
                        putOptionsFloorsDesc.Add("VINYL");
                        break;
                    case "WOOD":
                        putOptionsFloorsDesc.Add("WOOD");
                        break;
                }
            }

            // Market options that doesn't match
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            if (putOptionsFloorsDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_565", string.Join(",", putOptionsFloorsDesc.ToArray()), "Flooring", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_164", listing.KitchenDesc); // Kitchen Features
            var putOptionsKitchenDesc = new List<string>();
            var optionsKitchenDesc = string.IsNullOrWhiteSpace(listing.KitchenDesc) ? new List<string>() : listing.KitchenDesc.Split(',').ToList();
            foreach (string option in optionsKitchenDesc)
            {
                switch (option)
                {
                    case "2KTCH":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "BRAREA":
                        putOptionsKitchenDesc.Add("BKFAR");
                        break;
                    case "BRFAR":
                        putOptionsKitchenDesc.Add("BKFBA");
                        break;
                    case "CNISL":
                        putOptionsKitchenDesc.Add("Center_Island");
                        break;
                    case "CORTP":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "GLYTP":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "GMCNT":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "KTNET":
                        putOptionsKitchenDesc.Add("Kitchenette");
                        break;
                    case "NSTCN":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "OFMRM":
                        putOptionsKitchenDesc.Add("Open_to_Fam_Rm");
                        break;
                    case "SLSTNC":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "TILEC":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WPANT":
                        putOptionsKitchenDesc.Add("Walk_in_Pantry");
                        break;
                }
            }

            List<string> optionsAppliancesDesc = string.IsNullOrWhiteSpace(listing.InclusionsDesc) ? new List<string>() : listing.InclusionsDesc.Split(',').ToList();
            foreach (string option in optionsAppliancesDesc)
            {
                switch (option)
                {
                    case "AENON":
                        putOptionsKitchenDesc.Add("NONE");
                        break;
                    case "BICMK":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "BLTOV":
                        putOptionsKitchenDesc.Add("BLINO");
                        break;
                    case "CKTPE":
                        putOptionsKitchenDesc.Add("ELECT");
                        putOptionsKitchenDesc.Add("COOKT");
                        break;
                    case "CKTPG":
                        putOptionsKitchenDesc.Add("GAS");
                        putOptionsKitchenDesc.Add("COOKT");
                        break;
                    case "CNVCM":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "COFF":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "CONVE":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "DBLOV":
                        putOptionsKitchenDesc.Add("DOUBL");
                        break;
                    case "DRYER":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "DSPSL":
                        putOptionsKitchenDesc.Add("DISPO");
                        break;
                    case "DSWSR":
                        putOptionsKitchenDesc.Add("DISHW");
                        break;
                    case "DWNDF":
                        putOptionsKitchenDesc.Add("DOWND");
                        break;
                    case "ELCWT":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "ENGAP":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "EXFVT":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "GASWH":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "GEOTH":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "HASYS":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "IHWTR":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "MCWOV":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "RCXHF":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "RECWT":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "RFGTR":
                        putOptionsKitchenDesc.Add("REFRI");
                        break;
                    case "RFSZT":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "RNGFS":
                        putOptionsKitchenDesc.Add("RANGE");
                        break;
                    case "SCLOV":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "SNGOV":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "SOLWH":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "STKDW":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "STVCO":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "TNKWH":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "TSCMP":
                        putOptionsKitchenDesc.Add("TRASH");
                        break;
                    case "WINRE":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WSHR":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WTFLL":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WTFLO":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WTSFL":
                        // putOptionsKitchenDesc.Add("");
                        break;
                    case "WTSFO":
                        // putOptionsKitchenDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // 'BLINM','Built-In Microwave',
            // 'CUSTO','Custom Cabinets',
            // 'ICEM','Ice Maker',
            // 'ICEMC','Ice Maker Connection',
            // 'ISLAN','Island',
            // 'OTHSE','Other-See Remarks',
            // 'PANTR','Pantry',
            // 'CNTOP','Solid Counter Tops',
            // 'VENTH','Vent Hood',
            // 'Eat_in_Kitchen','Eat in Kitchen',
            if (optionsKitchenDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_566", string.Join(",", putOptionsKitchenDesc.ToArray()), "Kitchen Features", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_165", listing.LaundryFacilityDesc); // Laundry
            var putOptionsLaundryFacilityDesc = new List<string>();
            var optionsLaundryFacilityDesc = string.IsNullOrWhiteSpace(listing.LaundryFacilityDesc) ? new List<string>() : listing.LaundryFacilityDesc.Split(',').ToList();
            foreach (string option in optionsLaundryFacilityDesc)
            {
                switch (option)
                {
                    case "BATH":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "CHTE":
                        putOptionsLaundryFacilityDesc.Add("Chute");
                        break;
                    case "COFF":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "CRPT":
                        putOptionsLaundryFacilityDesc.Add("Carport");
                        break;
                    case "CSTL":
                        putOptionsLaundryFacilityDesc.Add("CLOSE");
                        break;
                    case "ELCCN":
                        putOptionsLaundryFacilityDesc.Add("DRYER");
                        putOptionsLaundryFacilityDesc.Add("ELECT");
                        break;
                    case "GASCN":
                        putOptionsLaundryFacilityDesc.Add("DRYER");
                        putOptionsLaundryFacilityDesc.Add("GAS");
                        break;
                    case "GRGE":
                        putOptionsLaundryFacilityDesc.Add("INGAR");
                        break;
                    case "HALL":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "INUNT":
                        putOptionsLaundryFacilityDesc.Add("INEAC");
                        break;
                    case "KTCH":
                        putOptionsLaundryFacilityDesc.Add("INKIT");
                        break;
                    case "LFAGN":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LFCOM":
                        putOptionsLaundryFacilityDesc.Add("COMMO");
                        break;
                    case "LFNON":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LFSEP":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LUPL":
                        putOptionsLaundryFacilityDesc.Add("UPPER");
                        break;
                    case "LURM":
                        putOptionsLaundryFacilityDesc.Add("Utility_Laundry");
                        break;
                    case "MNLV":
                        putOptionsLaundryFacilityDesc.Add("MAINL");
                        break;
                    case "MULMA":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "NOCNS":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "SINK":
                        putOptionsLaundryFacilityDesc.Add("Sink");
                        break;
                    case "STCON":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "UTRM":
                        // putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "WSHCN":
                        putOptionsLaundryFacilityDesc.Add("WASHE");
                        break;
                }
            }

            // Market options that doesn't match
            // 'BASEM','Basement',
            // 'INSID','Inside',
            // 'LAUND','Laundry Room',
            // 'LOWER','Lower Level',
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            if (putOptionsLaundryFacilityDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_567", string.Join(",", putOptionsLaundryFacilityDesc.ToArray()), "Laundry", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_166", listing.InteriorDesc); // Interior Features
            var putOptionsInteriorDesc = new List<string>();
            var optionsInteriorDesc = string.IsNullOrWhiteSpace(listing.InteriorDesc) ? new List<string>() : listing.InteriorDesc.Split(',').ToList();
            foreach (string option in optionsInteriorDesc)
            {
                switch (option)
                {
                    case "BBKCA":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "BENTC":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "BPANT":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "BSAFE":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CDCLS":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CLBM":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CLCFF":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CLCTH":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CLHIH":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CLVLT":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "CRMLD":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "DBMWT":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "ELVTR":
                        putOptionsInteriorDesc.Add("ELEVA");
                        break;
                    case "FRAST":
                        putOptionsInteriorDesc.Add("CARBO");
                        break;
                    case "FRNDR":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "IILPL":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "INTCM":
                        putOptionsInteriorDesc.Add("INTER");
                        break;
                    case "INUTL":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "IWICL":
                        putOptionsInteriorDesc.Add("WALKI");
                        break;
                    case "LNVOC":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "LRECE":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "MURBD":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsInteriorDesc.Add("NONE");
                        break;
                    case "POCKD":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "PSHUT":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "SCSYL":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "SCSYO":
                        putOptionsInteriorDesc.Add("SECSY");
                        break;
                    case "SHTRS":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "SKYLG":
                        putOptionsInteriorDesc.Add("SKYLI");
                        break;
                    case "SMKDT":
                        putOptionsInteriorDesc.Add("SMOKE");
                        break;
                    case "TKLGH":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "WETBR":
                        putOptionsInteriorDesc.Add("WETBA");
                        break;
                    case "WNTRM":
                        putOptionsInteriorDesc.Add("WINDO");
                        break;
                    case "WRSCT":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "WRSSD":
                        // putOptionsInteriorDesc.Add("");
                        break;
                    case "WRSTR":
                        putOptionsInteriorDesc.Add("WSPEAK");
                        break;
                }
            }

            // Market options that doesn't match
            // 'ATTIC','Attic Fan',
            // 'CEILI','Ceiling Fans ',
            // 'CNTRD','Central Distribution Plumbing System',
            // 'CNTRV','Central Vacuum',
            // 'FINIS','Finished Basment',
            // 'GAMER','Game Room',
            // 'GARDE','Garden Tub',
            // 'HANDI','Handicap Design',
            // 'HIGHC','High Ceilings',
            // 'INLAW','In-Law Suites',
            // 'MSTRD','Master Down',
            // 'MSTRU','Master Up',
            // 'MLTDI','Multiple Dining Areas',
            // 'MLTLI','Multiple Living Areas',
            // 'OFFIC','Office',
            // 'OTHSE','Other-See Remarks',
            // 'SAUNA','Sauna',
            // 'SEPAR','Separate Shower',
            // 'SHOWO','Shower Only',
            // 'SHOWT','Shower\/Tub Combo',
            // 'SPLIT','Split Bedroom',
            // 'UNFIN','Unfinished Basement',
            // 'WTRSO','Water Softener-Lease',
            // 'WASOO','Water Softner-Owned',
            // 'WHEEL','Wheelchair Access',
            // 'WHIRL','Whirlpool',
            if (putOptionsInteriorDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_571", string.Join(",", putOptionsInteriorDesc.ToArray()), "Interior Features", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_167", listing.GarageDesc); // Garage/Carport
            var putOptionsGarageDesc = new List<string>();
            var optionsGarageDesc = string.IsNullOrWhiteSpace(listing.GarageDesc) ? new List<string>() : listing.GarageDesc.Split(',').ToList();
            foreach (string option in optionsGarageDesc)
            {
                switch (option)
                {
                    case "GATCH":
                        putOptionsGarageDesc.Add("ATTAC");
                        break;
                    case "GDMLP":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "GDROP":
                        putOptionsGarageDesc.Add("GARAG");
                        break;
                    case "GDRSG":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "GDTCH":
                        putOptionsGarageDesc.Add("DETGA");
                        break;
                    case "GENFR":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "GENRR":
                        putOptionsGarageDesc.Add("REENG");
                        break;
                    case "GENRV":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "GENSD":
                        putOptionsGarageDesc.Add("SIENG");
                        break;
                    case "GENSI":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "GOLFC":
                        // putOptionsGarageDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsGarageDesc.Add("NONEG");
                        break;
                    case "PARGA":
                        // putOptionsGarageDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // '1CARC','1 Car Carport',
            // '1CARG','1 Car Garage',
            // '2CARC','2 Car Carport',
            // '2CARG','2 Car Garage',
            // '3PLCA','3+ Car Carport',
            // '3PLGA','3+ Car Garage',
            // 'CIRCU','Circular Drive',
            // 'CONVE','Converted to Living Space',
            // 'DETCA','Detached Carport',
            // 'NONEC','None-Carport',
            // 'OTHCA','Other Carport-See Remarks',
            // 'OTHGA','Other Garage-See Remarks',
            // 'OVRCA','Oversized Carport',
            // 'OVRGA','Oversized Garage',
            // 'REENC','Rear Entry Carport',
            // 'RVPAR','RV Parking Available',
            // 'SIENC','Side Entry Carport',
            if (putOptionsGarageDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_570", string.Join(",", putOptionsGarageDesc.ToArray()), "Garage/Carport", tabName);
            }
        }

        private void FillLotEnvironmentUtilityInformation(ResidentialListingRequest listing)
        {
            string tabName = "Lot/Environment/Utility";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Lot/Environment/Utility")); // Lot/Environment/Utility

            this.uploaderClient.WriteTextbox(By.Id("Input_576"), listing.LotDim); // Lot Dimensions
            // this.uploaderClient.WriteTextbox(By.Id("Input_170"), ??? ); // Apx Acreage
            this.uploaderClient.SetSelect(By.Id("Input_578"), value: "0", fieldLabel: "Manufactured Allowed", tabName); // Manufactured Allowed (default hardcode "No")

            // this.uploaderClient.SetMultipleCheckboxById("Input_172", listing.FenceDesc); // Fencing
            var putOptionsFenceDesc = new List<string>();
            var optionsFenceDesc = string.IsNullOrWhiteSpace(listing.ExteriorDesc) ? new List<string>() : listing.ExteriorDesc.Split(',').ToList();
            foreach (string option in optionsFenceDesc)
            {
                switch (option)
                {
                    case "ADDDW":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "CHLNK":
                        putOptionsFenceDesc.Add("CHAIN");
                        break;
                    case "CROSSFENCE":
                        putOptionsFenceDesc.Add("CROSS");
                        break;
                    case "DECBR":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "DGRUN":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "DTQTR":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "GAZE":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "GRILL":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "HANGR":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "HORSE":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "OTKT":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "PRFNC":
                        putOptionsFenceDesc.Add("PARTI");
                        break;
                    case "PRSPR":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "PTSLB":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "PVFNC":
                        putOptionsFenceDesc.Add("PRIVA");
                        break;
                    case "RANCHFENCE":
                        putOptionsFenceDesc.Add("RANCH");
                        break;
                    case "SPCL":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "SPSYS":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "STONE":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "TREES":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "WFIMPROV":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "WFUNIMPROV":
                        // putOptionsFenceDesc.Add("");
                        break;
                    case "WIRE":
                        putOptionsFenceDesc.Add("Wire");
                        break;
                    case "WRGHT":
                        putOptionsFenceDesc.Add("WROUG");
                        break;
                }
            }

            if (putOptionsFenceDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_581", string.Join(",", putOptionsFenceDesc.ToArray()), "Fencing", tabName);
            }

            this.uploaderClient.SetSelect(By.Id("Input_582"), listing.IsWaterFront, "Waterfront", tabName); // Waterfront
            this.uploaderClient.SetMultipleCheckboxById("Input_585", "NONE", "Water Features", tabName); // Water Features
            /*List<String> putOptionsWaterfrontDesc = new List<String>();
            List<String> optionsWaterfrontDesc = String.IsNullOrWhiteSpace(listing.WaterfrontDesc) ? new List<String>() : listing.WaterfrontDesc.Split(',').ToList();
            foreach (string option in optionsWaterfrontDesc)
            {
                switch (option)
                {
                    case "CANALFRT":
                        putOptionsWaterfrontDesc.Add("CANAL");
                        break;
                    case "CREEKFRT":
                        putOptionsWaterfrontDesc.Add("CREK");
                        break;
                    case "LAKEFRT":
                        putOptionsWaterfrontDesc.Add("LAKE");
                        break;
                    case "PONDFRT":
                        putOptionsWaterfrontDesc.Add("PONDS");
                        break;
                    case "RIVERFRT":
                        putOptionsWaterfrontDesc.Add("RIVER");
                        break;
                }
            }*/
            // Market options that doesn't match
            // 'BLTLK','Belton Lake',
            // 'BLANC','Blanco River',
            // 'BRAZ','Brazos River',
            // 'CANYO','Canyon Lake\/U.S. Corp of Engineers',
            // 'CITYS','City Skyline View',
            // 'COMAL','Comal River',
            // 'COLOR','Colorado River',
            // 'COUNT','Countryside View',
            // 'CREKS','Creek-Seasonal',
            // 'GUADA','Guadalupe River',
            // 'HILLC','Hill Country View',
            // 'LKAUS','Lake Austin',
            // 'BAST','Lake Bastrop',
            // 'BUCKN','Lake Buchanan',
            // 'LAKED','Lake Dunlap',
            // 'LAKEL','Lake LBJ',
            // 'LAKEM','Lake McQueeney',
            // 'LAKEP','Lake Placid',
            // 'LAKES','Lake Seguin',
            // 'TRAV','Lake Travis',
            // 'MEADO','Meadow Lake',
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            // 'PEDER','Pedernales River',
            // 'SANMA','San Marcos River',
            // 'STHLK','Stillhouse Hollow Lake',
            // 'WTRAC','Water Access',
            // 'WTRVI','Water View',
            // 'WTRFR','Waterfront'

            /*if (putOptionsWaterfrontDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_585", string.Join(",", putOptionsWaterfrontDesc.ToArray())); // Water Features
            }*/

            // this.uploaderClient.SetMultipleCheckboxById("Input_171", "SPRIN"); // Exterior Features (default hardcode "Sprinkler System")
            var putOptionsExteriorDesc = new List<string>();
            var optionsExteriorDesc = string.IsNullOrWhiteSpace(listing.ExteriorDesc) ? new List<string>() : listing.ExteriorDesc.Split(',').ToList();
            foreach (string option in optionsExteriorDesc)
            {
                switch (option)
                {
                    case "ADDDW":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "BBQ":
                        putOptionsExteriorDesc.Add("BBQGR");
                        break;
                    case "BOATHOUSE":
                        putOptionsExteriorDesc.Add("BOATH");
                        break;
                    case "CHLNK":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "CROSSFENCE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "CVPAT":
                        putOptionsExteriorDesc.Add("CPATIO");
                        break;
                    case "DBLPN":
                        putOptionsExteriorDesc.Add("DOUBL");
                        break;
                    case "DECBR":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "DGRUN":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "DK/BL":
                        putOptionsExteriorDesc.Add("DECK");
                        break;
                    case "DOCK":
                        putOptionsExteriorDesc.Add("DOCK");
                        break;
                    case "DTQTR":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "GARAGEAPT":
                        putOptionsExteriorDesc.Add("GARAG");
                        break;
                    case "GAZE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "GLASSEDINPORCH":
                        putOptionsExteriorDesc.Add("GLASS");
                        break;
                    case "GRILL":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "GTTRS":
                        putOptionsExteriorDesc.Add("GUTTE");
                        break;
                    case "HANGR":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "HORSE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsExteriorDesc.Add("NONE");
                        break;
                    case "OTHER":
                        putOptionsExteriorDesc.Add("OTHSE");
                        break;
                    case "OTKT":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "PRFNC":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "PRSPR":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "PTSLB":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "PVFNC":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "PVTEN":
                        putOptionsExteriorDesc.Add("TENNI");
                        break;
                    case "RANCHFENCE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "SCREENEDPORCH":
                        putOptionsExteriorDesc.Add("SCREE");
                        break;
                    case "SOLAR":
                        putOptionsExteriorDesc.Add("SOLAR");
                        break;
                    case "SPCL":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "SPSYS":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "STONE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "STORMDOORS":
                        putOptionsExteriorDesc.Add("STODO");
                        break;
                    case "STRG":
                        putOptionsExteriorDesc.Add("STORA");
                        break;
                    case "STRWN":
                        putOptionsExteriorDesc.Add("STOWI");
                        break;
                    case "TREES":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "WFIMPROV":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "WFUNIMPROV":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "WIRE":
                        // putOptionsExteriorDesc.Add("");
                        break;
                    case "WKSHP":
                        putOptionsExteriorDesc.Add("WORKS");
                        break;
                    case "WRGHT":
                        // putOptionsExteriorDesc.Add("");
                        break;
                }
            }

            if (putOptionsExteriorDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_596", string.Join(",", putOptionsExteriorDesc.ToArray()), "Exterior Features", tabName);
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_175", listing.LotDesc); // Topo/Land Description
            var putOptionsLotDesc = new List<string>();
            var optionsLotDesc = string.IsNullOrWhiteSpace(listing.LotDesc) ? new List<string>() : listing.LotDesc.Split(',').ToList();
            foreach (string option in optionsLotDesc)
            {
                switch (option)
                {
                    case "ALLEYACC":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "BACKGOLF":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "BACKPARK":
                        putOptionsLotDesc.Add("GREEN");
                        break;
                    case "CANAL":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "CORNER":
                        putOptionsLotDesc.Add("CORNE");
                        break;
                    case "CULDESAC":
                        putOptionsLotDesc.Add("CULDE");
                        break;
                    case "CULTVTD":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "CURBS":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "FLAGLOT":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "GOLFCOMM":
                        putOptionsLotDesc.Add("ONGOL");
                        break;
                    case "INTERIOR":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "IRREG":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "LAKE":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "LAKEFRT":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "LEVEL":
                        putOptionsLotDesc.Add("LEV");
                        break;
                    case "NOBKYDGR":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "OPEN":
                        putOptionsLotDesc.Add("OPEN");
                        break;
                    case "POND":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "PRIVROAD":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "PUBLROAD":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "ROLLING":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "SLOPE":
                        putOptionsLotDesc.Add("SLOPI");
                        break;
                    case "STREAM":
                        // putOptionsLotDesc.Add("");
                        break;
                    case "WOODED":
                        putOptionsLotDesc.Add("WOODE");
                        break;
                    case "XERISCAP":
                        // putOptionsLotDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // 'AGEXE','Ag Exempt',
            // 'BORDE','Borders State Park\/Game Ranch',
            // 'FLODP','Flood Plain',
            // 'GENTL','Gentle Rolling',
            // 'HORSE','Horse Property',
            // 'HUNTI','Hunting Permitted',
            // 'MATUR','Mature Trees',
            // 'MOBIL','Mobile Home Park',
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            // 'PRTWO','Partially Wooded',
            // 'RANCH','Ranch',
            // 'SECLU','Secluded',
            if (putOptionsLotDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_597", string.Join(",", putOptionsLotDesc.ToArray()), "Topo/Land Description", tabName); // Topo/Land Description
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_176", listing.CommonFeatures); // Neighborhood Amenities
            var putOptionsCommonFeatures = new List<string>();
            var optionsLotCommonFeatures = string.IsNullOrWhiteSpace(listing.CommonFeatures) ? new List<string>() : listing.CommonFeatures.Split(',').ToList();
            foreach (string option in optionsLotCommonFeatures)
            {
                switch (option)
                {
                    case "CLSTRMBX":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "CLUBHSE":
                        putOptionsCommonFeatures.Add("CLUBH");
                        break;
                    case "COMMONS":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "DOGPRK":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "ELEVATOR":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "EQUEST":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "FITCTR":
                        putOptionsCommonFeatures.Add("FITCNT");
                        break;
                    case "GAMERM":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "GOLFPRIV":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "GOLFPUB":
                        putOptionsCommonFeatures.Add("GOLFC");
                        break;
                    case "GRILL":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "HTTUBCOM":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "JOGBIKE":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "KITCHFAC":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "LAKEPRIV":
                        putOptionsCommonFeatures.Add("LAKER");
                        break;
                    case "PARK":
                        putOptionsCommonFeatures.Add("PARKA");
                        break;
                    case "PETAM":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "PLAYGRND":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "POOLCOMM":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "SAUNA":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "SMAIRPRT":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "SPRTCTS":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "SPRTFAC":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "STORAGE":
                        // putOptionsCommonFeatures.Add("");
                        break;
                    case "TENNISCT":
                        putOptionsCommonFeatures.Add("TENNI");
                        break;
                    case "UNGRUTIL":
                        // putOptionsCommonFeatures.Add("");
                        break;
                }
            }

            //// Market options that doesn't match
            //// 'BASKE','Basketball Court',
            //// 'BBQGR','BBQ\/Grill',
            //// 'BIKET','Bike Trails',
            //// 'BOATD','Boat Dock',
            //// 'BOATR','Boat Ramp',
            //// 'BRIDL','Bridle Path',
            //// 'CONTR','Controlled Access',
            //// 'FISHI','Fishing Pier',
            //// 'NONE','None',
            //// 'OTHSE','Other-See Remarks',
            //// 'SWIMM','Swimming Pool',
            //// 'VOLLE','Volleyball Court',
            //// 'WALKI','Walking\/Jogging Trail'

            if (putOptionsCommonFeatures.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_599", string.Join(",", putOptionsCommonFeatures.ToArray()), "Neighborhood Amenities", tabName);  // Neighborhood Amenities
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_177", listing.LotDesc); // Access/Road Surface
            var putOptionsLotDescForAccessRoadSurface = new List<string>();
            foreach (string option in optionsLotDesc)
            {
                switch (option)
                {
                    case "ALLEYACC":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "BACKGOLF":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CANAL":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CULTVTD":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CURBS":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "FLAGLOT":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "INTERIOR":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "IRREG":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "LAKE":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "LAKEFRT":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "NOBKYDGR":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "POND":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "PRIVROAD":
                        putOptionsLotDescForAccessRoadSurface.Add("PRIVA");
                        break;
                    case "PUBLROAD":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "ROLLING":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "STREAM":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "XERISCAP":
                        //// putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // 'ASPHA','Asphalt',
            // 'CALIC','Caliche',
            // 'CITYS','City Street',
            // 'CONCR','Concrete',
            // 'COUNT','County Road',
            // 'CURBS','Curbs',
            // 'DIRT','Dirt',
            // 'EASEM','Easement Road',
            // 'GRAVE','Gravel',
            // 'INTER','Interstate Hwy-1 mi or Less',
            // 'NONE','None',
            // 'OTHSE','Other-See Remarks',
            // 'PAVED','Paved',
            // 'SEALE','Sealed & Chipped',
            // 'SIDEW','Sidewalks',
            // 'STATE','State Highway',
            // 'USHIG','US Highway'

            //// if (putOptionsLotDescForAccessRoadSurface.Count > 0)
            //// {
            ////    this.uploaderClient.SetMultipleCheckboxById("Input_177", string.Join(",", putOptionsLotDescForAccessRoadSurface.ToArray())); // Access/Road Surface
            //// }
            this.uploaderClient.SetMultipleCheckboxById("Input_600", "CITYS", "Access/Road Surface", tabName); // Access/Road Surface

            // this.uploaderClient.SetMultipleCheckboxById("Input_178", listing.HeatSystemDesc); // Heat
            var putOptionsHeat = new List<string>();
            var optionsLotHeat = string.IsNullOrWhiteSpace(listing.HeatSystemDesc) ? new List<string>() : listing.HeatSystemDesc.Split(',').ToList();
            foreach (string option in optionsLotHeat)
            {
                switch (option)
                {
                    case "1UNIT":
                        putOptionsHeat.Add("1UNIT");
                        break;
                    case "2UNIT":
                        putOptionsHeat.Add("2UNIT");
                        break;
                    case "3+UNIT":
                        putOptionsHeat.Add("3PLUN");
                        break;
                    case "CNTRL":
                        putOptionsHeat.Add("CENTR");
                        break;
                    case "FLRFR":
                        putOptionsHeat.Add("FLOOR");
                        break;
                    case "HTPMP":
                        //// putOptionsHeat.Add("");
                        break;
                    case "JET":
                        //// putOptionsHeat.Add("");
                        break;
                    case "NONE":
                        putOptionsHeat.Add("NONE");
                        break;
                    case "OTHER":
                        putOptionsHeat.Add("OTHSE");
                        break;
                    case "PANEL":
                        //// putOptionsHeat.Add("");
                        break;
                    case "WNDUN":
                        putOptionsHeat.Add("WINDO");
                        break;
                    case "WOODSTOVE":
                        putOptionsHeat.Add("WOODS");
                        break;
                    case "ZONED":
                        putOptionsHeat.Add("ZONED");
                        break;
                }
            }

            //// Market options that doesn't match
            //// '1UNIT','1 Unit',
            //// '2UNIT','2 Units',
            //// '3PLUN','3+ Units',
            //// 'Fireplace','Fireplace',
            //// 'GASJE','Gas Jets',
            //// 'OTHSE','Other-See Remarks',
            //// 'PROPA','Propane\/Butane',
            //// 'WOODS','Wood Stove',
            //// 'ZONED','Zoned'

            if (putOptionsHeat.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_610", string.Join(",", putOptionsHeat.ToArray()), "Heat", tabName); // Heat
            }

            //// this.uploaderClient.SetMultipleCheckboxById("Input_179", listing.CoolSystemDesc); // A/C
            var putOptionsCoolSystemDesc = new List<string>();
            var optionsLotCoolSystemDesc = string.IsNullOrWhiteSpace(listing.CoolSystemDesc) ? new List<string>() : listing.CoolSystemDesc.Split(',').ToList();
            foreach (string option in optionsLotCoolSystemDesc)
            {
                switch (option)
                {
                    case "":
                        //// putOptionsCoolSystemDesc.Add("");
                        break;
                        /*case "":
                            putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            //putOptionsCoolSystemDesc.Add("");
                            break;
                        case "":
                            putOptionsCoolSystemDesc.Add("");
                            break;*/
                }
            }

            //// Market options that doesn't match
            //// '1UNIT','1 Unit',
            //// '2UNIT','2 Units',
            //// '3PLUN','3+ Units',
            //// 'ATTIC','Attic Fan',
            //// 'ELECT','Electric',
            //// 'HEATP','Heat Pump',
            //// 'OTHSE','Other-See Remarks',
            //// 'ZONED','Zoned'

            if (putOptionsCoolSystemDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_611", string.Join(",", putOptionsCoolSystemDesc.ToArray()), "A/C ", tabName); // A/C
            }

            // this.uploaderClient.SetMultipleCheckboxById("Input_180", listing.SewerDesc); // Water/Sewer
            var putOptionsSewerDesc = new List<string>();
            var optionsSewerDesc = string.IsNullOrWhiteSpace(listing.SewerDesc) ? new List<string>() : listing.SewerDesc.Split(',').ToList();
            foreach (string option in optionsSewerDesc)
            {
                switch (option)
                {
                    case "CITY":
                        putOptionsSewerDesc.Add("CITYW");
                        break;
                    case "CITYPRO":
                        //// putOptionsSewerDesc.Add("");
                        break;
                    case "MUD":
                        putOptionsSewerDesc.Add("MUD");
                        break;
                    case "NONE":
                        putOptionsSewerDesc.Add("NONES");
                        putOptionsSewerDesc.Add("NONEW");
                        break;
                    case "PRIVATE":
                        putOptionsSewerDesc.Add("PRVTW");
                        break;
                    case "SEPTICO":
                        putOptionsSewerDesc.Add("SPTC");
                        break;
                    case "SPTND":
                        putOptionsSewerDesc.Add("SPTCR");
                        break;
                    case "SPTSHRD":
                        //// putOptionsSewerDesc.Add("");
                        break;
                    case "WTRDIST":
                        //// putOptionsSewerDesc.Add("");
                        break;
                }
            }

            //// Market options that doesn't match
            //// 'AEROB','Aerobic Septic',
            //// 'COOPW','Co-Op Water',
            //// 'ONSSE','On-Site Sewer',
            //// 'ONSWT','On-Site Water',
            //// 'OTHSE','Other Sewer-See Remarks',
            //// 'OTHWT','Other Water-See Remarks',
            //// 'PUBLI','Public Sewer',
            //// 'SEPRS','Separate Sewer Meters',
            //// 'SEPRW','Separate Water Meters',
            //// 'WELL','Well',
            //// 'WELLR','Well Required'

            if (putOptionsSewerDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_612", string.Join(",", putOptionsSewerDesc.ToArray()), "Water/Sewer", tabName); // Water/Sewer
            }

            // if (!String.IsNullOrWhiteSpace(listing.CTXRWaterHeater))
            //    this.uploaderClient.SetMultipleCheckboxById("Input_181", listing.CTXRWaterHeater.Trim(), "Water/Heater", tabName); // Water/Heater
            // this.uploaderClient.SetMultipleCheckboxById("Input_182", listing.UtilitiesDesc); // Other Utilities
            var putOptionsUtilitiesDesc = new List<string>();
            var optionsUtilitiesDesc = string.IsNullOrWhiteSpace(listing.UtilitiesDesc) ? new List<string>() : listing.UtilitiesDesc.Split(',').ToList();
            foreach (string option in optionsUtilitiesDesc)
            {
                switch (option)
                {
                    case "ABOVGRND":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELEAVAIL":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELECTRIC":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELENOAVL":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "FUELTANK":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "GAS":
                        putOptionsUtilitiesDesc.Add("GASIN");
                        putOptionsUtilitiesDesc.Add("NATUR");
                        break;
                    case "GASAVAIL":
                        putOptionsUtilitiesDesc.Add("GASIN");
                        putOptionsUtilitiesDesc.Add("GASAV");
                        break;
                    case "GASNOAVL":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsUtilitiesDesc.Add("NONE");
                        break;
                    case "PHONAVAL":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PHONNOT":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PHONPROP":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PROPANE":
                        putOptionsUtilitiesDesc.Add("PBTOW");
                        break;
                    case "PROPAVAL":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PROPNEED":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "SOLAR":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "UNDRGRND":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                    case "WIND":
                        //// putOptionsUtilitiesDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            // 'AVAIL','Available',
            // 'CABLE','Cable TV',
            // 'CITYE','City Electric',
            // 'CITYG','City Garbage',
            // 'COOPE','Co-Op Electric',
            // 'HIGHS','High Speed Internet ',
            // 'ONSEL','On-Site Electric',
            // 'OTHSE','Other-See Remarks',
            // 'PRIVA','Private Garbage Service',
            // 'PBTLE','Propane\/Butane Tank-Leased',
            // 'SATEL','Satellite Dish',
            // 'SEPER','Separate Meters',
            // 'TELEP','Telephone'
            if (putOptionsUtilitiesDesc.Count > 0)
            {
                this.uploaderClient.SetMultipleCheckboxById("Input_613", string.Join(",", putOptionsUtilitiesDesc.ToArray()), " Other Utilities", tabName); // Other Utilities
            }
        }

        private void FillFinancialInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Financial Information";
            this.uploaderClient.ExecuteScript(script: " jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Financial")); // Financial

            //// this.uploaderClient.SetMultipleCheckboxById("Input_149", ??? ); // Special
            //// this.uploaderClient.WriteTextbox(By.Name("PROPSDTRMS"), listing.PROPSDTRMS);
            //// this.uploaderClient.SetMultipleCheckboxById("Input_150", "NEGOT,CASH,CONVE,FHA,TEXAS,VA"); // Proposed Terms
            //// this.uploaderClient.SetMultipleCheckboxById("Input_150", listing.PROPSDTRMS, "Proposed Terms", tabName); // Proposed Terms

            // this.uploaderClient.SetSelect(By.Id("Input_624"), listing.HasHOA, "HOA", tabName); // HOA
            if (!string.IsNullOrEmpty(listing.HOA) && (listing.HOA == "MAND"))
            {
                // this.uploaderClient.SetSelect(By.Id("Input_151"), "1", "HOA", tabName); // HOA
                this.uploaderClient.SetSelect(By.Id("Input_622"), value: "MAN", fieldLabel: "HOA Mandatory", tabName); // HOA Mandatory
            }
            else
            {
                this.uploaderClient.SetSelect(By.Id("Input_622"), value: "NONE", fieldLabel: "HOA Mandatory", tabName); // HOA Mandatory
            }

            this.uploaderClient.WriteTextbox(By.Id("Input_623"), listing.AssocName); // HOA Name
            this.uploaderClient.WriteTextbox(By.Id("Input_624"), listing.AssocFee); // HOA Amount
            this.uploaderClient.SetSelect(By.Id("Input_625"), listing.AssocFeePaid, fieldLabel: "HOA Term", tabName); // HOA Term
        }

        private void FillShowingInformation(ResidentialListingRequest listing)
        {
            const string tabName = "Showing Information";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");

            this.uploaderClient.ClickOnElement(By.LinkText("Brokerage/Showing Information")); // Brokerage/Showing Information

            // this.uploaderClient.WriteTextbox(By.Id("Input_209"), listing.AgentList); // List Agent MLS ID TODO: Verify the correct property
            // this.uploaderClient.ExecuteScript("javascript:document.getElementById('Input_209_Refresh').value='1';RefreshToSamePage();"); // Refresh TODO: Veriry if necessary click in this button

            // this.uploaderClient.WriteTextbox(By.Id("Input_211"), listing.AgentList); // List Agent MLS ID TODO: Verify the correct property
            // this.uploaderClient.ExecuteScript("javascript:document.getElementById('Input_211_Refresh').value='1';RefreshToSamePage();"); // Refresh TODO: Veriry if necessary click in this button
            this.uploaderClient.SetSelect(By.Id("Input_632"), value: "Pct", fieldLabel: "Buyer Agency $ or %", tabName); // Buyer Agency $ or %  (default hardcode "%")
            this.uploaderClient.WriteTextbox(By.Id("Input_633"), value: "3"); // Buyer Agency Compensation (default hardcode "3")
            // this.uploaderClient.SetSelect(By.Id("Input_213"), ??? ); // Variable Compensation
            this.uploaderClient.SetMultipleCheckboxById("Input_645", csvValues: "APPOI", fieldLabel: "How to Show/Occupancy", tabName); // How to Show/Occupancy (default hardcode "Appointment Only")
            this.uploaderClient.WriteTextbox(By.Id("Input_635"), value: "0"); // Buyer Agency Compensation (default hardcode "0")
            this.uploaderClient.SetSelect(By.Id("Input_636"), value: "Pct", fieldLabel: "Sub Agency $ or % ", tabName); // Sub Agency $ or % (default hardcode "%")
            this.uploaderClient.SetSelect(By.Id("Input_637"), listing.ProspectsExempt, "Prospects Exempt", tabName); // Prospects Exempt (default hardcode "No")
            this.uploaderClient.WriteTextbox(By.Id("Input_638"), listing.TitleCo); // Pref Title Company
            this.uploaderClient.WriteTextbox(By.Id("Input_639"), listing.EarnestMoney); // Earnest Money
        }

        private void FillRemarks(ResidentialListingRequest listing)
        {
            const string tabName = "Remarks";
            this.uploaderClient.ExecuteScript(" jQuery(document).scrollTop(0);");
            this.uploaderClient.ClickOnElement(By.LinkText("Remarks")); // Financial Information

            this.uploaderClient.SetSelect(By.Id("Input_143"), value: "1", fieldLabel: "IDX Opt In", tabName); // IDX Opt In (default hardcode "Yes")
            this.uploaderClient.SetSelect(By.Id("Input_144"), value: "1", fieldLabel: "Display on Internet", tabName); // Display on Internet (default hardcode "Yes")
            this.uploaderClient.SetSelect(By.Id("Input_145"), value: "1", fieldLabel: "Display Address", tabName); // Display Address (default hardcode "Yes")
            this.uploaderClient.SetSelect(By.Id("Input_146"), value: "0", fieldLabel: "Allow AVM", tabName); // Allow AVM (default hardcode "Yes")
            this.uploaderClient.SetSelect(By.Id("Input_147"), value: "1", fieldLabel: "Allow Comment", tabName); // Allow Comment (default hardcode "Yes")

            this.UpdatePublicRemarksInRemarksTab(listing); // Public Remarks
            this.UpdatePrivateRemarksInRemarksTab(listing); // Agent Remarks
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
            // driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("Input_917")));
            Thread.Sleep(400);
            this.uploaderClient.WriteTextbox(By.Id("Input_140"), listing.GetPublicRemarks()); // Internet / Remarks / Desc. of Property
            this.uploaderClient.WriteTextbox(By.Id("Input_142"), listing.Directions); // Syndication Remarks
        }

        private void UpdatePrivateRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            var bonusMessage = string.Empty;
            if (listing.BonusCheckBox.HasValue && listing.BonusCheckBox.Value &&
                listing.BuyerCheckBox.HasValue && listing.BuyerCheckBox.Value)
            {
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details. ";
            }
            else if (listing.BonusCheckBox.HasValue && listing.BonusCheckBox.Value)
            {
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            }
            else if (listing.BuyerCheckBox.HasValue && listing.BuyerCheckBox.Value)
            {
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
            }

            var realtorContactEmail = string.Empty;
            if (!string.IsNullOrEmpty(listing.ContactEmailFromCompany))
            {
                realtorContactEmail = listing.ContactEmailFromCompany;
            }
            else if (!string.IsNullOrEmpty(listing.RealtorContactEmail))
            {
                realtorContactEmail = listing.RealtorContactEmail;
            }
            else if (!string.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
            {
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;
            }

            realtorContactEmail =
                (!string.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : string.Empty;

            this.uploaderClient.WriteTextbox(By.Id("Input_141"), bonusMessage + listing.GetPrivateRemarks() + realtorContactEmail); // Agent Remarks
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if (!listing.IsNewListing)
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            this.uploaderClient.WriteTextbox(By.Id("INPUT__93"), value: listing.Latitude); // Latitude
            this.uploaderClient.WriteTextbox(By.Id("INPUT__94"), value: listing.Longitude); // Longitude
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
            this.uploaderClient.ExecuteScript(script: "javascript:var btn = jQuery('#m_lbSave')[0]; btn.click();");
            this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
            this.NavigateToQuickEdit(listing.MLSNum);

            Thread.Sleep(400);

            // Enter Manage Photos
            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Manage Photos"), cancellationToken);
            this.uploaderClient.ClickOnElement(By.LinkText("Manage Photos"));

            var media = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: MarketCode.CTX, cancellationToken);
            var i = 0;
            // Upload Images
            // foreach (var image in media.OrderBy(x => x.Order))
            foreach (var image in media)
            {
                // MQ-311 - Uploader - Convert PNG for NTREIS and ACTRIS
                if (!string.IsNullOrWhiteSpace(image.Extension) && (image.Extension.ToLower().Equals(".gif") || image.Extension.ToLower().Equals(".png")))
                {
                    image.Extension = ".jpg";
                    string[] elements = Regex.Split(image.PathOnDisk, ".");
                    if (elements.Length == 2)
                    {
                        var fileName = elements[0].ToString();
                        image.PathOnDisk = fileName + ".jpg";
                    }
                }

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("m_ucImageLoader_m_tblImageLoader"), cancellationToken);

                this.uploaderClient.FindElement(By.Id("m_ucImageLoader_m_tblImageLoader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id($"photoCell_{i}"), cancellationToken);

                this.uploaderClient.ExecuteScript($"jQuery('#photoCell_{i} a')[0].click();");
                Thread.Sleep(500);

                this.uploaderClient.ExecuteScript($"jQuery('#m_tbxDescription').val('{image.Caption}');");

                Thread.Sleep(500);

                this.uploaderClient.ExecuteScript("jQuery('#m_ucDetailsView_m_btnSave').parent().removeClass('disabled');");

                this.uploaderClient.ClickOnElementById("m_ucDetailsView_m_btnSave");

                i++;
            }
        }
    }
}
