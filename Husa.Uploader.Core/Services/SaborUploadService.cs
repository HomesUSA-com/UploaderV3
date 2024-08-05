namespace Husa.Uploader.Core.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
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

    public class SaborUploadService : ISaborUploadService
    {
        private const string StartTimeFunctionScript = " function startTimer(duration, display) {var timer = duration, minutes, seconds;window.setInterval(function () {minutes = parseInt(timer / 60, 10);seconds = parseInt(timer % 60, 10);minutes = minutes < 10 ? \"0\" + minutes : minutes;seconds = seconds < 10 ? \"0\" + seconds : seconds;display.textContent = minutes + \":\" + seconds;if (--timer < 0) {timer = duration;}}, 1000);}";

        private readonly IUploaderClient uploaderClient;
        private readonly IMediaRepository mediaRepository;
        private readonly IServiceSubscriptionClient serviceSubscriptionClient;
        private readonly ApplicationOptions options;
        private readonly ILogger<SaborUploadService> logger;

        public SaborUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            IMediaRepository mediaRepository,
            IServiceSubscriptionClient serviceSubscriptionClient,
            ILogger<SaborUploadService> logger)
        {
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            this.serviceSubscriptionClient = serviceSubscriptionClient ?? throw new ArgumentNullException(nameof(serviceSubscriptionClient));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket => MarketCode.SanAntonio;

        public bool IsFlashRequired => false;

        public bool CanUpload(ResidentialListingRequest listing) => listing.MarketCode == this.CurrentMarket;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public UploadResult Logout()
        {
            this.logger.LogInformation("Logging out of the MLS.");
            this.uploaderClient.SwitchTo().DefaultContent();
            this.uploaderClient.ClickOnElement(
                findBy: By.CssSelector("a[href='servlet/SignOut']"),
                shouldWait: false,
                waitTime: 0,
                isElementOptional: false);

            return UploadResult.Success;
        }

        public async Task<LoginResult> Login(Guid companyId)
        {
            var company = await this.serviceSubscriptionClient.Company.GetCompany(companyId);
            var credentialsTask = this.serviceSubscriptionClient.Corporation.GetMarketReverseProspectInformation(this.CurrentMarket);
            var marketInfo = this.options.MarketInfo.Sabor;
            this.uploaderClient.NavigateToUrl(marketInfo.LogoutUrl);
            Thread.Sleep(1000);
            this.uploaderClient.NavigateToUrl(marketInfo.LoginUrl);
            Thread.Sleep(1000);
            try
            {
                var credentials = await LoginCommon.GetMarketCredentials(company, credentialsTask);
                try
                {
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("go"));
                    this.uploaderClient.WriteTextbox(By.Id("j_username"), credentials[LoginCredentials.Username]);
                    this.uploaderClient.WriteTextbox(By.Id("j_password"), credentials[LoginCredentials.Password]);
                    this.uploaderClient.ClickOnElementByName(elementName: "go");
                }
                catch
                {
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("login"));
                    this.uploaderClient.WriteTextbox(By.Id("userid"), credentials[LoginCredentials.Username]);
                    this.uploaderClient.WriteTextbox(By.Id("password"), credentials[LoginCredentials.Password]);
                    this.uploaderClient.SubmitForm(By.Name("login"));
                }

                if (this.uploaderClient.IsElementPresent(By.Name("remindLater"), isVisible: true))
                {
                    this.uploaderClient.ClickOnElementByName(elementName: "remindLater");
                }

                this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("newlistingLink"));
            }
            catch
            {
                this.uploaderClient.ExecuteScript("let head = document.getElementsByTagName(\"head\")[0];let script = document.createElement(\"script\");script.setAttribute(\"src\", \"https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js\");document.body.appendChild(script);");
                this.uploaderClient.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/animate.css' type='text/css'>\");");

                this.uploaderClient.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/igrowl.css' type='text/css'>\");");

                this.uploaderClient.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/fonts/feather.css' type='text/css'>\");");
                this.uploaderClient.ExecuteScript("$(\"head\").append('<script src=\"https://leadmanager.homesusa.com/Scripts/igrowl.js\"></script>')");
                Thread.Sleep(2000);

                string errorMessageScript = "  $.iGrowl({type: 'error',title: 'HomesUSA - Uploader',message: 'Failed to Log In. Please try to enter the credentials manually within 5 minutes. <div id=\"time\" style=\"font-weight: bold\"></div>',delay: 0,small: false,placement:{ x: 'right', y: 'top'}, offset: {x: 30,y: 50},animShow: 'fadeInDown',animHide: 'bounceOutUp'}); var fiveMinutes = 60 * 5,display = document.getElementById('time'); startTimer(fiveMinutes, display);";
                this.uploaderClient.ExecuteScript($"{StartTimeFunctionScript}{errorMessageScript}");
                this.uploaderClient.WaitUntilElementExists(By.Id("newlistingLink"), waitTime: new TimeSpan(0, 5, 2));
            }

            return LoginResult.Logged;
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
                if (this.uploaderClient.UploadInformation.IsNewListing)
                {
                    this.uploaderClient.ClickOnElementById("newlistingLink");
                    this.NewProperty(listing);
                }
                else
                {
                    Thread.Sleep(1000);
                    this.EditProperty(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript(script: $"jQuery('.dctable-cell > a:contains(\"{listing.MLSNum}\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript(script: "jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();", isScriptOptional: true);
                }

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
                this.logger.LogInformation("Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: listing.IsNewListing);
                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                try
                {
                    if (listing.IsNewListing)
                    {
                        this.uploaderClient.ClickOnElementById("newlistingLink");
                        this.NewProperty(listing);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        this.EditProperty(listing.MLSNum);
                        this.uploaderClient.ExecuteScript(script: "jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                        Thread.Sleep(1000);
                        this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                        Thread.Sleep(1000);
                        this.uploaderClient.ExecuteScript(script: "jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();", isScriptOptional: true);
                    }

                    this.FillGeneralListingInformation(listing);
                    this.FillExteriorInformation(listing);
                    this.FillInteriorInformation(listing);
                    this.FillUtilitiesInformation(listing);
                    this.FillTaxHoaInformation(listing);
                    this.FillOfficeInformation(listing as SaborListingRequest);
                    this.FillRemarksInformation(listing as SaborListingRequest);

                    if (listing.IsNewListing)
                    {
                        await this.FillMedia(listing.ResidentialListingRequestID, cancellationToken);
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

        public Task<UploadResult> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return PartialUploadListing(logIn);

            async Task<UploadResult> PartialUploadListing(bool logIn)
            {
                this.logger.LogInformation("Partial Uploading the information for the listing {requestId}", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: listing.IsNewListing);
                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                try
                {
                    Thread.Sleep(1000);
                    this.EditProperty(listing.MLSNum);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                    Thread.Sleep(1000);
                    try
                    {
                        this.uploaderClient.ExecuteScript(script: "jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();", isScriptOptional: true);
                    }
                    catch
                    {
                        // Ignoring exception because the field is optional
                    }

                    this.FillGeneralListingInformation(listing, isNotPartialFill: false);
                    this.FillTaxHoaInformation(listing);
                    this.FillOfficeInformation(listing as SaborListingRequest);
                    this.FillRemarksInformation(listing as SaborListingRequest);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
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
                const string tabName = "General";
                this.logger.LogInformation("Updating the status of the listing {requestId} in the {tabName} tab.", listing.ResidentialListingRequestID, tabName);

                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                    Thread.Sleep(1000);
                }

                try
                {
                    this.EditProperty(listing.MLSNum);

                    Thread.Sleep(2000);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[0].click();");
                    Thread.Sleep(1000);

                    this.uploaderClient.ExecuteScript(
                        script: "jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();",
                        isScriptOptional: true);

                    this.uploaderClient.SwitchTo().Frame(this.uploaderClient.FindElementById("csframe"));

                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("statuses"), cancellationToken);
                    this.uploaderClient.SetSelect(By.Id("statuses"), listing.ListStatus, "Listing Status", tabName);

                    if (listing.ListStatus == "SLD")
                    {
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("HOWSOLDID"), cancellationToken);
                        this.uploaderClient.WriteTextbox(By.Id("HOWSOLDID"), listing.HowSold); // How Sold/Sale Terms
                        this.uploaderClient.WriteTextbox(By.Id("CLOSEDATE"), listing.ClosedDate.Value.ToString("MM/dd/yyyy")); // Closing Date
                        this.uploaderClient.WriteTextbox(By.Id("SOLDPRICE"), listing.SoldPrice.DecimalToString()); // Sold Price
                        this.uploaderClient.WriteTextbox(By.Id("CONTINFO"), listing.ContingencyInfo); // Contingent Info
                        this.uploaderClient.WriteTextbox(By.Id("SELLCONCES"), listing.SellConcess.PriceWithDollarSign()); // Seller Concessions
                        this.uploaderClient.WriteTextbox(By.Id("SELL_CONC_DESCID"), listing.SellConcessDescription); // Seller Concessions Description
                        this.uploaderClient.FindElement(By.Name("AGTRMRKS")).Clear(); // Agent Confidential Remarks
                    }

                    if (listing.ListStatus == "PDB" || listing.ListStatus == "PND" || listing.ListStatus == "SLD")
                    {
                        this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("CONTDATE"), cancellationToken);
                        this.uploaderClient.WriteTextbox(By.Id("CONTDATE"), listing.ContractDate.HasValue ? listing.ContractDate.Value.ToString("MM/dd/yyyy") : string.Empty); // Contract Date
                        this.uploaderClient.WriteTextbox(By.Id("SELLAGT1"), listing.AgentLoginName);
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

        public Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingPrice(logIn);

            async Task<UploadResult> UpdateListingPrice(bool logIn)
            {
                this.logger.LogInformation("Updating the price of the listing {requestId} to {listPrice}.", listing.ResidentialListingRequestID, listing.ListPrice);

                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                }

                try
                {
                    Thread.Sleep(1000);

                    this.EditProperty(listing.MLSNum);

                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                    this.uploaderClient.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[1].click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();", isScriptOptional: true);
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("csframe"), cancellationToken);
                    this.uploaderClient.SwitchTo().Frame(this.uploaderClient.FindElementById("csframe"));
                    try
                    {
                        Thread.Sleep(1000);
                        this.uploaderClient.ExecuteScript("jQuery('#LISTPRICE').attr('onchange','');");
                        this.uploaderClient.WriteTextbox(By.Id("LISTPRICE"), value: listing.ListPrice); // List Price
                    }
                    catch
                    {
                        this.uploaderClient.ExecuteScript("jQuery('#LISTPRICE').attr('onchange','');");
                        this.uploaderClient.WriteTextbox(By.Id("LISTPRICE"), value: listing.ListPrice);
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

        public Task<UploadResult> UpdateImages(ResidentialListingRequest listing, CancellationToken cancellationToken = default)
        {
            if (listing is null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            return UpdateListingImages();

            async Task<UploadResult> UpdateListingImages()
            {
                this.logger.LogInformation("Updating the media of the listing {requestId}.", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);

                this.EditProperty(listing.MLSNum);

                Thread.Sleep(1000);
                this.uploaderClient.ExecuteScript($"jQuery('.dctable-cell > a:contains(\"{listing.MLSNum}\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                Thread.Sleep(1000);
                this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                this.uploaderClient.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[5].click();");
                Thread.Sleep(1000);
                await this.ProcessImages(listing.ResidentialListingRequestID, cancellationToken);
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
                this.logger.LogInformation("Updating the completion date of the listing {requestId}.", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                if (logIn)
                {
                    await this.Login(listing.CompanyId);
                    Thread.Sleep(1000);
                }

                try
                {
                    this.EditProperty(listing.MLSNum);
                    Thread.Sleep(1000);

                    this.uploaderClient.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                    Thread.Sleep(1000);
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                    Thread.Sleep(1000);
                    this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                    Thread.Sleep(1000);
                    this.FillGeneralCompletionDateInformation(listing.BuildCompletionDate);
                    Thread.Sleep(1000);
                    this.FillRemarksInformation(listing as SaborListingRequest, isCompletionUpdate: true);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Failure uploading the lising {requestId}", listing.ResidentialListingRequestID);
                    return UploadResult.Failure;
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

            return UpdateListingOpenHouse();

            async Task<UploadResult> UpdateListingOpenHouse()
            {
                this.logger.LogInformation("Updating the open house information of the listing {requestId}.", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);
                await this.Login(listing.CompanyId);
                Thread.Sleep(1000);

                this.EditProperty(listing.MLSNum);
                Thread.Sleep(1000);

                this.uploaderClient.ExecuteScript(script: "jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                Thread.Sleep(1000);

                this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[7].click();");
                Thread.Sleep(1000);
                this.uploaderClient.ExecuteScript(
                    script: "jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();",
                    isScriptOptional: true);

                this.DeleteOpenHouses();

                if (listing.EnableOpenHouse)
                {
                    this.AddOpenHouses(listing);
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
                this.logger.LogInformation("Updating the virtual tours of the listing {requestId}.", listing.ResidentialListingRequestID);
                this.uploaderClient.InitializeUploadInfo(listing.ResidentialListingRequestID, isNewListing: false);

                await this.Login(listing.CompanyId);

                this.EditProperty(listing.MLSNum);

                Thread.Sleep(1000);
                this.uploaderClient.ExecuteScript(script: $"jQuery('.dctable-cell > a:contains(\"{listing.MLSNum}\")').parent().parent().find('div:eq(27) > span > a:first').click();");
                Thread.Sleep(1000);
                this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("modal-dialog"), cancellationToken);
                this.uploaderClient.ExecuteScript(script: "jQuery('.modal-body > .inner-modal-body > div').find('button')[6].click();");
                Thread.Sleep(1000);
                this.uploaderClient.SwitchTo().Frame(this.uploaderClient.FindElementById("main"));
                this.uploaderClient.SwitchTo().Frame(this.uploaderClient.FindElementById("workspace"));
                this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("VIRTTOUR"), cancellationToken);
                var virtualTourResponse = await this.mediaRepository.GetListingVirtualTours(listing.ResidentialListingRequestID, market: MarketCode.SanAntonio, cancellationToken);
                var virtualTour = virtualTourResponse.FirstOrDefault();
                if (virtualTour != null)
                {
                    this.uploaderClient.WriteTextbox(By.Name("VIRTTOUR"), virtualTour.GetUnbrandedUrl());
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

        private void NewProperty(ResidentialListingRequest listing)
        {
            const string tabName = "General";
            const string errorMessageScript = "  $.iGrowl({type: 'error',title: 'HomesUSA - Uploader',message: 'Please, enter a valid area code within 5 minutes and click in the button Continue. <button onclick=\'javascript:document.getElementByName(\"save-page\").click()\'>Continue</button> <div id=\"time\" style=\"font-weight: bold\"></div>',delay: 0,small: false,placement:{ x: 'right', y: 'top'}, offset: {x: 30,y: 50},animShow: 'fadeInDown',animHide: 'bounceOutUp'}); var fiveMinutes = 60 * 5,display = document.getElementById('time'); startTimer(fiveMinutes, display);";
            Thread.Sleep(1000);

            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("dcModal"));

            string propertyCategoryType = listing.Category == "SFD" ? "RE" : "MF";
            this.uploaderClient.SetSelectWithScript("category", "property-type-selector", 0, propertyCategoryType, "Class", tabName); // Class
            this.uploaderClient.ExecuteScript("$('.search-options input[type=\"checkbox\"]').attr('id', 'autoPopulateFromTax');$('#autoPopulateFromTax').click();"); // Auto-populate from Tax data
            this.uploaderClient.ExecuteScript("$('.search-options input[type=\"checkbox\"]:last').attr('id', 'manuallyEnterAllData');$('#manuallyEnterAllData').click();"); // Manually enter all listing data
            this.uploaderClient.ExecuteScript("$('.modal-header button:eq(2)').click();"); // Next>>

            Thread.Sleep(3000);

            // Second Page of new listing
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("save-page"));

            this.uploaderClient.WriteTextbox(By.Id("AREAID"), listing.MLSArea); // Area
            try
            {
                this.uploaderClient.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord); // Mapsco Grid
                this.uploaderClient.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice.ToString()); // List Price
            }
            catch
            {
                this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
                this.uploaderClient.ExecuteScript(script: "document.getElementById('AREAID').focus();");
                this.uploaderClient.ExecuteScript(script: "$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/animate.css' type='text/css'>\");");
                this.uploaderClient.ExecuteScript(script: "$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/igrowl.css' type='text/css'>\");");
                this.uploaderClient.ExecuteScript(script: "$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/fonts/feather.css' type='text/css'>\");");
                this.uploaderClient.ExecuteScript(script: "$(\"head\").append('<script src=\"https://leadmanager.homesusa.com/Scripts/igrowl.js\"></script>')");
                Thread.Sleep(2000);
                this.uploaderClient.ExecuteScript(script: $"{StartTimeFunctionScript}{errorMessageScript}");

                this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("fakeElement"), waitTime: new TimeSpan(0, 5, 2));

                this.uploaderClient.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord); // Mapsco Grid
                this.uploaderClient.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice.ToString()); // List Price
            }

            this.uploaderClient.WriteTextbox(By.Id("ADDRNUMBER"), listing.StreetNum); // Street Number
            this.uploaderClient.WriteTextbox(By.Id("ADDRSTREET"), listing.StreetName); // Street Name
            this.uploaderClient.WriteTextbox(By.Id("CITYID"), listing.CityCode); // City

            this.uploaderClient.WriteTextbox(By.Id("STATEID"), listing.State); // State
            this.uploaderClient.WriteTextbox(By.Id("zip5"), listing.Zip); // Zip
            this.uploaderClient.WriteTextbox(By.Id("COUNTYID"), listing.County); // County
            if (listing.TaxID != null)
            {
                this.uploaderClient.WriteTextbox(By.Id("COUNTACTNO"), listing.TaxID);
            }

            this.uploaderClient.WriteTextbox(By.Id("MULTIPLE_CANSID"), entry: "NO"); // Multiple County AcctNos
            if (this.uploaderClient.FindElement(By.Id("AREAID")) != null)
            {
                this.uploaderClient.ClickOnElementByName(elementName: "save-page");
                var addressSuggestion = this.uploaderClient.FindElement(By.ClassName("address-suggestion"), isElementOptional: true);
                if (addressSuggestion != null && addressSuggestion.Displayed)
                {
                    this.uploaderClient.ExecuteScript(script: "ignoreSuggestions();", isScriptOptional: true);
                    this.uploaderClient.ClickOnElementByName(elementName: "save-page", isElementOptional: true);
                }
            }
        }

        [SuppressMessage("SonarLint", "S2589", Justification = "Ignored due to suspected false positive")]
        private void EditProperty(string mlsnum)
        {
            const string tabName = "General";
            this.uploaderClient.ScrollDown(500);
            this.uploaderClient.FindElement(By.Id("listTxnsLink")).Click();
            this.uploaderClient.WaitUntilElementIsDisplayed(By.ClassName("mylistcontainer"));

            this.uploaderClient.ExecuteScript("jQuery('.select-list-styled:eq(1) > select').attr('id','selectlist');");
            this.uploaderClient.SetSelect(By.Id("selectlist"), "ALL", "Filters", tabName);
            Thread.Sleep(2000);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("mylistings"));

            var mlsFound = false;
            while (!mlsFound)
            {
                try
                {
                    var result = this.uploaderClient.ExecuteScript("return jQuery('.dctable-cell > a:contains(\"" + mlsnum + "\")').length;").ToString();
                    if (result != "0")
                    {
                        mlsFound = true;
                        break;
                    }

                    var nextButtonVisible = this.uploaderClient.ExecuteScript("return jQuery('.main-content > .mylistcontainer > .affix-top > ul.pagination > li').length;").ToString();
                    if (!string.IsNullOrEmpty(nextButtonVisible) && nextButtonVisible != "0")
                    {
                        int nextButtonIndex = (int.Parse(nextButtonVisible) > 0) ? (int.Parse(nextButtonVisible) - 2) : 0;
                        string nextButtonDisabled = this.uploaderClient.ExecuteScript("return jQuery('ul.pagination > li:eq(" + nextButtonIndex + ")').is(':disabled');").ToString();
                        if (!bool.Parse(nextButtonDisabled))
                        {
                            this.uploaderClient.ExecuteScript("return jQuery('ul.pagination > li:eq(" + nextButtonIndex + ") > a').click();", isScriptOptional: true);
                            mlsFound = false;
                        }
                    }

                    Thread.Sleep(3000);
                }
                catch
                {
                    Thread.Sleep(3000);
                }
            }
        }

        private void FillGeneralCompletionDateInformation(DateTime? listingBuildCompletionDate)
        {
            this.uploaderClient.WriteTextbox(By.Name("NEW_CONST_EST_COMPLETION"), listingBuildCompletionDate.Value.ToString("MM/yy"), isElementOptional: true); // Construction
        }

        private void FillGeneralListingInformation(ResidentialListingRequest listing, bool isNotPartialFill = true)
        {
            Thread.Sleep(1000);
            string direction = listing.Directions;
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
                    direction += ".";
                }
            }

            this.uploaderClient.WriteTextbox(By.Name("INSTORDIR"), direction); // Inst/Dir
            this.uploaderClient.WriteTextbox(By.Name("HOMEFACES"), listing.FacesDesc, true); // Home Faces
            if (listing.YearBuilt.HasValue)
            {
                this.uploaderClient.WriteTextbox(By.Name("YEAR_BUILT"), listing.YearBuilt.Value); // Year Built
            }
            else
            {
                var unknownYearBuiltElement = this.uploaderClient.FindElement(By.Id("unknownYEAR_BUILTU"), shouldWait: false, isElementOptional: true);
                if (unknownYearBuiltElement != null && !unknownYearBuiltElement.Selected)
                {
                    this.uploaderClient.ClickOnElementById("unknownYEAR_BUILTU");
                }
            }

            this.uploaderClient.WriteTextbox(By.Name("SQFEET"), listing.SqFtTotal, true); // Square Feet
            this.uploaderClient.WriteTextbox(By.Name("SOURCESQFT"), listing.SqFtSource, true); // Source SQFT/Acre
            if (string.IsNullOrWhiteSpace(listing.SqFtSource))
            {
                this.uploaderClient.WriteTextbox(By.Name("SOURCESQFT"), "B"); // Source SQFT/Acre
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Name("SOURCESQFT"), listing.SqFtSource); // Source SQFT/Acre
            }

            this.uploaderClient.SetAttribute(By.Name("SCHLDIST"), listing.SchoolDistrict.ToUpper(), "value"); // School District
            this.uploaderClient.SetAttribute(By.Name("ELEMSCHL"), listing.SchoolName1, "value"); // Elementary School

            this.uploaderClient.SetAttribute(By.Name("MIDSCHL"), listing.SchoolName2, "value"); // Middle School
            this.uploaderClient.SetAttribute(By.Name("HIGHSCHL"), listing.HighSchool, "value"); // High School
            this.uploaderClient.WriteTextbox(By.Name("CONSTRCTN"), entry: "NEW"); // Construction

            if (isNotPartialFill)
            {
                this.uploaderClient.SetAttribute(By.Name("TYPE"), listing.Category, attributeName: "value"); // Type
                this.uploaderClient.WriteTextbox(By.Name("BLOCK"), listing.Block); // Block
                this.uploaderClient.WriteTextbox(By.Name("LGLDSCLOT"), listing.LotNum); // Legal Desc-Lot
                this.uploaderClient.SetAttribute(By.Name("SBDIVISION"), listing.Subdivision, "value"); // Subdivision (Legal Name)
                this.uploaderClient.SetAttribute(By.Name("SUBDIVISION_CKA"), listing.Subdivision, "value"); // Subdivision (Common Name)
                this.uploaderClient.WriteTextbox(By.Name("LEGALDESC"), listing.Legal); // Legal Description
                this.uploaderClient.FindElement(By.Name("CONSTRCTN")).SendKeys(Keys.Tab);
                this.uploaderClient.WriteTextbox(By.Name("NEW_CONST_EST_COMPLETION"), listing.BuildCompletionDate.Value.ToString("MM/yy"), isElementOptional: true); // Construction
                this.uploaderClient.WriteTextbox(By.Name("BLDRNAME"), listing.OwnerName); // Builder Name
                this.uploaderClient.WriteTextbox(By.Name("ACCESS_HOME"), listing.HasHandicapAmenities); // Accessible/Adaptive Home

                try
                {
                    this.uploaderClient.WriteTextbox(By.Name("NGHBRHDMNT"), listing.CommonFeatures); // Neighborhood Amenities
                    if (listing.HasHandicapAmenities == "YES")
                    {
                        if (!string.IsNullOrEmpty(listing.AccessibilityDesc))
                        {
                            this.uploaderClient.WriteTextbox(By.Name("ACESIBILTY"), listing.AccessibilityDesc, isElementOptional: false); // Accessible/Adaptive Details
                        }
                    }
                }
                catch
                {
                    // Ignoring exception because the fields are optional
                }

                if (listing.BuiltStatus != BuiltStatus.ReadyNow)
                {
                    this.uploaderClient.WriteTextbox(By.Name("MISCELANES"), entry: "UNDCN", isElementOptional: true); // Miscellaneous
                }
                else
                {
                    this.uploaderClient.ExecuteScript("clearPicklist('MISCELANEStable');selectVals('MISCELANES');closeDiv();");
                }

                this.uploaderClient.WriteTextbox(By.Name("GREEN_CERT"), listing.GreenCerts, isElementOptional: true); // Green Certification
                this.uploaderClient.WriteTextbox(By.Name("GREEN_FEAT"), listing.GreenFeatures, isElementOptional: true); // Green Features
                this.uploaderClient.WriteTextbox(By.Name("ENERGY_EFF"), listing.EnergyDesc, isElementOptional: true); // Energy Efficiency
            }
        }

        private void FillExteriorInformation(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('1') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("STYLE"), listing.HousingStyleDesc); // Style
            this.uploaderClient.FindElement(By.Name("STYLE")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("NOSTRY"), listing.NumStories); // # of Stories
            this.uploaderClient.FindElement(By.Name("NOSTRY")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("EXTERIOR"), listing.ExteriorDesc); // Exterior
            this.uploaderClient.FindElement(By.Name("EXTERIOR")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.SetAttribute(By.Name("ROOF"), listing.RoofDesc, "value"); // Roof
            this.uploaderClient.SetAttribute(By.Name("ROOF"), listing.RoofDesc, "value"); // Roof
            this.uploaderClient.FindElement(By.Name("ROOF")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("FOUNDATION"), listing.FoundationDesc); // Foundation
            this.uploaderClient.FindElement(By.Name("FOUNDATION")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            try
            {
                if (!string.IsNullOrWhiteSpace(listing.ParkingDesc))
                {
                    this.uploaderClient.WriteTextbox(By.Name("PARKING"), listing.ParkingDesc); // Parking
                    this.uploaderClient.FindElement(By.Name("PARKING")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                this.ShowMarkToFieldByNameElement("PARKING");
            }

            this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
            this.uploaderClient.ExecuteScript(" closeDiv(); ");
            this.uploaderClient.WriteTextbox(By.Name("ADDL_PARKING"), listing.OtherParking); // Additional/Other Parking
            this.uploaderClient.FindElement(By.Name("ADDL_PARKING")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("POOL"), listing.HasPool ? "Y" : "N"); // Pool
            if (!string.IsNullOrWhiteSpace(listing.PoolDesc)) //// Pool/Spa
            {
                this.uploaderClient.WriteTextbox(By.Name("POOLSPA"), listing.PoolDesc);
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Name("POOLSPA"), "NONE");
            }

            this.uploaderClient.WriteTextbox(By.Name("EXTERRFTRS"), listing.ExteriorFeatures, true); // Exterior Features
            this.uploaderClient.FindElement(By.Name("EXTERRFTRS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("LOTSIZE"), listing.LotSize); // Lot Size (Acres)
            this.uploaderClient.FindElement(By.Name("LOTSIZE")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("LOTDSCRPTN"), listing.LotDesc, true); // Lot Description
            this.uploaderClient.FindElement(By.Name("LOTDSCRPTN")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("LOTDIMENSIONS"), listing.LotDim, true); // Lot Dimensions
            this.uploaderClient.FindElement(By.Name("LOTDIMENSIONS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            this.uploaderClient.WriteTextbox(By.Name("LTMPRVMNTS"), listing.UtilitiesDesc, true); // Lot Improvements
        }

        private void FillInteriorInformation(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('2') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("INTERIOR"), listing.InteriorDesc); // Interior
            this.uploaderClient.WriteTextbox(By.Name("INCLUSIONS"), listing.InclusionsDesc); // Inclusions
            try
            {
                if (!string.IsNullOrWhiteSpace(listing.FloorsDesc))
                {
                    this.uploaderClient.WriteTextbox(By.Name("FLOOR"), listing.FloorsDesc); // Floor
                    this.uploaderClient.FindElement(By.Name("FLOOR")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                this.ShowMarkToFieldByNameElement("FLOOR");
            }

            this.uploaderClient.AcceptAlertWindow(isElementOptional: true);

            try
            {
                if (!string.IsNullOrWhiteSpace(listing.NumberFireplaces))
                {
                    this.uploaderClient.WriteTextbox(By.Name("FIREPLACE1"), listing.NumberFireplaces); // # Fireplaces
                    this.uploaderClient.FindElement(By.Name("FIREPLACE1")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                this.ShowMarkToFieldByNameElement("FIREPLACE1");
            }

            this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
            this.uploaderClient.ExecuteScript(" closeDiv(); ");

            try
            {
                if (this.uploaderClient.WaitUntilElementIsDisplayed(By.Name("FIREPLACE")) &&
                    !string.IsNullOrWhiteSpace(listing.FireplaceDesc) &&
                    !listing.FireplaceDesc.Equals("NA"))
                {
                    this.uploaderClient.WriteTextbox(By.Name("FIREPLACE"), listing.FireplaceDesc); // Fireplace
                    this.uploaderClient.FindElement(By.Name("FIREPLACE")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                this.ShowMarkToFieldByNameElement("FIREPLACE");
            }

            this.uploaderClient.AcceptAlertWindow(isElementOptional: true);
            this.uploaderClient.ExecuteScript(" closeDiv(); ");
            this.uploaderClient.WriteTextbox(By.Name("WNDWCVRNGS"), listing.WindowCoverings); // Window Coverings
            this.uploaderClient.WriteTextbox(By.Name("BEDROOMS"), listing.Beds); // Bedrooms
            this.uploaderClient.ExecuteScript(" BEDROOMSActions() ");
            this.uploaderClient.WriteTextbox(By.Name("MASTERBDRM"), listing.Bed1Desc); // Master Bedroom
            if (listing.ClosetLength > 0 && listing.ClosetWidth > 0)
            {
                this.uploaderClient.WriteTextbox(By.Name("leftMBRCLOSET_SIZE"), listing.ClosetLength);
                this.uploaderClient.WriteTextbox(By.Name("rightMBRCLOSET_SIZE"), listing.ClosetWidth);
                this.uploaderClient.FindElement(By.Name("rightMBRCLOSET_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);

                this.uploaderClient.WriteTextbox(By.Name("MBRCLOSET_LEVEL"), listing.Bed1Level, isElementOptional: true); // Master Bedroom Level
            }

            this.uploaderClient.WriteTextbox(By.Name("leftADDEDIT_BATHS"), listing.BathsFull); // Bathrooms Full
            this.uploaderClient.WriteTextbox(By.Name("rightADDEDIT_BATHS"), listing.BathsHalf); // Bathrooms Half
            this.uploaderClient.FindElement(By.Name("rightADDEDIT_BATHS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            try
            {
                this.uploaderClient.WriteTextbox(By.Name("MASTERBATH"), listing.BedBathDesc); // Master Bath
                this.uploaderClient.ExecuteScript(" MASTERBATHActions(); closeDiv(); ");
                // selectVals('MASTERBATH'); ; MASTERBATHActions(); closeDiv();
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.LivingRoom3Length != null && listing.LivingRoom3Width != null) //// Entry Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftENTRM_SIZE"), listing.LivingRoom3Length, isElementOptional: true);  // Length
                    this.uploaderClient.WriteTextbox(By.Name("rightENTRM_SIZE"), listing.LivingRoom3Width, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightENTRM_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("ENTRM_LEVEL"), listing.LivingRoom3Level, isElementOptional: true); // Entry Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.LivingRoom1Length != null && listing.LivingRoom1Width != null) //// Living Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftLVRM_SIZE"), listing.LivingRoom1Length, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightLVRM_SIZE"), listing.LivingRoom1Width, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightLVRM_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("LVRM_LEVEL"), listing.LivingRoom1Level, isElementOptional: true); // Living Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.LivingRoom2Length != null && listing.LivingRoom2Width != null) //// Family Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftFAMRM_SIZE"), listing.LivingRoom2Length, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightFAMRM_SIZE"), listing.LivingRoom2Width, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightFAMRM_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("FAMRM_LEVEL"), listing.LivingRoom2Level, true); // Family Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.StudyLength != null && listing.StudyWidth != null) //// Study/Office Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftSTYOROF_SIZE"), listing.StudyLength, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightSTYOROF_SIZE"), listing.StudyWidth, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightSTYOROF_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("STYOROF_LEVEL"), listing.StudyLevel, isElementOptional: true); // Study/Office level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            if (listing.KitchenLength != null && listing.KitchenWidth != null) //// Kitchen Size
            {
                this.uploaderClient.WriteTextbox(By.Name("leftKIT_SIZE"), listing.KitchenLength); // Lenght
                this.uploaderClient.WriteTextbox(By.Name("rightKIT_SIZE"), listing.KitchenWidth); // Width
                this.uploaderClient.FindElement(By.Name("rightKIT_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                this.uploaderClient.WriteTextbox(By.Name("KIT_LEVEL"), listing.KitchenLevel, isElementOptional: true); // Kitchen Room level
            }

            try
            {
                if (listing.BreakfastLength != null && listing.BreakfastWidth != null) //// Breakfast Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftBFSTRM_SIZE"), listing.BreakfastLength, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightBFSTRM_SIZE"), listing.BreakfastWidth, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightBFSTRM_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("BFSTRM_LEVEL"), listing.BreakfastLevel, isElementOptional: true); // Breakfast Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.DiningRoomLength != null && listing.DiningRoomWidth != null) //// Dining Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftDINR_SIZE"), listing.DiningRoomLength, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightDINR_SIZE"), listing.DiningRoomWidth, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightDINR_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("DINR_LEVEL"), listing.DiningRoomLevel, isElementOptional: true); // Dining Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.UtilityRoomLength != null && listing.UtilityRoomWidth != null) //// Utility Room Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftUTLRM_SIZE"), listing.UtilityRoomLength, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightUTLRM_SIZE"), listing.UtilityRoomWidth, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightUTLRM_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("UTLRM_LEVEL"), listing.UtilityRoomLevel, isElementOptional: true); // Utility Room level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bed1Length != null && listing.Bed1Width != null) //// Master Bedroom Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftMBR_SIZE"), listing.Bed1Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightMBR_SIZE"), listing.Bed1Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightMBR_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("MBR_LEVEL"), listing.Bed1Level, isElementOptional: true); // Master Bedroom level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (!string.IsNullOrEmpty(listing.Bed1Desc) && listing.Bed1Desc.Contains("DUAL")
                && listing.Mbr2Len != null && listing.Mbr2Wid != null) //// Master Bedroom 2 Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftMBR2_SIZE"), listing.Mbr2Len); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightMBR2_SIZE"), listing.Mbr2Wid); // Width
                    this.uploaderClient.FindElement(By.Name("rightMBR2_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("MBR2_LEVEL"), listing.MBR2LEVEL, isElementOptional: true); // Master Bedroom level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bath1Length != null && listing.Bath1Width != null) //// Master Bath Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftMBTH_SIZE"), listing.Bath1Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightMBTH_SIZE"), listing.Bath1Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightMBTH_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("MBTH_LEVEL"), listing.Bath1Level, isElementOptional: true); // Master Bedroom level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bed2Length != null && listing.Bed2Width != null) //// Bedroom 2 Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftBDRM2_SIZE"), listing.Bed2Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightBDRM2_SIZE"), listing.Bed2Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightBDRM2_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("BDRM2_LEVEL"), listing.Bed2Level, isElementOptional: true); // Bedroom 2 level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bed3Length != null && listing.Bed3Width != null) //// Bedroom 3 Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftBDRM3_SIZE"), listing.Bed3Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightBDRM3_SIZE"), listing.Bed3Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightBDRM3_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("BDRM3_LEVEL"), listing.Bed3Level, isElementOptional: true); // Bedroom 3 level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bed4Length != null && listing.Bed4Width != null) //// Bedroom 4 Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftBDRM4_SIZE"), listing.Bed4Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightBDRM4_SIZE"), listing.Bed4Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightBDRM4_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("BDRM4_LEVEL"), listing.Bed4Level, isElementOptional: true); // Bedroom 4 level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                if (listing.Bed5Length != null && listing.Bed5Width != null) //// Bedroom 5 Size
                {
                    this.uploaderClient.WriteTextbox(By.Name("leftBDRM5_SIZE"), listing.Bed5Length, isElementOptional: true); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightBDRM5_SIZE"), listing.Bed5Width, isElementOptional: true); // Width
                    this.uploaderClient.FindElement(By.Name("rightBDRM5_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    this.uploaderClient.WriteTextbox(By.Name("BDRM45_LEVEL"), listing.Bed5Level, isElementOptional: true); // Bedroom 5 Level
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                // 25.
                if (listing.OtherRoom1Length != null && listing.OtherRoom1Width != null)
                {
                    this.uploaderClient.WriteTextbox(By.Name("OTHER_ROOMS"), "OTHR"); // Other Rooms
                    this.uploaderClient.FindElement(By.Name("OTHER_ROOMS")).SendKeys(Keys.Tab);

                    this.uploaderClient.WriteTextbox(By.Name("A1N"), "GAME");
                    this.uploaderClient.FindElement(By.Name("A1N")).SendKeys(Keys.Tab);

                    this.uploaderClient.WriteTextbox(By.Name("leftA1S"), listing.OtherRoom1Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightA1S"), listing.OtherRoom1Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightA1S")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);

                    this.uploaderClient.WriteTextbox(By.Name("A1L"), listing.OtherRoom1Level); // Other Room 1 Level
                    this.uploaderClient.FindElement(By.Name("A1L")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            try
            {
                // 26.
                if (listing.OtherRoom2Length != null && listing.OtherRoom2Width != null)
                {
                    this.uploaderClient.WriteTextbox(By.Name("A2N"), "MDIA");
                    this.uploaderClient.FindElement(By.Name("A2N")).SendKeys(Keys.Tab);

                    this.uploaderClient.WriteTextbox(By.Name("leftA2S"), listing.OtherRoom2Length); // Lenght
                    this.uploaderClient.WriteTextbox(By.Name("rightA2S"), listing.OtherRoom2Width); // Width
                    this.uploaderClient.FindElement(By.Name("rightA2S")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);

                    this.uploaderClient.WriteTextbox(By.Name("A2L"), listing.OtherRoom2Level); // Other Room 1 Level
                    this.uploaderClient.FindElement(By.Name("A2L")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }
        }

        private void FillUtilitiesInformation(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('3') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("AIRCNDTNNG"), listing.CoolSystemDesc); // Air Conditioning
            this.uploaderClient.FindElement(By.Name("AIRCNDTNNG")).SendKeys(Keys.Tab);

            this.uploaderClient.WriteTextbox(By.Name("HEATING"), listing.HeatSystemDesc); // Heating
            this.uploaderClient.FindElement(By.Name("HEATING")).SendKeys(Keys.Tab);

            this.uploaderClient.WriteTextbox(By.Name("HEATINGFUL"), listing.HeatingFuel); // Heating Fuel
            this.uploaderClient.FindElement(By.Name("HEATINGFUL")).SendKeys(Keys.Tab);

            this.uploaderClient.WriteTextbox(By.Name("WATERSEWER"), listing.WaterDesc); // Water/Sewer
            this.uploaderClient.FindElement(By.Name("WATERSEWER")).SendKeys(Keys.Tab);

            this.uploaderClient.WriteTextbox(By.Name("UTSPELEC"), listing.SupElectricity, isElementOptional: true); // Utility Supplier: Elec
            this.uploaderClient.WriteTextbox(By.Name("UTSPGAS"), listing.SupGas, isElementOptional: true); // Utility Supplier: Gas
            this.uploaderClient.WriteTextbox(By.Name("UTSPWATER"), listing.SupWater, isElementOptional: true); // Utility Supplier: Water
            this.uploaderClient.WriteTextbox(By.Name("UTSPSEWER"), listing.SupSewer, isElementOptional: true); // Utility Supplier: Sewer
            this.uploaderClient.WriteTextbox(By.Name("UTSPGRBGE"), listing.SupGarbage, isElementOptional: true); // Utility Supplier: Grbge
            this.uploaderClient.WriteTextbox(By.Name("UTSPOTHER"), listing.SupOther, isElementOptional: true); // Utility Supplier: Other
        }

        private void FillTaxHoaInformation(ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('4') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("MTPLCNTY"), "NO"); // Taxed by Mltpl Counties
            this.uploaderClient.WriteTextbox(By.Name("TAX_YEAR"), listing.TaxYear); // Certified Tax Year
            this.uploaderClient.WriteTextbox(By.Name("TOTALTAX"), listing.TaxRate); // Total Tax (Without Exemptions)
            this.uploaderClient.WriteTextbox(By.Name("HOAMNDTRY"), listing.HOA); // HOA
            if (listing.HOA == "MAND")
            {
                this.uploaderClient.FindElement(By.Name("HOAMNDTRY")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);

                this.uploaderClient.AcceptAlertWindow(isElementOptional: true);

                Thread.Sleep(500);
                this.uploaderClient.ExecuteScript("openPicklist('HOAMNDTRY')");
                this.uploaderClient.ExecuteScript("selectVals('HOAMNDTRY'); ; HOAMNDTRYActions(); closeDiv();");
                Thread.Sleep(1000);
                this.uploaderClient.AcceptAlertWindow(isElementOptional: true);

                this.uploaderClient.WriteTextbox(By.Name("MLTPLHOA"), listing.HasMultipleHOA); // Multiple
                if (listing.HasMultipleHOA == "YES")
                {
                    this.uploaderClient.FindElement(By.Name("MLTPLHOA")).SendKeys(Keys.Tab);
                    this.uploaderClient.WriteTextbox(By.Name("NUM_HOA"), listing.NumHoas);
                }

                this.FillHoaInfo(listing.HOAs);
            }
        }

        private void FillHoaInfo(IList<HoaRequest> hoaRequests)
        {
            var i = 1;
            foreach (var hoaRequest in hoaRequests)
            {
                var hoaAttr = this.SelectHoaAttr(i);

                this.uploaderClient.WriteTextbox(By.Name(hoaAttr[0]), hoaRequest.Fee); // HOA Fee
                this.uploaderClient.WriteTextbox(By.Name(hoaAttr[1]), hoaRequest.Name); // HOA Name
                this.uploaderClient.WriteTextbox(By.Name(hoaAttr[2]), hoaRequest.FeePaid); // Payment Frequency
                this.uploaderClient.WriteTextbox(By.Name(hoaAttr[3]), hoaRequest.TransferFee); // Assoc Transfer Fee

                this.FillHoaPhone(hoaRequest.Phone, i);
                i++;
            }
        }

        private void FillHoaPhone(HoaPhone phone, int numHoa)
        {
            if (!phone.IsEmpty())
            {
                var attrNames = this.SelectHoaPhoneAttr(numHoa);
                this.uploaderClient.WriteTextbox(By.Name(attrNames[0]), phone.AreaCode, isElementOptional: true); // HOA Contact form number 1
                this.uploaderClient.WriteTextbox(By.Name(attrNames[1]), phone.Prefix, isElementOptional: true); // HOA Contact form number 2
                this.uploaderClient.WriteTextbox(By.Name(attrNames[2]), phone.LineNumber, isElementOptional: true); // HOA Contact form number 3
            }
        }

        private List<string> SelectHoaAttr(int numHoa)
        {
            var list = new List<string> { "HOAFEE", "HOANAME", "PYMNTFREQ", "ASNTRNFEE", "HOAWEBSITE" };
            if (numHoa < 2)
            {
                return list;
            }

            return list.Select(x => $"{x}{numHoa}").ToList();
        }

        private List<string> SelectHoaPhoneAttr(int numHoa) => numHoa switch
        {
            1 => new List<string> { "HOAPHONE1", "HOAPHONE2", "HOAPHONE3" },
            2 => new List<string> { "HOAPHONE21", "HOAPHONE22", "HOAPHONE23" },
            3 => new List<string> { "HOAPHONE31", "HOAPHONE32", "HOAPHONE33" },
            4 => new List<string> { "HOAPHONE41", "HOAPHONE42", "HOAPHONE43" },
            _ => throw new ArgumentOutOfRangeException(nameof(numHoa)),
        };

        private void FillOfficeInformation(SaborListingRequest listing)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('5') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("CONTRACT"), "EA"); // Contract

            if (listing.ExpiredDate != null)
            {
                this.uploaderClient.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate.Value.Date.ToString("MM/dd/yyyy"), isElementOptional: true); // Expiration Date
            }

            if (this.uploaderClient.UploadInformation.IsNewListing)
            {
                var listDate = DateTime.Now;
                if (listing.ListStatus.ToLower() == "pnd")
                {
                    listDate = listDate.AddDays(-2);
                }
                else if (listing.ListStatus.ToLower() == "sld")
                {
                    listDate = listDate.AddDays(-this.options.ListDateSold);
                }

                var now = listDate.ToString("MM/dd/yyyy");
                this.uploaderClient.WriteTextbox(By.Name("LSTDATE"), now); // List Date
                this.uploaderClient.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate.Value.Date.ToString("MM/dd/yyyy")); // Expiration Date
            }

            this.uploaderClient.WriteTextbox(By.Name("PROPSDTRMS"), listing.ProposedTerms); // Proposed Terms
            Thread.Sleep(400);
            this.uploaderClient.WriteTextbox(By.Name("POSSESSION"), "NEGO"); // Possession
            this.uploaderClient.WriteTextbox(By.Name("PHTOSHOW"), listing.AgentListApptPhone); // Ph to Show

            this.uploaderClient.WriteTextbox(By.Name("SHWCONTCT"), listing.Showing); // Showing Contact
            this.uploaderClient.WriteTextbox(By.Name("LOCKBOX"), entry: "NONE"); // Lockbox Type
            this.uploaderClient.WriteTextbox(By.Name("OCCUPANCY"), entry: "VACNT", isElementOptional: true); // Occupancy
            this.uploaderClient.WriteTextbox(By.Name("CURRENTLY_LEASED"), entry: "NO", isElementOptional: true); // Currently Being Leased
            this.uploaderClient.WriteTextbox(By.Name("OWNER"), listing.OwnerName); // Owner

            this.uploaderClient.WriteTextbox(By.Name("LREAORLREB"), "NO"); // Owner LREA/LREB
            this.uploaderClient.WriteTextbox(By.Name("PRFTITLECO"), listing.TitleCo); // Preferred Title Company
            this.uploaderClient.WriteTextbox(By.Name("POT_SS_YNID"), "NO", isElementOptional: true); // Potential Short Sale
        }

        private void FillRemarksInformation(SaborListingRequest listing, bool isCompletionUpdate = false)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(" next_tab('6') ");
            Thread.Sleep(2000);

            if (!isCompletionUpdate)
            {
                this.ClickNextButton();

                var message = listing.GetAgentRemarksMessage();

                Thread.Sleep(2000);

                this.uploaderClient.WriteTextbox(By.Name("AGTRMRKS"), entry: string.Empty);
                this.uploaderClient.WriteTextbox(
                    findBy: By.Name("AGTRMRKS"),
                    entry: message,
                    isElementOptional: true); // Agent Confidential Rmrks
            }

            var remarks = listing.GetPublicRemarks();
            Thread.Sleep(2000);
            this.uploaderClient.WriteTextbox(By.Name("REMARKS"), remarks, isElementOptional: true); // Public Remarks
        }

        private async Task FillMedia(Guid residentialListingRequestID, CancellationToken cancellationToken)
        {
            Thread.Sleep(1000);
            this.uploaderClient.ExecuteScript(script: " next_tab('7') ");
            Thread.Sleep(2000);

            this.uploaderClient.WriteTextbox(By.Name("RTS"), entry: "NO"); // Are any property photos virtually staged?
            this.uploaderClient.FindElement(By.Id("managephotosbutton")).Click();
            Thread.Sleep(1000);
            await this.ProcessImages(residentialListingRequestID, cancellationToken);
            Thread.Sleep(3000);
        }

        private void ClickNextButton()
        {
            const string xpath = "//input[@value=\"Next Page >>\"]";
            var element = this.uploaderClient.FindElement(By.XPath(xpath), isElementOptional: true);
            if (element != null && element.Displayed)
            {
                this.uploaderClient.FindElements(By.XPath(xpath))[1].Click();
            }
        }

        private void FinalizeInsert(ResidentialListingRequest listing)
        {
            this.ClickNextButton();
            this.uploaderClient.FindElements(By.Name("save-page"))[1].Click();
            this.uploaderClient.FindElements(By.Name("Assign Listing Number"))[1].Click();
            var mlsNumber = this.uploaderClient.FindElement(By.Name("dc")).FindElement(By.XPath("//table//td//b")).Text;
            if (!string.IsNullOrEmpty(mlsNumber))
            {
                listing.MLSNum = mlsNumber;
            }

            this.uploaderClient.FindElements(By.Name("    OK    "))[1].Click();
        }

        private async Task ProcessImages(Guid residentialListingRequestID, CancellationToken token)
        {
            string mediaFolderName = "Husa.Core.Uploader";

            this.uploaderClient.WaitUntilElementExists(By.Id("photo-browser"), token);
            if (!this.uploaderClient.UploadInformation.IsNewListing)
            {
                this.DeleteResources();
            }

            Thread.Sleep(1000);

            var mediaFiles = await this.mediaRepository.GetListingImages(residentialListingRequestID, market: MarketCode.SanAntonio, token);
            var folder = Path.Combine(Path.GetTempPath(), mediaFolderName, Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            foreach (var photo in mediaFiles)
            {
                Thread.Sleep(500);
                await this.mediaRepository.PrepareImage(photo, MarketCode.SanAntonio, token, folder);
                this.uploaderClient.FindElement(By.Name("files[]")).SendKeys(photo.PathOnDisk);
            }

            var isUploadInProgress = false;
            while (!isUploadInProgress)
            {
                try
                {
                    isUploadInProgress = this.uploaderClient.WaitUntilElementDisappears(By.ClassName("fileupload-progress"), token);
                    this.logger.LogInformation("The images upload for listing request {listingRequestId} is complete.", residentialListingRequestID);
                }
                catch
                {
                    isUploadInProgress = false;
                }
            }
        }

        private void DeleteResources()
        {
            this.uploaderClient.WaitUntilScriptIsComplete(script: "return document.readyState", expectedCompletedResult: "complete");
            var photos = this.uploaderClient.FindElement(By.Id("sortable-list")).FindElements(By.TagName("li"));
            if (photos != null && photos.Count > 0)
            {
                for (int i = 1; i < photos.Count; i++)
                {
                    photos[i].FindElement(By.ClassName("photo-checkbox")).Click();
                }

                this.uploaderClient.ScrollToTop();
                this.uploaderClient.ExecuteScript("javascript: deleteSelected()");
                Thread.Sleep(500);
                this.uploaderClient.ExecuteScript("jQuery('.ui-dialog.ui-widget > #delete-photos-dialog').parent().find('div:eq(3)').find('button:eq(0)').click()");
            }
        }

        private void FillCompletionInformation(ResidentialListingRequest listing)
        {
            var miscComplete = string.Empty;
            if (listing.MiscellaneousDesc != null)
            {
                miscComplete = listing.MiscellaneousDesc.Replace("UNDCN", string.Empty);
                var doubleComma = miscComplete.IndexOf(",,");
                if (doubleComma != -1)
                {
                    miscComplete = miscComplete.Replace(",,", string.Empty);
                }
            }

            this.uploaderClient.WriteTextbox(By.Name("MISCELANES"), miscComplete, isElementOptional: true); // Miscellaneous
        }

        private void FillBasicInformation()
        {
            const string xpath = "//input[@onclick=\"ignoreSuggestions();\"]";

            try
            {
                var ignoreSuggestionsPath = By.XPath(xpath);
                this.uploaderClient.WaitUntilElementIsDisplayed(ignoreSuggestionsPath);
                var element = this.uploaderClient.FindElement(ignoreSuggestionsPath);
                if (element != null && element.Displayed)
                {
                    this.uploaderClient.ClickOnElement(
                        findBy: ignoreSuggestionsPath,
                        shouldWait: false,
                        waitTime: 0,
                        isElementOptional: false);
                }
                else
                {
                    Thread.Sleep(1000);
                    var checkboxes =
                        this.uploaderClient
                        .FindElements(By.TagName("input"))
                        .Where(x => x.GetAttribute("type") == "checkbox" && x.GetAttribute("value") == "on");

                    if (checkboxes.Any())
                    {
                        var ignoreSuggestions = checkboxes.First();
                        if (ignoreSuggestions.GetAttribute("onclick") == "ignoreSuggestions();")
                        {
                            ignoreSuggestions.Click();
                        }
                    }
                }
            }
            catch
            {
                // Ignoring exception because the fields are optional
            }

            if (this.uploaderClient.UploadInformation.IsNewListing)
            {
                try
                {
                    this.uploaderClient.FindElement(By.Name("save-page")).Click();
                }
                catch
                {
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("main"));
                    this.uploaderClient.SwitchTo("main");
                    this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("workspace"));
                    this.uploaderClient.SwitchTo("workspace");
                    this.uploaderClient.ScrollDown();
                    this.uploaderClient.FindElement(By.Name("save-page")).Click();
                }
            }
            else
            {
                this.uploaderClient.ClickOnElementById("submit-data", isElementOptional: true);
            }
        }

        private void SetLongitudeAndLatitudeValues(ResidentialListingRequest listing)
        {
            if (!string.IsNullOrEmpty(listing.MLSNum))
            {
                this.logger.LogInformation("Skipping configuration of latitude and longitude for listing {address} because it already has an mls number", $"{listing.StreetNum} {listing.StreetName}");
                return;
            }

            this.uploaderClient.WaitUntilElementIsDisplayed(By.LinkText("Re-position Property on the Map"));
            this.uploaderClient.ClickOnElement(
                findBy: By.LinkText("Re-position Property on the Map"),
                shouldWait: false,
                waitTime: 0,
                isElementOptional: false);
            this.uploaderClient.WaitUntilElementIsDisplayed(By.Id("lng"));

            this.uploaderClient.WriteTextbox(By.Id("lat"), listing.Latitude); // Latitude
            this.uploaderClient.WriteTextbox(By.Id("lng"), listing.Longitude); // Longitude
        }

        private void ShowMarkToFieldByNameElement(string elementName)
            => this.uploaderClient.ExecuteScript(script: " var elem = document.createElement('img'); elem.setAttribute('tooltip', 'Requires attention'); elem.setAttribute('src', 'http://www.fancyicons.com/free-icons/221/modern-anti-malware/png/24/security_warning_24.png');elem.setAttribute('height', '24');elem.setAttribute('width', '24'); document.getElementsByName('" + elementName + "')[0].parentNode.appendChild(elem); document.getElementsByName('" + elementName + "')[0].parentNode.setAttribute('style', 'background-color: #ffe3a3;') ");

        private void DeleteOpenHouses()
        {
            Thread.Sleep(2000);
            var isDeleteDone = false;
            while (!isDeleteDone)
            {
                var openHouseElements = this.uploaderClient.FindElements(By.Name("dc"));
                if (openHouseElements != null && openHouseElements.Any())
                {
                    var element = openHouseElements
                        .FirstOrDefault()?
                        .FindElements(By.TagName("a"))
                        .FirstOrDefault(x => x.GetAttribute("href").Contains("delTour("));

                    if (element != null)
                    {
                        element.Click();
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        isDeleteDone = true;
                    }
                }
                else
                {
                    isDeleteDone = true;
                }
            }

            Thread.Sleep(2000);
        }

        [SuppressMessage("SonarLint", "S2589", Justification = "Ignored to avoid potential breaking of the method")]
        private void AddOpenHouses(ResidentialListingRequest listing)
        {
            const string tabName = "Add Open House";
            Thread.Sleep(1000);
            var openHouseType = "O";
            var maxIterations = 4;
            var iterationCount = 0;
            var sortedOpenHouses = listing.OpenHouse.OrderBy(openHouse => openHouse.Date).ToList();
            foreach (var openHouse in sortedOpenHouses)
            {
                if (iterationCount >= maxIterations)
                {
                    break;
                }

                this.uploaderClient.ClickOnElementById(elementId: "addTourLink");
                Thread.Sleep(1000);

                this.uploaderClient.SwitchToLast();
                Thread.Sleep(1000);
                this.uploaderClient.ClickOnElementById(elementId: $"type{openHouseType}");

                this.uploaderClient.WriteTextbox(By.Id("dayOfEvent"), entry: openHouse.Date); // Date
                this.uploaderClient.SetSelect(By.Id("startTimeHour"), value: OpenHouseExtensions.GetOpenHouseHours(openHouse.StartTime.To12Format()), fieldLabel: "Start Time Hour", tabName);
                this.uploaderClient.SetSelect(By.Id("startTimeMin"), value: openHouse.StartTime.Minutes.ToString(), fieldLabel: "Start Time Min", tabName);
                var fromTimeTT = openHouse.StartTime.Hours >= 12 ? "PM" : "AM";
                this.uploaderClient.SetSelect(By.Id("startTimeAmPm"), value: fromTimeTT, fieldLabel: "Start Time AM/PM", fieldSection: tabName);

                this.uploaderClient.SetSelect(By.Id("stopTimeHour"), value: OpenHouseExtensions.GetOpenHouseHours(openHouse.EndTime.To12Format()), fieldLabel: "End Time Hour", tabName);
                this.uploaderClient.SetSelect(By.Id("stopTimeMin"), value: openHouse.EndTime.Minutes.ToString(), fieldLabel: "End Time Min", tabName);
                var endTimeTT = openHouse.EndTime.Hours >= 12 ? "PM" : "AM";
                this.uploaderClient.SetSelect(By.Id("stopTimeAmPm"), value: endTimeTT, fieldLabel: "End Time AM/PM", fieldSection: tabName);

                this.uploaderClient.ClickOnElementById(elementId: $"lunch{openHouse.Lunch}");

                this.uploaderClient.ClickOnElementById(elementId: $"refreshments{openHouse.Refreshments}");
                Thread.Sleep(2000);

                this.uploaderClient.ExecuteScript(script: "jQuery('.button.Save').click();");
                Thread.Sleep(2000);

                var window = this.uploaderClient.WindowHandles.FirstOrDefault();
                this.uploaderClient.SwitchTo().Window(windowName: window);
                iterationCount++;
            }

            Thread.Sleep(1000);
        }
    }
}
