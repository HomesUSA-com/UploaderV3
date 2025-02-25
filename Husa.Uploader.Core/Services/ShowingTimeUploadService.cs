namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Api.Contracts.Models.ShowingTime;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums.ShowingTime;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;

    public class ShowingTimeUploadService : IShowingTimeUploadService
    {
        private const string UrlBase = "https://apptcenter.showingtime.com";
        private readonly ShowingTimeSettings settings;
        private readonly IUploaderClient uploaderClient;
        private readonly ILogger<ShowingTimeUploadService> logger;

        public ShowingTimeUploadService(
            IUploaderClient uploaderClient,
            IOptions<ApplicationOptions> options,
            ILogger<ShowingTimeUploadService> logger)
        {
            this.settings = options?.Value.ShowingTime ?? throw new ArgumentNullException(nameof(options));
            this.uploaderClient = uploaderClient ?? throw new ArgumentNullException(nameof(uploaderClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarketCode CurrentMarket { get; private set; }

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.uploaderClient.CloseDriver();
        }

        public Task<LoginResult> Login(CancellationToken cancellationToken = default) => Task.Factory.StartNew(
            () =>
        {
            this.logger.LogDebug("Starting login");
            this.uploaderClient.NavigateToUrl(this.AbsoluteUrl("Account/Login"));
            this.uploaderClient.WaitForElementToBeVisible(By.ClassName("ui-accordion-header"), TimeSpan.FromMilliseconds(300));
            this.uploaderClient.WriteTextbox(By.Id("UserName"), this.settings.Credentials.Username);
            this.uploaderClient.WriteTextbox(By.Id("Password"), this.settings.Credentials.Password);
            Thread.Sleep(200);
            this.uploaderClient.ClickOnElement(By.XPath("//button[@class='button login']"));
            this.uploaderClient.ClickOnElement(By.XPath("//div[@class='ui-dialog-buttonset']/button[2]"));
            this.logger.LogDebug("login");

            return LoginResult.Logged;
        },
            cancellationToken);

        public UploadResult Logout()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToListing(string listingId, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.NavigateToUrl(this.AbsoluteUrl($"Listings/Management/{listingId}"));
            Thread.Sleep(500);
        },
            cancellationToken);

        public Task SetAppointmentCenter(CancellationToken cancellationToken) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.ClickOnElementById("Allow3rdPartyAppts_Yes");
            this.uploaderClient.ClickOnElementById("AllowOnlineShowingRequests_Yes");
        },
            cancellationToken);

        public Task SetAppointmentSettings(AppointmentType appointmentType, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
                () =>
            {
                this.uploaderClient.WaitUntilElementExists(By.Id("appointmentTypeSelection"));

                this.uploaderClient.ExecuteScript(
                    "document.querySelector('#appointmentTypeSelection > div > a').click()");
                switch (appointmentType)
                {
                    case AppointmentType.AppointmentRequired:
                        this.uploaderClient.ExecuteScript(
                            "document.querySelector(`a[data-dk-dropdown-value='APPOINTMENT_REQUIRED_ANY']`).click()");
                        break;
                    case AppointmentType.GoAndShow:
                        this.uploaderClient.ExecuteScript(
                            "document.querySelector(`a[data-dk-dropdown-value='GO_AND_SHOW']`).click()");
                        break;
                    case AppointmentType.CourtesyCall:
                        this.uploaderClient.ExecuteScript(
                            "document.querySelector(`a[data-dk-dropdown-value='COURTESY_CALL']`).click()");
                        break;
                    default:
                        this.uploaderClient.ExecuteScript(
                            "document.querySelector('#appointmentTypeSelection > div > a').click()");
                        break;
                }

                if (appointmentType == AppointmentType.AppointmentRequired)
                {
                    this.uploaderClient.WaitForElementToBeVisible(By.Id("isAgentAccompany_No"), TimeSpan.FromMilliseconds(600));
                    this.uploaderClient.ClickOnElementById("isAgentAccompany_No");
                }

                this.uploaderClient.ClickOnElementById("SendFeedback_Yes");
                this.uploaderClient.ClickOnElementById("IsOccupied_No");
            },
                cancellationToken);

        public Task SetAppointmentRestrictions(AppointmentRestrictionsInfo info, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
                () =>
        {
            var allowSameDayRequest = info.LeadTime ? "Yes" : "No";
            var allowAppraisals = info.AllowAppraisals ? "Yes" : "No";
            var allowInspections = info.AllowInspectionsAndWalkThroughs ? "Yes" : "No";
            this.uploaderClient.ClickOnElementById($"AllowAppraisals_{allowAppraisals}");
            this.uploaderClient.ClickOnElementById($"AllowInspections_{allowInspections}");
            this.uploaderClient.ClickOnElementById($"AllowSameDayRequests_{allowSameDayRequest}");
            this.uploaderClient.SetSelect(By.Id("RequiredLeadTime"), info.RequiredTimeHours);
            this.uploaderClient.SetSelect(By.Id("SuggestedLeadTime"), info.SuggestedTimeHours);
        },
                cancellationToken);

        public Task SetAccessInformation(AccessInformationInfo info, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.SetSelect(By.Id("AccessType"), (int)info.AccessMethod.Value);
            switch (info.AccessMethod.Value)
            {
                case AccessMethod.CodeBox:
                    this.uploaderClient.WriteTextbox(By.Id("LockboxSerialNumber"), info.Serial);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxNotes"), info.Location);
                    break;
                case AccessMethod.Combination:
                    this.uploaderClient.WriteTextbox(By.Id("LockboxNotes"), info.Combination);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxCbsCode"), info.Location);
                    break;
                case AccessMethod.Keypad:
                    this.uploaderClient.WriteTextbox(By.Id("LockboxNotes"), info.Location);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxCbsCode"), info.Code);
                    break;
                case AccessMethod.MasterLockBluetooth:
                    this.uploaderClient.WriteTextbox(By.Id("LockboxSerialNumber"), info.DeviceId);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxNotes"), info.Location);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxCbsCode"), info.SharingCode);
                    break;
                case AccessMethod.SentriLock:
                case AccessMethod.SupraiBox:
                    this.uploaderClient.WriteTextbox(By.Id("LockboxSerialNumber"), info.Serial);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxNotes"), info.Location);
                    this.uploaderClient.WriteTextbox(By.Id("LockboxCbsCode"), info.CbsCode);
                    break;
            }

            var provideAlarm = info.ProvideAlarmDetails ? "Yes" : "No";
            this.uploaderClient.ClickOnElementById($"ProvideAlarm{provideAlarm}");
            if (info.ProvideAlarmDetails)
            {
                this.uploaderClient.WriteTextbox(By.Id("DisarmCode"), info.AlarmDisarmCode);
                this.uploaderClient.WriteTextbox(By.Id("ArmCode"), info.AlarmArmCode);
                this.uploaderClient.WriteTextbox(By.Id("PassCode"), info.AlarmPasscode);
                this.uploaderClient.WriteTextbox(By.Id("AlarmNotes"), info.AlarmNotes);
            }
        },
            cancellationToken);

        public Task SetAdditionalInstructions(AdditionalInstructionsInfo info, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.WriteTextbox(By.Id("ShowingInstructionsToApptStaff"), info.NotesForApptStaff);
            this.uploaderClient.WriteTextbox(By.Id("ShowingInstructionsToShowingAgent"), info.NotesForShowingAgent);
        },
            cancellationToken);

        public Task<bool> AddExistentContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.ExecuteScript("document.querySelector('#addListingContact').click()");
            Task.Delay(1000).Wait();
            var searchText = string.IsNullOrEmpty(contact.Email?.Trim()) ?
                $"{contact.FirstName} {contact.LastName}".Trim() : contact.Email;
            this.uploaderClient.WriteTextbox(By.Id("agentClientsFilter"), searchText);
            this.uploaderClient.ExecuteScript("document.querySelector('#agentClientSearch').click()");

            this.WaitUntilUnblockUI(By.XPath("//div[@class='blockUI blockMsg blockPage']"));

            this.uploaderClient.WaitForElementToBeVisible(By.Id("agentClientsTable"), TimeSpan.FromSeconds(3));
            var element = this.uploaderClient.FindElement(
                By.XPath("//table[@id='agentClientsTable']/tbody/tr[1]"), isElementOptional: false);

            if (element.Text.Contains("No matching records found"))
            {
                this.uploaderClient.ExecuteScript(
                    "document.querySelectorAll('.ui-dialog-buttonset button')[1].click()");
                return false;
            }

            element.Click();
            this.uploaderClient.WaitForElementToBeVisible(By.Id("agentClientContactDetails"), TimeSpan.FromSeconds(3));
            this.uploaderClient.ClickOnElementById("ACIsOwner_Yes");
            this.uploaderClient.ClickOnElementById("ACIsOccupant_No");
            this.uploaderClient.ClickOnElementById("saveContact");
            return true;
        },
            cancellationToken);

        public Task<bool> AddNewContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            var xpath = "//div[@class='ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable']";
            this.uploaderClient.ExecuteScript("document.querySelector(`#addListingContact`).click()");
            this.uploaderClient.WaitForElementToBeVisible(By.XPath(xpath), TimeSpan.FromSeconds(4));
            this.uploaderClient.ExecuteScript("document.querySelectorAll(`.ui-dialog .ui-tabs-nav li > a`)[1].click()");

            this.WaitUntilUnblockUI(By.XPath("//div[@class='blockUI blockMsg blockPage']"));

            this.uploaderClient.WriteTextbox(By.Id("FirstName"), contact.FirstName);
            this.uploaderClient.WriteTextbox(By.Id("LastName"), contact.LastName);
            this.uploaderClient.ExecuteScript("document.querySelector(`#IsOwner_Yes`).click()");
            this.uploaderClient.ExecuteScript("document.querySelector(`#IsOccupant_No`).click()");

            if (!string.IsNullOrEmpty(contact.MobilePhone?.Trim()))
            {
                var phone = long.Parse(contact.MobilePhone).ToString("(000) 000-0000");
                this.uploaderClient.SetSelect(By.Id("PhoneDescriptions_0_"), "2");
                this.uploaderClient.WriteTextbox(By.Id("PhoneDescriptions_0_"), phone);
            }

            if (!string.IsNullOrEmpty(contact.OfficePhone?.Trim()))
            {
                var phone = long.Parse(contact.OfficePhone).ToString("(000) 000-0000");
                this.uploaderClient.SetSelect(By.Id("PhoneDescriptions_1_"), "3");
                this.uploaderClient.WriteTextbox(By.Id("Phones_1_"), phone);
            }

            if (!string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                this.uploaderClient.WriteTextbox(By.Id("Email"), contact.Email.Trim());
            }

            this.uploaderClient.ExecuteScript("document.querySelector(`#saveContact`).click()");

            return true;
        },
            cancellationToken);

        public Task EditContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.ExecuteScript(@"document.querySelector(`button[title='Edit Contact']`).click()");
            this.uploaderClient.WaitForElementToBeVisible(
                By.XPath("//div[@class='ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable']"),
                TimeSpan.FromSeconds(3));

            if (string.IsNullOrEmpty(contact.MobilePhone?.Trim()))
            {
                this.uploaderClient.SetSelect(By.Name("PhoneDescriptions[0]"), "-1");
                this.uploaderClient.WriteTextbox(By.Name("Phones[0]"), string.Empty);
            }
            else
            {
                var phone = long.Parse(contact.MobilePhone.Trim()).ToString("(000) 000-0000");
                this.uploaderClient.SetSelect(By.Name("PhoneDescriptions[0]"), "2");
                this.uploaderClient.WriteTextbox(By.Name("Phones[0]"), phone);
            }

            if (string.IsNullOrEmpty(contact.OfficePhone?.Trim()))
            {
                this.uploaderClient.SetSelect(By.Name("PhoneDescriptions[1]"), "-1");
                this.uploaderClient.WriteTextbox(By.Name("Phones[1]"), string.Empty);
            }
            else
            {
                var phone = long.Parse(contact.OfficePhone.Trim()).ToString("(000) 000-0000");
                this.uploaderClient.SetSelect(By.Name("PhoneDescriptions[1]"), "3");
                this.uploaderClient.WriteTextbox(By.Name("Phones[1]"), phone);
            }

            if (string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                this.uploaderClient.WriteTextbox(By.Id("SABOR--966512Email"), string.Empty);
            }
            else
            {
                this.uploaderClient.WriteTextbox(By.Id("SABOR--966512Email"), contact.Email.Trim());
            }

            var saveBtn = this.uploaderClient.FindElementById("contactEditSaveContact");

            if (string.IsNullOrEmpty(saveBtn.GetAttribute("disabled")))
            {
                this.uploaderClient.ClickOnElementById("contactEditSaveContact");
            }
            else
            {
                this.uploaderClient.ClickOnElementById("contactEditCancel");
            }
        },
            cancellationToken);

        public Task SetContactConfirmSection(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            var confirmSection = this.uploaderClient.FindElement(By.ClassName("confirm-section"), isElementOptional: true);
            if (confirmSection?.GetCssValue("display") != "none")
            {
                if (contact.ConfirmAppointmentsByEmail.Value
                    || contact.ConfirmAppointmentsByMobilePhone.Value
                    || contact.ConfirmAppointmentsByOfficePhone.Value
                    || contact.ConfirmAppointmentsByText.Value)
                {
                    this.uploaderClient.ExecuteScript(
                            $"document.querySelector('#ConfirmMethod_SendConfirmation_{position}').click()");
                }

                if (contact.ConfirmAppointmentsByText.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#confirmSms_{position}').click()");
                }

                if (contact.ConfirmAppointmentsByEmail.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#confirmEmail_{position}').click()");
                }

                if (contact.ConfirmAppointmentsByMobilePhone.Value || contact.ConfirmAppointmentsByOfficePhone.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#confirmPhone_{position}').click()");
                    var caller = contact.ConfirmAppointmentCallerByOfficePhone ?? contact.ConfirmAppointmentCallerByMobilePhone;
                    this.uploaderClient.SetSelect(By.Id($"phoneRequestChoice_{position}"), caller.Equals(ConfirmAppointmentCaller.AutoCall));
                }

                if (contact.SendOnFYIByEmail.Value || contact.SendOnFYIByText.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#ConfirmMethod_FYI_{position}').click()");
                }

                if (contact.SendOnFYIByText.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#fyiSms_{position}').click()");
                }

                if (contact.SendOnFYIByEmail.Value)
                {
                    this.uploaderClient.ExecuteScript(
                        $"document.querySelector('#fyiEmail_{position}').click()");
                }
            }
        },
            cancellationToken);

        public Task SetContactNotificationSection(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.ExecuteScript(
                    $"['#notifySms_{position}', '#notifyEmail_{position}', '#notifyPhone_{position}']"
                    + ".map(x => document.querySelector(x).checked = false);");

            if (contact.NotifyAppointmentsChangesByText.Value)
            {
                this.uploaderClient.ExecuteScript(
                    $"document.querySelector('#notifySms_{position}').click()");
            }

            if (contact.NotifyAppointmentChangesByEmail.Value)
            {
                this.uploaderClient.ExecuteScript(
                    $"document.querySelector('#notifyEmail_{position}').click()");
            }

            if (contact.NotifyAppointmentChangesByOfficePhone.Value || contact.NotifyAppointmentChangesByMobilePhone.Value)
            {
                this.uploaderClient.ExecuteScript(
                    $"document.querySelector('#notifyPhone_{position}').click()");
                var callType = contact.AppointmentChangesNotificationsOptionsOfficePhone ?? contact.AppointmentChangesNotificationsOptionsMobilePhone;
                this.uploaderClient.SetSelect(By.Id($"CallToCancelConfirm{position}"), callType.Value.ToStringFromEnumMember());
            }
        },
            cancellationToken);

        public async Task SetContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default)
        {
            var element = this.uploaderClient.FindElement(By.Id($"contact{position}_row"), isElementOptional: true);

            if (element is null)
            {
                var added = await this.AddExistentContact(contact, position, cancellationToken)
                    || await this.AddNewContact(contact, position, cancellationToken);

                element = added ? this.uploaderClient.FindElement(By.Id($"contact{position}_row"), isElementOptional: true) : null;
            }

            if (element is not null)
            {
                await this.EditContact(contact, position, cancellationToken);
                await this.SetContactConfirmSection(contact, position, cancellationToken);
                await this.SetContactNotificationSection(contact, position, cancellationToken);
            }
        }

        public async Task SetContacts(IEnumerable<ContactDetailInfo> contacts, CancellationToken cancellationToken)
        {
            for (int index = 0; index < (contacts?.Count() ?? 0); index++)
            {
                await this.SetContact(contacts.ElementAt(index), index + 1, cancellationToken);
            }
        }

        public async Task<UploadResult> Upload(ResidentialListingRequest request, bool logIn = true, CancellationToken cancellationToken = default)
        {
            if (request?.ShowingTime is null || request.MLSNum is null
                || (logIn && (await this.Login(cancellationToken)) != LoginResult.Logged)
                || !await this.FindListing(request.MLSNum, cancellationToken))
            {
                return UploadResult.Failure;
            }

            await this.SetAppointmentCenter(cancellationToken);
            await this.SetAppointmentSettings(request.ShowingTime.AppointmentType.Value, cancellationToken);
            await this.SetAppointmentRestrictions(request.ShowingTime.AppointmentRestrictions, cancellationToken);
            await this.SetAccessInformation(request.ShowingTime.AccessInformation, cancellationToken);
            await this.SetAdditionalInstructions(request.ShowingTime.AdditionalInstructions, cancellationToken);
            await this.SetContacts(request.ShowingTime.Contacts, cancellationToken);

            return UploadResult.Success;
        }

        public Task<bool> FindListing(string mlsNumber, CancellationToken cancellationToken = default) =>
            Task.Factory.StartNew(
            () =>
        {
            this.uploaderClient.NavigateToUrl(this.AbsoluteUrl("Select/Listing"));
            this.uploaderClient.WaitForElementToBeVisible(By.Id("listings_table"), TimeSpan.FromSeconds(3));
            this.uploaderClient.WriteTextbox(By.Id("q"), mlsNumber);
            this.uploaderClient.ClickOnElementById("search");
            Task.Delay(3000).Wait();
            this.uploaderClient.WaitForElementToBeVisible(By.Id("listings_table"), TimeSpan.FromSeconds(3));
            var firstRow = this.uploaderClient.FindElement(By.XPath("//table[@id='listings_table']/tbody/tr[1]"), isElementOptional: true);
            this.uploaderClient.ExecuteScript("document.querySelector('#listings_table > tbody > tr')?.click()");
            return firstRow is not null;
        },
            cancellationToken);

        private string AbsoluteUrl(string path) => $"{UrlBase}/{path}";

        private void WaitUntilUnblockUI(By by)
        {
            try
            {
                this.uploaderClient.WaitUntilElementDisappears(by);
                Task.Delay(1000);
            }
            catch
            {
                // Nothing to do
            }
        }
    }
}
