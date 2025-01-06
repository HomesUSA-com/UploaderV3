namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
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

    public class AmarilloUploadService : IAmarilloUploadService
    {
        private const string ButtonNextId = "nextbtn";
        private const string LenderOwnerFieldLabel = "Lender Owned";
        private const string PotentialShortSaleFieldLabel = "Potential Short Sale";
        private const string ForeclosedFieldLabel = "Foreclosed";
        private const string TenantFieldLabel = "Tenant";
        private const string SurveillanceEquipmentOnSiteFieldLabel = "Surveillance equipment on-site";
        private const string HOARequiremientFieldLabel = "HOA Requiremient";
        private const string FireplacesNumberFieldLabel = "Fireplaces number";
        private const string FailureMessage = "Failure uploading the lising {requestId}";
        private const string OneQuaterBathFieldLabel = "1/4 Baths";
        private const string HalfBathFieldLabel = "1/2 Baths";
        private const string ThreeQuaterBathFieldLabel = "3/4 Baths";
        private const string FullBathFieldLabel = "Full Baths";
        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<AmarilloUploadService> logger;

        public AmarilloUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<AmarilloUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.Amarillo;

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public async Task<LoginResult> Login(Guid companyId)
        {
            var marketInfo = this.options.MarketInfo.Amarillo;
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            this.uploaderClient.DeleteAllCookies();

            var credentials = await LoginCommon.GetMarketCredentials(company, credentialsTask);

            // Connect to the login page
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("login-button"));

            this.uploaderClient.WriteTextbox(By.Id("user"), credentials[LoginCredentials.Username]);
            this.uploaderClient.ClickOnElementById("login-button");
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Id("password"), credentials[LoginCredentials.Password]);
            this.uploaderClient.ClickOnElementById("login-button");

            Thread.Sleep(2000);

            return LoginResult.Logged;
        }

        public UploadResult Logout()
        {
            this.uploaderClient.ClickOnElementById("user-menu-button");
            Thread.Sleep(500);
            this.uploaderClient.ClickOnElementById("user-menu-button");
            return UploadResult.Success;
        }

        public Task<UploadResult> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(listing));

            return EditListing();

            async Task<UploadResult> EditListing()
            {
                this.logger.LogInformation("Editing the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);

                if (listing.IsNewListing)
                {
                    this.NavigateToNewPropertyInput();
                }
                else
                {
                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            ArgumentNullException.ThrowIfNull(listing);

            return UploadListing(logIn);

            async Task<UploadResult> UploadListing(bool logIn)
            {
                try
                {
                    this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                    }

                    Thread.Sleep(2000);

                    if (listing.IsNewListing)
                    {
                        this.NavigateToNewPropertyInput();
                        this.FillGeneralInformationNewListing(listing as AmarilloListingRequest);
                        this.FillAddressFields(listing as AmarilloListingRequest);
                        this.FillMainFields(listing as AmarilloListingRequest);
                        this.FillDetailsForNewListings(listing as AmarilloListingRequest);
                    }
                    else
                    {
                        this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                        this.GoToTabById("listing-information");
                        this.FillMainFields(listing as AmarilloListingRequest);
                        this.FillDetailsForExistingListings(listing as AmarilloListingRequest);
                    }

                    this.FillRoomInformation(listing as AmarilloListingRequest);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, FailureMessage, listing?.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            ArgumentNullException.ThrowIfNull(nameof(listing));

            return UploadListing(logIn);

            async Task<UploadResult> UploadListing(bool logIn)
            {
                try
                {
                    this.logger.LogInformation("Partial Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                    }

                    Thread.Sleep(2000);

                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);
                    this.GoToTabById("listing-information");
                    this.FillMainFields(listing as AmarilloListingRequest);
                    this.FillDetailsForExistingListings(listing as AmarilloListingRequest);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, FailureMessage, listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            ArgumentNullException.ThrowIfNull(listing);

            return UpdateListingCompletionDate(logIn);

            async Task<UploadResult> UpdateListingCompletionDate(bool logIn)
            {
                try
                {
                    this.logger.LogInformation("Updating CompletionDate for the listing {requestId}", listing.ResidentialListingRequestID);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                    }

                    this.NavigateToEditResidentialForm(listing.MLSNum, cancellationToken);

                    this.GoToTabById("listing-information");

                    this.GoToPropertyInformationTab();
                    this.UpdateYearBuiltDescription(listing);
                    this.FillCompletionDate(listing);

                    this.GoToRemarksTab();
                    this.UpdatePublicRemarksInRemarksTab(listing);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, FailureMessage, listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateImages(ResidentialListingRequest listing, bool logIn = true, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(listing);

            return UpdateListingImages(logIn);
            async Task<UploadResult> UpdateListingImages(bool logIn)
            {
                try
                {
                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.logger.LogInformation("Updating media for the listing {requestId}", listing.ResidentialListingRequestID);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                    this.NavigateToQuickEdit(listing.MLSNum);
                    Thread.Sleep(1000);

                    // Enter Manage Photos
                    this.uploaderClient.ExecuteScript("jQuery('a[title=\"Attach photos to this listing\"')[0].click()");

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("files"), cancellationToken);
                    this.DeleteAllImages();
                    await this.ProcessImages(listing, cancellationToken);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, FailureMessage, listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            ArgumentNullException.ThrowIfNull(nameof(listing));

            return UpdateListingPrice(logIn);

            async Task<UploadResult> UpdateListingPrice(bool logIn)
            {
                try
                {
                    this.logger.LogInformation("Updating the price of the listing {requestId} to {listPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);

                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);
                    Thread.Sleep(1000);
                    var mlsPreffix = listing.MLSNum.Split('-')[0];
                    var listNumber = listing.MLSNum.Split('-')[1];
                    var url = "https://amt.flexmls.com/cgi-bin/mainmenu.cgi?cmd=url+brokerload/changeprice/step2.html&list_nbr=" + listNumber + "&prefix=" + mlsPreffix;
                    this.uploaderClient.NavigateToUrl(url);

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("list_price"), cancellationToken);
                    this.uploaderClient.WriteTextbox(By.Id("list_price"), listing.ListPrice); // List Price
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, FailureMessage, listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            ArgumentNullException.ThrowIfNull(nameof(listing));

            return UpdateListingStatus(logIn);

            async Task<UploadResult> UpdateListingStatus(bool logIn)
            {
                try
                {
                    this.logger.LogInformation("Editing the status information for the listing {requestId}", listing.ResidentialListingRequestID);
                    this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                    if (logIn)
                    {
                        await this.Login(listing.CompanyId);
                        Thread.Sleep(1000);
                    }

                    this.NavigateToQuickEdit(listing.MLSNum);
                    Thread.Sleep(1000);

                    switch (listing.ListStatus)
                    {
                        case "Cancelled":
                            HandleCancelledStatus(cancellationToken);
                            break;
                        case "Closed":
                            HandleClosedStatus(listing);
                            break;
                        case "Pending":
                        case "Under Contract W/Contingency":
                            HandlePendingStatus(listing);
                            break;
                        case "Withdrawn":
                            HandleWithdrawnStatus(listing);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid Status '{listing.ListStatus}' for Amarillo Listing with Id '{listing.ResidentialListingID}'");
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the listing {requestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
                }

                return UploadResult.Success;
            }

            void HandleClosedStatus(ResidentialListingRequest listing)
            {
                this.uploaderClient.ExecuteScript("javascript:changestatus('C');");
                Thread.Sleep(1000);

                this.uploaderClient.WriteTextbox(By.Id("cancel_date"), listing.ClosedDate?.ToShortDateString()); // Cancel Date
            }

            void HandlePendingStatus(ResidentialListingRequest listing)
            {
                this.uploaderClient.ExecuteScript("javascript:changestatus('P');");
                Thread.Sleep(1000);

                this.uploaderClient.WriteTextbox(By.Id("un_contr_date"), listing.ContractDate?.ToShortDateString()); // Under Contract Date

                var marketInfo = this.options.MarketInfo.Amarillo;
                var listingMember = marketInfo.AgentId;
                this.FillDropDownWithAutocomplete(listingMember, "me_tech_id");  // Listing Member
                this.FillDropDownWithAutocomplete(listingMember, "sell_agent_id");  // Selling Member
                this.FillDropDownWithAutocomplete(listingMember, "co_sell_agent_id");  // Co-selling Member
            }

            void HandleCancelledStatus(CancellationToken cancellationToken)
            {
                this.uploaderClient.ExecuteScript("javascript:changestatus('L');");
                Thread.Sleep(1000);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("cancel_date"), cancellationToken);
                this.uploaderClient.WriteTextbox(By.Id("cancel_date"), listing.ClosedDate?.ToShortDateString()); // Cancel Date
            }

            void HandleWithdrawnStatus(ResidentialListingRequest listing)
            {
                this.uploaderClient.ExecuteScript("javascript:changestatus('W');");
                Thread.Sleep(1000);

                this.uploaderClient.WriteTextbox(By.Id("withdraw_date"), listing.WithdrawnDate?.ToShortDateString()); // Withdrawal Date

                var marketInfo = this.options.MarketInfo.Amarillo;
                var listingMember = marketInfo.AgentId;
                this.FillDropDownWithAutocomplete(listingMember, "me_tech_id");  // Listing Member

                this.uploaderClient.WriteTextbox(By.Id("status_change_date"), DateTime.Now.ToShortDateString()); // Withdrawn Date
            }
        }

        public Task<UploadResult> UploadVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(listing);

            return UploadListingVirtualTour();

            async Task<UploadResult> UploadListingVirtualTour()
            {
                this.logger.LogInformation("Updating VirtualTour for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);
                await this.Login(listing.CompanyId);
                this.NavigateToQuickEdit(listing.MLSNum);
                Thread.Sleep(1000);

                return UploadResult.Success;
            }
        }

        public Task<UploadResult> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(listing));

            return UploadOpenHouse();

            async Task<UploadResult> UploadOpenHouse()
            {
                this.logger.LogInformation("Editing the information of Open House for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, listing.IsNewListing);
                await this.Login(listing.CompanyId);
                this.NavigateToQuickEdit(listing.MLSNum);

                Thread.Sleep(1000);
                //// Enter Open House
                this.uploaderClient.ExecuteScript("jQuery('a[title=\"Maintain open houses for this listing\"')[0].click()");

                this.CleanOpenHouse();

                if (listing.EnableOpenHouse)
                {
                    this.AddOpenHouses(listing);
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

        public Task<UploadResult> UpdateLotImages(LotListingRequest listing, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UploadResult> UpdateLotPrice(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            throw new NotImplementedException();
        }

        public Task<UploadResult> EditLot(LotListingRequest listing, CancellationToken cancellationToken, bool logIn)
        {
            throw new NotImplementedException();
        }

        private void NavigateToNewPropertyInput()
        {
            this.uploaderClient.SwitchTo().Frame("top_frame");
            this.uploaderClient.ExecuteScript("jQuery('#bookmarkMenuItems > li:eq(5) > a')[0].click();");
            Thread.Sleep(1000);
            this.uploaderClient.SwitchTo().Frame("view_frame");
        }

        private void NavigateToQuickEdit(string mlsNumber)
        {
            this.uploaderClient.NavigateToUrl("https://amt.flexmls.com/cgi-bin/mainmenu.cgi?cmd=url+brokerload/changelisting/step1.html");
            Thread.Sleep(1000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("changeListingNextButton"));
            var mlsNumberFormatted = mlsNumber.Split("-");
            this.uploaderClient.WriteTextbox(By.Name("prefix"), value: mlsNumberFormatted[0]);
            this.uploaderClient.WriteTextbox(By.Name("list_nbr"), value: mlsNumberFormatted[1]);
            Thread.Sleep(500);
            this.uploaderClient.ClickOnElement(By.Id("changeListingNextButton"));
            Thread.Sleep(1000);
        }

        private void NavigateToEditResidentialForm(string mlsNumber, CancellationToken cancellationToken = default)
        {
            this.NavigateToQuickEdit(mlsNumber);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("change-listing-001"), cancellationToken);
            Thread.Sleep(1000);
        }

        private void FillGeneralInformationNewListing(ResidentialListingRequest listing)
        {
            var marketInfo = this.options.MarketInfo.Amarillo;

            this.uploaderClient.ClickOnElementById("tab0_0");

            this.uploaderClient.SetSelect(By.Id("card_fmt"), "A"); // Property Type

            var listingMember = marketInfo.AgentId;
            this.FillDropDownWithAutocomplete(listingMember, "me_tech_id");  // Listing Member
            this.FillDropDownWithAutocomplete(listingMember, "co_list_agent");  // Co-listing Agent

            this.uploaderClient.SetSelect(By.Id("amt_lookup_county"), listing.County); // County

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void FillAddressFields(ResidentialListingRequest listing)
        {
            this.uploaderClient.WriteTextbox(By.Id("house_nbr"), listing.StreetNum); // House #
            this.uploaderClient.SetSelect(By.Id("streetdirprefix"), listing.StreetDir); // Street Direction Pfx
            this.uploaderClient.WriteTextbox(By.Id("streetname"), listing.StreetName); // Street Name
            this.uploaderClient.SetSelect(By.Id("streetsuffix"), listing.StreetType); // Street Sfx
            this.uploaderClient.WriteTextbox(By.Id("zip"), listing.Zip); // Postal Code
            this.uploaderClient.SetSelect(By.Id("county"), listing.County); // County

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void FillMainFields(ResidentialListingRequest listing)
        {
            this.uploaderClient.SetSelect(By.Id("userdefined17"), "N"); // To Be Auctioned
            this.uploaderClient.SetSelect(By.Id("userdefined14"), listing.IsNewConstruction.BoolToYesNoBool().ToTitleCase()); // New Construction
            this.uploaderClient.WaitUntilElementExists(By.Id("userdefined34"));
            this.uploaderClient.WriteTextbox(By.Id("userdefined34"), listing.Excludes); // Exclusions
            this.uploaderClient.SetSelect(By.Id("userdefined51"), "N"); // Fixture Lease(s)?
            this.uploaderClient.SetSelect(By.Id("style"), listing.HousingStyleDesc); // Style

            this.uploaderClient.SetSelect(By.Id("userdefined10"), (listing as AmarilloListingRequest).Zone); // Zone
            if (!string.IsNullOrEmpty((listing as AmarilloListingRequest).MLSArea))
            {
                this.uploaderClient.SetSelect(By.Id("area"), (listing as AmarilloListingRequest).MLSArea.Split('-')[0].Trim()); // Area
            }

            this.uploaderClient.WriteTextbox(By.Id("total_br"), listing.Beds); // Total Bedroom
            this.uploaderClient.WriteTextbox(By.Id("total_bath"), listing.BathsTotal); // Total Bathrooms
            this.uploaderClient.WriteTextbox(By.Id("lock_box_nbr"), listing.KeyboxNumber); // Lock Box Number

            this.uploaderClient.SetSelect(By.Id("type"), (listing as AmarilloListingRequest).RealtorType); // Realtor.COM Type
            this.uploaderClient.SetSelect(By.Id("userdefined9"), listing.SqFtSource); // Square Foot Source
            this.uploaderClient.WriteTextbox(By.Id("userdefined28"), listing.GarageCapacity); // Garage Stall
            this.uploaderClient.SetSelect(By.Id("userdefined46"), (listing as AmarilloListingRequest).HasAttachedGarage.BoolToYesNoBool()[0]); // Attached Garage
            this.uploaderClient.SetSelect(By.Id("userdefined47"), (listing as AmarilloListingRequest).HasDetachedGarage.BoolToYesNoBool()[0]); // Attached Garage
            this.uploaderClient.SetSelect(By.Id("userdefined48"), (listing as AmarilloListingRequest).HasCarpot.BoolToYesNoBool()[0]); // Carport
            this.uploaderClient.SetSelect(By.Id("userdefined49"), (listing as AmarilloListingRequest).HasRVParking.BoolToYesNoBool()[0]); // RV Parking

            this.uploaderClient.WriteTextbox(By.Id("garage_rem"), listing.GarageDesc); // Garage Remarks
            this.uploaderClient.WriteTextbox(By.Id("lot_size"), listing.LotSize); // Lot Size Dimensions
            this.uploaderClient.WriteTextbox(By.Id("userdefined24"), listing.LotSizeAcres); // Approximate Acres
            this.uploaderClient.WriteTextbox(By.Id("yr_built"), listing.YearBuilt); // Year Built
            this.uploaderClient.WriteTextbox(By.Id("userdefined26"), listing.LotDim); // Lot Size Square Feet
            this.uploaderClient.SetSelect(By.Id("userdefined3"), listing.FacesDesc); // Dwelling Faces

            var remarks = listing.GetPublicRemarks();
            this.uploaderClient.WriteTextbox(By.Id("remark1"), remarks.Length < 800 ? remarks : remarks.Substring(0, 799), true); // Public Remarks

            var agentRemarks = listing.GetAgentRemarksMessage();
            this.uploaderClient.WriteTextbox(By.Id("remark2"), agentRemarks.Length < 400 ? agentRemarks : agentRemarks.Substring(0, 399), true); // Private Remarks

            var showingInstructions = listing.ShowingInstructions;
            this.uploaderClient.WriteTextbox(By.Id("userdefined38"), agentRemarks.Length < 500 ? showingInstructions : showingInstructions.Substring(0, 499), true); // Showing Instructions

            this.uploaderClient.ScrollDownPosition(4000);

            this.uploaderClient.WriteTextbox(By.Id("taxes"), listing.TaxRate); // Taxes
            this.uploaderClient.WriteTextbox(By.Id("userdefined25"), listing.ExemptionsDesc); // Taxes w/o Exemptions
            this.uploaderClient.WriteTextbox(By.Id("parcel_nbr"), (listing as AmarilloListingRequest).PropertyId); // Property ID
            this.uploaderClient.WriteTextbox(By.Id("legal"), listing.Legal); // Legal
            this.uploaderClient.WriteTextbox(By.Id("userdefined4"), (listing as AmarilloListingRequest).CommonName); // Common Name

            this.uploaderClient.ScrollDownPosition(5000);

            this.uploaderClient.SetSelect(By.Id("owner"), (listing as AmarilloListingRequest).IsPropertyInPID.BoolToYesNoBool().ToTitleCase()); // Is Property in PID?
            this.uploaderClient.SetSelect(By.Id("owner_phone"), (listing as AmarilloListingRequest).IsInsideCityLimits.BoolToYesNoBool().ToTitleCase()); // Inside City Limits
            this.uploaderClient.WriteTextbox(By.Id("userdefined18"), (listing as AmarilloListingRequest).SupervisorName); // L/A Supervisor Name
            this.uploaderClient.WriteTextbox(By.Id("userdefined19"), (listing as AmarilloListingRequest).SupervisorPhone); // Supervisor's Phone
            this.uploaderClient.WriteTextbox(By.Id("userdefined43"), (listing as AmarilloListingRequest).SupervisorLicense); // L/A Supervisor License Number
            this.uploaderClient.WriteTextbox(By.Id("userdefined35"), listing.OwnerName); // Owner
            this.uploaderClient.SetSelect(By.Id("userdefined45"), (listing as AmarilloListingRequest).HasForeignSeller.BoolToYesNoBool()[0]); // Foreign Seller
            this.uploaderClient.WriteTextbox(By.Id("directions"), listing.Directions); // Directions
            this.uploaderClient.SetSelect(By.Id("terms"), "Call L/A"); // Showing Phone Number

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void FillDetailsForExistingListings(ResidentialListingRequest listing)
        {
            this.uploaderClient.SetSelectByText(By.Id("t_679"), "Negotiable", "Hydrostatic", "Hydrostatic Test"); // Hydrostatic Tests

            this.uploaderClient.SetSelectByText(By.Id("t_731"), (listing as AmarilloListingRequest).HasLenderOwned.BoolToYesNoBool().ToTitleCase(), LenderOwnerFieldLabel, LenderOwnerFieldLabel); // Lender Owned

            if ((listing as AmarilloListingRequest).HasDistressedSale)
            {
                this.uploaderClient.ClickOnElementById("i_733"); // Distressed Sale
            }

            if ((listing as AmarilloListingRequest).HasPotentialShortSale)
            {
                this.uploaderClient.ClickOnElementById("i_735");
                this.uploaderClient.SetSelectByText(By.Id("t_735"), (listing as AmarilloListingRequest).HasPotentialShortSale.BoolToYesNoBool().ToTitleCase(), PotentialShortSaleFieldLabel, PotentialShortSaleFieldLabel); // Potential Short Sale
            }

            if ((listing as AmarilloListingRequest).HasForeclosed)
            {
                this.uploaderClient.ClickOnElementById("i_737");
                this.uploaderClient.SetSelectByText(By.Id("t_737"), (listing as AmarilloListingRequest).HasForeclosed.BoolToYesNoBool().ToTitleCase(), ForeclosedFieldLabel, ForeclosedFieldLabel); // Foreclosed
            }

            if ((listing as AmarilloListingRequest).HasTenant)
            {
                this.uploaderClient.ClickOnElementById("i_739");
                this.uploaderClient.SetSelectByText(By.Id("t_739"), (listing as AmarilloListingRequest).HasTenant.BoolToYesNoBool().ToTitleCase(), TenantFieldLabel, TenantFieldLabel); // Tenant
                this.uploaderClient.WriteTextbox(By.Id("t_741"), (listing as AmarilloListingRequest).DateLeaseExpires); // Date Lease Expires
            }

            this.uploaderClient.ClickOnElementById("i_743");
            this.uploaderClient.SetSelectByText(By.Id("t_743"), "No", SurveillanceEquipmentOnSiteFieldLabel, SurveillanceEquipmentOnSiteFieldLabel); // Surveillance equipment on-site

            this.CheckOptions((listing as AmarilloListingRequest).SpecialFeatures); // Spectial Features

            this.uploaderClient.ScrollDownPosition(3000);

            this.uploaderClient.WriteTextbox(By.Id("t_801"), (listing as AmarilloListingRequest).OtherSpecialFeatures); // Other Spec. Feature

            this.CheckOptions((listing as AmarilloListingRequest).ParkingFeatures); // Parking Features

            this.CheckOptions((listing as AmarilloListingRequest).CommunityFeatures); // Communuity Features

            if ((listing as AmarilloListingRequest).HoaRequirement)
            {
                this.uploaderClient.ClickOnElementById("i_837");
                this.uploaderClient.SetSelectByText(By.Id("t_837"), (listing as AmarilloListingRequest).HoaRequirement.BoolToYesNoBool().ToTitleCase(), HOARequiremientFieldLabel, HOARequiremientFieldLabel); // HOA Requiremient
            }

            this.CheckOptions(listing.AppliancesDesc); // Appliances
            this.CheckOptionsByLabel(listing.ExteriorDesc); // Exterior
            this.CheckOptionsByLabel((listing as AmarilloListingRequest).ConstructionType); // Construction type
            this.CheckOptionsByLabel((listing as AmarilloListingRequest).StoriesFeatures); // Stories/Level
            this.CheckOptions(listing.InteriorDesc); // Interior Features

            this.uploaderClient.ClickOnElementById("i_1005");
            this.uploaderClient.SetSelectByText(By.Id("t_1005"), (listing as AmarilloListingRequest).NumQuarterBaths.ToString(), OneQuaterBathFieldLabel, OneQuaterBathFieldLabel); // 1/4 Baths

            this.uploaderClient.ClickOnElementById("i_1007");
            this.uploaderClient.SetSelectByText(By.Id("t_1007"), listing.BathsHalf.ToString(), HalfBathFieldLabel, HalfBathFieldLabel); // 1/2 Baths

            this.uploaderClient.ClickOnElementById("i_1009");
            this.uploaderClient.SetSelectByText(By.Id("t_1009"), (listing as AmarilloListingRequest).NumThreeQuartersBaths.ToString(), ThreeQuaterBathFieldLabel,  ThreeQuaterBathFieldLabel); // 3/4 Baths

            this.uploaderClient.ClickOnElementById("i_1011");
            this.uploaderClient.SetSelectByText(By.Id("t_1011"), listing.BathsFull.ToString(), FullBathFieldLabel, FullBathFieldLabel); // Full Baths

            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=Powder]').prop('checked', " + (listing as AmarilloListingRequest).HasPowder.ToString().ToLower() + ")"); // Powder

            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=\"Hollywood/Jack&Jill\"]').prop('checked', " + (listing as AmarilloListingRequest).HasHollywoodJackAndJill.ToString().ToLower() + ")"); // Hollywood/Jack&Jill

            var thereFireplaces = int.TryParse(listing.NumberFireplaces, out _);
            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=Number]').prop('checked', " + thereFireplaces.ToString().ToLower() + ")"); // Fireplaces number
            if (thereFireplaces)
            {
                this.uploaderClient.SetSelectByText(By.Id("t_1021"), listing.NumberFireplaces ?? listing.NumberFireplaces.ToString(), FireplacesNumberFieldLabel, FireplacesNumberFieldLabel); // Fireplaces number
            }

            this.CheckOptionsByLabel(listing.FireplaceDesc); // Fireplaces description
            this.CheckOptions(listing.LaundryLocDesc); // Laundry Location
            this.CheckOptions((listing as AmarilloListingRequest).LaundryFeatures); // Laundry Features
            this.CheckOptions(listing.FenceDesc); // Fence
            this.CheckOptions(listing.RoofDesc); // Roof
            this.CheckOptions(listing.FoundationDesc); // Foundation
            this.CheckOptions((listing as AmarilloListingRequest).DetachedStructures); // Detached Structures
            this.CheckOptions((listing as AmarilloListingRequest).WaterFeature); // Water
            this.CheckOptions(listing.WaterDesc); // Sewer
            this.CheckOptions(listing.PoolDesc); // Pool
            this.CheckOptions(listing.HeatSystemDesc); // Heat
            this.CheckOptions(listing.CoolSystemDesc); // A/C

            this.uploaderClient.WriteTextbox(By.Id("t_1299"), (listing as AmarilloListingRequest).WaterHeater); // # Heaters

            this.uploaderClient.SetSelect(By.Id("t_1317"), listing.SchoolName1); // Elementary School
            this.uploaderClient.SetSelect(By.Id("t_1319"), listing.SchoolName2); // Intermedaite School
            this.uploaderClient.SetSelect(By.Id("t_1321"), listing.HighSchool); // High School
            this.uploaderClient.SetSelect(By.Id("t_1323"), listing.SchoolDistrict); // Disctrict School

            this.CheckOptions(listing.FinancingProposed); // Possible Financing

            this.uploaderClient.WriteTextbox(By.Id("t_1341"), listing.TitleCo); // Suggested Title Company/Address

            var marketInfo = this.options.MarketInfo.Amarillo;

            this.uploaderClient.WriteTextbox(By.Id("t_1347"), marketInfo.AgentId); // Listing Agent
            this.uploaderClient.WriteTextbox(By.Id("t_1349"), listing.AgentListApptPhone); // Listing Agent's Phone

            this.uploaderClient.WriteTextbox(By.Id("t_1351"), marketInfo.AgentId); // Secondary Contact
            this.uploaderClient.WriteTextbox(By.Id("t_1353"), listing.OtherPhone); // Secondary Contact's Phone

            this.uploaderClient.WriteTextbox(By.Id("t_1355"), (listing as AmarilloListingRequest).SupervisorLicense); // Listing Agent's License Number

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void FillDetailsForNewListings(ResidentialListingRequest listing)
        {
            this.uploaderClient.SetSelectByText(By.Id("t_2713"), "Negotiable", "Hydrostatic", "Hydrostatic Test"); // Hydrostatic Tests

            this.uploaderClient.SetSelectByText(By.Id("t_2765"), (listing as AmarilloListingRequest).HasLenderOwned.BoolToYesNoBool().ToTitleCase(), LenderOwnerFieldLabel, LenderOwnerFieldLabel); // Lender Owned

            if ((listing as AmarilloListingRequest).HasDistressedSale)
            {
                this.uploaderClient.ClickOnElementById("i_2767"); // Distressed Sale
            }

            if ((listing as AmarilloListingRequest).HasPotentialShortSale)
            {
                this.uploaderClient.ClickOnElementById("i_2769");
                this.uploaderClient.SetSelectByText(By.Id("t_2769"), (listing as AmarilloListingRequest).HasPotentialShortSale.BoolToYesNoBool().ToTitleCase(), PotentialShortSaleFieldLabel, PotentialShortSaleFieldLabel); // Potential Short Sale
            }

            if ((listing as AmarilloListingRequest).HasForeclosed)
            {
                this.uploaderClient.ClickOnElementById("i_2771");
                this.uploaderClient.SetSelectByText(By.Id("t_2771"), (listing as AmarilloListingRequest).HasForeclosed.BoolToYesNoBool().ToTitleCase(), ForeclosedFieldLabel, ForeclosedFieldLabel); // Foreclosed
            }

            if ((listing as AmarilloListingRequest).HasTenant)
            {
                this.uploaderClient.ClickOnElementById("i_2773");
                this.uploaderClient.SetSelectByText(By.Id("t_2773"), (listing as AmarilloListingRequest).HasTenant.BoolToYesNoBool().ToTitleCase(), TenantFieldLabel, TenantFieldLabel); // Tenant
                this.uploaderClient.WriteTextbox(By.Id("t_2775"), (listing as AmarilloListingRequest).DateLeaseExpires); // Date Lease Expires
            }

            this.uploaderClient.ClickOnElementById("i_2777");
            this.uploaderClient.SetSelectByText(By.Id("t_2777"), "No", SurveillanceEquipmentOnSiteFieldLabel, SurveillanceEquipmentOnSiteFieldLabel); // Surveillance equipment on-site

            this.CheckOptions((listing as AmarilloListingRequest).SpecialFeatures); // Spectial Features

            this.uploaderClient.ScrollDownPosition(3000);

            this.uploaderClient.WriteTextbox(By.Id("t_2835"), (listing as AmarilloListingRequest).OtherSpecialFeatures); // Other Spec. Feature

            this.CheckOptions((listing as AmarilloListingRequest).ParkingFeatures); // Parking Features

            this.CheckOptions((listing as AmarilloListingRequest).CommunityFeatures); // Communuity Features

            if ((listing as AmarilloListingRequest).HoaRequirement)
            {
                this.uploaderClient.ClickOnElementById("i_2871");
                this.uploaderClient.SetSelectByText(By.Id("t_2871"), (listing as AmarilloListingRequest).HoaRequirement.BoolToYesNoBool().ToTitleCase(), HOARequiremientFieldLabel, HOARequiremientFieldLabel); // HOA Requiremient
            }

            this.CheckOptions(listing.AppliancesDesc); // Appliances
            this.CheckOptionsByLabel(listing.ExteriorDesc); // Exterior
            this.CheckOptionsByLabel((listing as AmarilloListingRequest).ConstructionType); // Construction type
            this.CheckOptionsByLabel((listing as AmarilloListingRequest).StoriesFeatures); // Stories/Level
            this.CheckOptions(listing.InteriorDesc); // Interior Features

            this.uploaderClient.ClickOnElementById("i_3039");
            this.uploaderClient.SetSelectByText(By.Id("t_3039"), (listing as AmarilloListingRequest).NumQuarterBaths.ToString(), OneQuaterBathFieldLabel, OneQuaterBathFieldLabel); // 1/4 Baths

            this.uploaderClient.ClickOnElementById("i_3041");
            this.uploaderClient.SetSelectByText(By.Id("t_3041"), listing.BathsHalf.ToString(), HalfBathFieldLabel, HalfBathFieldLabel); // 1/2 Baths

            this.uploaderClient.ClickOnElementById("i_3043");
            this.uploaderClient.SetSelectByText(By.Id("t_3043"), (listing as AmarilloListingRequest).NumThreeQuartersBaths.ToString(), ThreeQuaterBathFieldLabel, ThreeQuaterBathFieldLabel); // 3/4 Baths

            this.uploaderClient.ClickOnElementById("i_3045");
            this.uploaderClient.SetSelectByText(By.Id("t_3045"), listing.BathsFull.ToString(), FullBathFieldLabel, FullBathFieldLabel); // Full Baths

            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=Powder]').prop('checked', " + (listing as AmarilloListingRequest).HasPowder.ToString().ToLower() + ")"); // Powder

            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=\"Hollywood/Jack&Jill\"]').prop('checked', " + (listing as AmarilloListingRequest).HasHollywoodJackAndJill.ToString().ToLower() + ")"); // Hollywood/Jack&Jill

            var thereFireplaces = int.TryParse(listing.NumberFireplaces, out _);
            this.uploaderClient.ExecuteScript("jQuery('[data-field-id=Number]').prop('checked', " + thereFireplaces.ToString().ToLower() + ")"); // Fireplaces number
            if (thereFireplaces)
            {
                this.uploaderClient.SetSelectByText(By.Id("t_3055"), listing.NumberFireplaces ?? listing.NumberFireplaces.ToString(), FireplacesNumberFieldLabel, FireplacesNumberFieldLabel); // Fireplaces number
            }

            this.CheckOptionsByLabel(listing.FireplaceDesc); // Fireplaces description
            this.CheckOptions(listing.LaundryLocDesc); // Laundry Location
            this.CheckOptions((listing as AmarilloListingRequest).LaundryFeatures); // Laundry Features
            this.CheckOptions(listing.FenceDesc); // Fence
            this.CheckOptions(listing.RoofDesc); // Roof
            this.CheckOptions(listing.FoundationDesc); // Foundation
            this.CheckOptions((listing as AmarilloListingRequest).DetachedStructures); // Detached Structures
            this.CheckOptions((listing as AmarilloListingRequest).WaterFeature); // Water
            this.CheckOptions(listing.WaterDesc); // Sewer
            this.CheckOptions(listing.PoolDesc); // Pool
            this.CheckOptions(listing.HeatSystemDesc); // Heat
            this.CheckOptions(listing.CoolSystemDesc); // A/C

            this.uploaderClient.WriteTextbox(By.Id("t_3333"), (listing as AmarilloListingRequest).WaterHeater); // # Heaters

            this.uploaderClient.SetSelect(By.Id("t_3351"), listing.SchoolName1); // Elementary School
            this.uploaderClient.SetSelect(By.Id("t_3353"), listing.SchoolName2); // Intermedaite School
            this.uploaderClient.SetSelect(By.Id("t_3355"), listing.HighSchool); // High School
            this.uploaderClient.SetSelect(By.Id("t_3357"), listing.SchoolDistrict); // Disctrict School

            this.CheckOptions(listing.FinancingProposed); // Possible Financing

            this.uploaderClient.WriteTextbox(By.Id("t_3375"), listing.TitleCo); // Suggested Title Company/Address

            var marketInfo = this.options.MarketInfo.Amarillo;

            this.uploaderClient.WriteTextbox(By.Id("t_3381"), marketInfo.AgentId); // Listing Agent
            this.uploaderClient.WriteTextbox(By.Id("t_3383"), listing.AgentListApptPhone); // Listing Agent's Phone

            this.uploaderClient.WriteTextbox(By.Id("t_3385"), marketInfo.AgentId); // Secondary Contact
            this.uploaderClient.WriteTextbox(By.Id("t_3387"), listing.OtherPhone); // Secondary Contact's Phone

            this.uploaderClient.WriteTextbox(By.Id("t_3389"), (listing as AmarilloListingRequest).SupervisorLicense); // Listing Agent's License Number

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void FillRoomInformation(ResidentialListingRequest listing)
        {
            var script = this.uploaderClient.ExecuteScript("return jQuery('a[id^=remove_button_]').length;").ToString();
            int removeButtons = int.Parse(script);
            for (int index = 0; index < removeButtons; index++)
            {
                this.uploaderClient.ClickOnElementById("remove_button_0");
                Thread.Sleep(400);
                this.uploaderClient.AcceptAlertWindow();
            }

            var i = 0;
            foreach (var room in listing.Rooms)
            {
                this.uploaderClient.ClickOnElement(By.Id("addRoomButton"));
                Thread.Sleep(400);

                this.uploaderClient.SetSelect(By.Id($"roomtype_{i}"), room.RoomType, "Room Type", "Rooms"); // Room Type
                this.uploaderClient.WriteTextbox(By.Id($"std_length_{i}"), room.Length, isElementOptional: true); // Length
                this.uploaderClient.WriteTextbox(By.Id($"std_width_{i}"), room.Width, isElementOptional: true); // Width
                this.uploaderClient.SetSelect(By.Id($"std_level_{i}"), room.Level, "Level", "Rooms", isElementOptional: true); // Level
                i++;
            }

            this.uploaderClient.ClickOnElementById(ButtonNextId);

            Thread.Sleep(1000);
        }

        private void CheckOptions(string options)
        {
            if (!string.IsNullOrEmpty(options))
            {
                foreach (var item in options.Split(','))
                {
                    var querySelector = "jQuery('[data-field-id=\"" + item.Trim() + "\"]').prop('checked', true)";
                    this.uploaderClient.ExecuteScript(querySelector);
                }
            }
        }

        private void CheckOptionsByLabel(string options)
        {
            if (!string.IsNullOrEmpty(options))
            {
                foreach (var item in options.Split(','))
                {
                    var labelQuerySelector = "return $('label').filter(function() { return $(this).prop('textContent').trim() == '" + item.Trim() + "'; } ).length";
                    var labelResponse = this.uploaderClient.ExecuteScript(labelQuerySelector).ToString();
                    if (labelResponse == "1")
                    {
                        var querySelector = "$('label').filter(function() { return $(this).prop('textContent').trim() == '" + item.Trim() + "'; } ).parent().parent().find('input[type=checkbox]').prop('checked', true)";
                        this.uploaderClient.ExecuteScript(querySelector);
                    }
                }
            }
        }

        private void FillDropDownWithAutocomplete(string inputValue, string inputId)
        {
            this.uploaderClient.ExecuteScript("jQuery(\"#" + inputId + "\").select2('open')");

            char[] characters = inputValue.ToArray();

            foreach (var character in characters)
            {
                Thread.Sleep(400);
                var script = "var keyEvent = jQuery.Event('keypress');keyEvent.keyCode = " + (int)character + ";jQuery('.select2-search__field').trigger(keyEvent)";
                this.uploaderClient.ExecuteScript(script);
                this.uploaderClient.FindElements(By.ClassName("select2-search__field"))[0].SendKeys(character.ToString());
            }

            Thread.Sleep(2000);
            this.uploaderClient.FindElements(By.ClassName("select2-search__field"))[0].SendKeys(Keys.Down);
            Thread.Sleep(500);
            this.uploaderClient.FindElements(By.ClassName("select2-search__field"))[0].SendKeys(Keys.Enter);
        }

        private void GoToRemarksTab()
        {
            this.GoToTabById("Remarks/Tour Links");
        }

        private void GoToPropertyInformationTab()
        {
            this.GoToTabById("Property Information");
        }

        private void GoToTabById(string elementId)
        {
            this.uploaderClient.ScrollToTop();
            this.uploaderClient.ClickOnElement(By.Id(elementId));
            Thread.Sleep(500);
        }

        private void UpdateYearBuiltDescription(ResidentialListingRequest listing)
        {
            this.uploaderClient.WriteTextbox(By.Id("yr_built"), listing.YearBuilt); // Year Built
        }

        private void FillCompletionDate(ResidentialListingRequest listing)
        {
            if (listing.BuildCompletionDate.HasValue)
            {
                this.uploaderClient.WriteTextbox(By.Id("userdefined31"), listing.BuildCompletionDate); // Est Date of Completion
            }
        }

        private void UpdatePublicRemarksInRemarksTab(ResidentialListingRequest listing)
        {
            var remarks = listing.GetPublicRemarks();
            this.uploaderClient.WriteTextbox(By.Id("remark1"), remarks.Length < 800 ? remarks : remarks.Substring(0, 799), true); // Public Remarks

            var agentRemarks = listing.GetAgentRemarksMessage();
            this.uploaderClient.WriteTextbox(By.Id("remark2"), agentRemarks.Length < 400 ? agentRemarks : agentRemarks.Substring(0, 399), true); // Private Remarks
        }

        private void DeleteAllImages()
        {
            if (this.uploaderClient.FindElements(By.ClassName("photo-select")).Count > 0)
            {
                this.uploaderClient.ClickOnElement(By.Id("selectAllCheckbox"));
                Thread.Sleep(1000);
                this.uploaderClient.WaitForElementToBeVisible(By.Id("delete-selected"), TimeSpan.FromSeconds(10));
                this.uploaderClient.ClickOnElement(By.Id("delete-selected"));
                Thread.Sleep(1000);
                this.uploaderClient.ClickOnElement(By.PartialLinkText("Continue"));
            }
        }

        [SuppressMessage("SonarLint", "S2583", Justification = "Ignored due to suspected false positive")]
        private async Task ProcessImages(ResidentialListingRequest listing, CancellationToken cancellationToken)
        {
            string mediaFolderName = "Husa.Core.Uploader";

            var mediaFiles = await this.mediaRepository.GetListingImages(listing.ResidentialListingRequestID, market: MarketCode.Amarillo, token: cancellationToken);
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            foreach (var photo in mediaFiles)
            {
                Thread.Sleep(500);
                await this.mediaRepository.PrepareImage(photo, MarketCode.Amarillo, cancellationToken, folder);
                this.uploaderClient.FindElement(By.Id("files")).SendKeys(photo.PathOnDisk);
            }

            this.logger.LogInformation("The images upload for listing request {listingRequestId} is complete.", listing.ResidentialListingRequestID);
        }

        private void CleanOpenHouse()
        {
            var currentOpenHouses = this.uploaderClient.ExecuteScript("return $('.open-house-checkbox .openHouseCheckbox').length;").ToString();
            if (currentOpenHouses != "0")
            {
                var countOpenHouses = int.Parse(currentOpenHouses);
                this.uploaderClient.ExecuteScript("$('.open-house-checkbox .openHouseCheckbox').each(function () { $(this).prop('checked', true) }); $('#checkbox-headers').css('display', 'block');");
                this.uploaderClient.ClickOnElementById("delete-selected");
                Thread.Sleep(1000);
                if (countOpenHouses > 1)
                {
                    this.uploaderClient.ClickOnElementById("bulkDeleteModal_confirmationCheckbox");
                    this.uploaderClient.ExecuteScript("$('.confirmBulkActionBtn').click();");
                }
                else
                {
                    this.uploaderClient.ExecuteScript("$('button:contains(\"Yes, I\")').click();");
                }
            }

            Thread.Sleep(1000);
        }

        private void AddOpenHouses(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            var sortedOpenHouses = listing.OpenHouse.OrderBy(openHouse => openHouse.Date).ToList();
            foreach (var openHouse in sortedOpenHouses)
            {
                this.uploaderClient.ExecuteScript("$('button[data-target=\"#modal-open-house\"].add-open-house').click()");
                Thread.Sleep(1000);

                this.uploaderClient.SwitchToLast();
                Thread.Sleep(1000);
                this.uploaderClient.ExecuteScript("const datepicker = $('#oh-date')[0]._flatpickr;datepicker.setDate(\"" + openHouse.Date + "\");"); // Open House Date
                this.uploaderClient.ExecuteScript("const startTimepicker = $('#oh-time-start')[0]._flatpickr;startTimepicker.setDate(\"" + OpenHouseExtensions.GetOpenHouseHours(openHouse.StartTime.To12Format()) + "\");"); // Open House Start Time
                this.uploaderClient.ExecuteScript("const endTimepicker = $('#oh-time-end')[0]._flatpickr;endTimepicker.setDate(\"" + OpenHouseExtensions.GetOpenHouseHours(openHouse.EndTime.To12Format()) + "\");"); // Open House End Time
                Thread.Sleep(1000);
                this.uploaderClient.FindElementsByName("commit")[0].Click();
                Thread.Sleep(2000);
                var window = this.uploaderClient.WindowHandles.FirstOrDefault();
                this.uploaderClient.SwitchTo().Window(windowName: window);
            }

            Thread.Sleep(1000);
        }
    }
}
