namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ShowingTime;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums.ShowingTime;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.ShowingTime;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Core.Services.ShowingTime;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;
    using OpenQA.Selenium;

    public class ShowingTimeUploadService : IShowingTimeUploadService
    {
        private const string AlertClass = "ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable";
        private readonly IMarketUploadService marketUploadService;
        private readonly ILogger logger;
        private readonly string agentSelectorValue;

        public ShowingTimeUploadService(
            IMarketUploadService marketUploadService,
            string agentSelectorValue,
            ILogger logger)
        {
            this.marketUploadService = marketUploadService ?? throw new ArgumentNullException(nameof(marketUploadService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.agentSelectorValue = agentSelectorValue;
        }

        public MarketCode CurrentMarket => this.marketUploadService.CurrentMarket;
        private IUploaderClient UploaderClient => this.marketUploadService.UploaderClient;

        public void CancelOperation()
        {
            this.logger.LogInformation("Stopping driver at the request of the current user");
            this.UploaderClient.CloseDriver();
        }

        public Task<bool> FindListingOnMls(string mlsNumber, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
            {
                this.UploaderClient.ClickOnElement(By.XPath("//a/span[text()='Input']"));
                this.UploaderClient.WaitForElementToBeVisible(By.Id("m_dlInputList"), TimeSpan.FromSeconds(3));
                this.UploaderClient.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), mlsNumber);
                this.UploaderClient.ClickOnElementById("m_lvInputUISections_ctrl0_lbQuickEdit");
                Task.Delay(3000).Wait(cancellationToken);
                return this.UploaderClient.FindElement(By.XPath("//span[text()='Modify Property']"), isElementOptional: true) != null;
            },
            cancellationToken);

        public async Task<bool> GetInShowingTimeSite(Guid companyId, string mlsNumber, CancellationToken cancellationToken = default)
        {
            var success = !string.IsNullOrEmpty(mlsNumber);
            this.logger.LogDebug("Mls number provided: {MlsNumber}", mlsNumber);
            success = success && (await this.marketUploadService.Login(companyId))
                .Equals(LoginResult.Logged);
            this.logger.LogDebug("Mls Login success: {LoginSuccess}", success);
            this.UploaderClient.ClickOnElement(
                By.Id("ctl02_ctl10_HyperLink2"), shouldWait: true, waitTime: 1000, isElementOptional: true);
            success = success && await this.FindListingOnMls(mlsNumber, cancellationToken);
            this.logger.LogDebug("Mls Listing located: {FindListingSuccess}", success);
            if (success)
            {
                this.logger.LogDebug("Accessing...");
                this.UploaderClient.ClickOnElementById("m_oThirdPartyLinks_m_lvThirdPartLinks_ctrl3_m_lbtnThirdPartyLink");
                await Task.Delay(3000, cancellationToken);
                this.UploaderClient.SwitchTo().Window(this.UploaderClient.WindowHandles[1]);
                this.UploaderClient.ClickOnElement(
                    By.XPath("//button/span[text()='Remind Me Later']"),
                    shouldWait: true,
                    waitTime: 1000,
                    isElementOptional: true);
            }

            return success;
        }

        public async Task<int> DeleteDuplicateClients(Guid companyId, string mlsNumber, CancellationToken cancellationToken = default)
        {
            if (!await this.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken))
            {
                return 0;
            }

            this.UploaderClient.ClickOnElement(By.XPath("//a[text()='Contacts']"));
            await Task.Delay(2000, cancellationToken);
            this.UploaderClient.SetSelect(By.ClassName("selectList"), this.agentSelectorValue);
            this.UploaderClient.ClickOnElement(By.XPath("//a[text()='Sellers']"));
            var pages = int.Parse(
                this.UploaderClient.FindElement(
                    By.XPath("//div[@id='clientSellersTable_paginate']/span/a[last()]"),
                    shouldWait: true)
                ?.Text ?? "0");

            var clients = Enumerable.Range(0, pages)
                .Select(x => Array.Empty<ShowingTimeClientTableItem>())
                .Aggregate((acc, x) =>
                {
                    var table = this.UploaderClient.FindElementById("clientSellersTable");
                    var rows = table.FindElements(By.TagName("tr"));
                    var items = rows.Skip(1).Select(x =>
                    {
                        var columns = x.FindElements(By.TagName("td"));
                        return new ShowingTimeClientTableItem
                        {
                            FirstName = columns[0].Text,
                            LastName = columns[1].Text,
                            Phone = columns[3].Text,
                        };
                    }).ToArray();
                    this.UploaderClient.ClickOnElement(
                        By.Id("clientSellersTable_next"),
                        shouldWait: true,
                        waitTime: 3000,
                        isElementOptional: true);

                    return
                    [
                        .. acc,
                        ..items
                    ];
                });

            var duplicates = clients
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ThenBy(c => c.Phone)
                .GroupBy(c => new { c.FirstName, c.LastName, c.Phone })
                .Where(c => c.Count() > 1)
                .Select(c => new ShowingTimeClientTableItem
                {
                    FirstName = c.Key.FirstName,
                    LastName = c.Key.LastName,
                    Phone = c.Key.Phone,
                });

            var forSearch = duplicates
                .GroupBy(x => $"{x.FirstName} {x.LastName}")
                .ToDictionary(a => a.Key, b => b.Select(x => x.Phone).ToList());

            foreach (var (searchText, phones) in forSearch)
            {
                var rowIndex = 0;
                var phonesToKeep = new List<string>();
                this.UploaderClient.WriteTextbox(By.Id("tableFilter"), searchText);
                await Task.Delay(1000, cancellationToken);
                var table = this.UploaderClient.FindElementById("clientSellersTable");
                var rows = table.FindElements(By.TagName("tr")).Skip(1);
                var rowsTotal = rows.Count();

                while (rowIndex < rowsTotal)
                {
                    foreach (var row in rows)
                    {
                        var columns = row.FindElements(By.TagName("td"));
                        var phone = columns[3].Text;
                        var listings = columns[4].Text.Trim();
                        if (!phonesToKeep.Contains(phone))
                        {
                            phonesToKeep.Add(phone);
                        }
                        else if (phones.Contains(phone) && string.IsNullOrEmpty(listings))
                        {
                            columns[6].FindElement(By.TagName("a")).Click();
                            await Task.Delay(3000, cancellationToken);
                            this.UploaderClient.ClickOnElement(
                                By.Id("deleteButton"),
                                shouldWait: true,
                                waitTime: 3000,
                                isElementOptional: false);
                            await Task.Delay(3000, cancellationToken);
                            var confirmButton = this.UploaderClient.FindElement(
                                By.XPath("//button/span[text()='Yes']"),
                                shouldWait: true,
                                isElementOptional: false);
                            confirmButton.Click();
                            await Task.Delay(3000, cancellationToken);
                            table = this.UploaderClient.FindElementById("clientSellersTable");
                            rows = table.FindElements(By.TagName("tr")).Skip(1 + rowIndex);
                            rowsTotal = rows.Count();
                            break;
                        }

                        rowIndex += 1;
                    }
                }
            }

            return duplicates.Count();
        }

        public Task SetAppointmentCenter(AppointmentSettingsResponse info, CancellationToken cancellationToken) =>
            Task.Run(
            () =>
        {
            var allowThirdPartyApps = info.AllowApptCenterTakeAppts ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"Allow3rdPartyAppts_{allowThirdPartyApps}");

            var allowOnlineShowingRequests = info.AllowShowingAgentsToRequest ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"AllowOnlineShowingRequests_{allowOnlineShowingRequests}");

            var appointmentPresentationType = (info.AppointmentPresentationType ?? default).ToStringFromEnumMember();
            this.UploaderClient.SetSelect(
                By.Id("AppointmentPresentationType"),
                appointmentPresentationType,
                isElementOptional: false);
        },
            cancellationToken);

        public Task SetAppointmentSettings(AppointmentSettingsResponse info, CancellationToken cancellationToken = default) =>
            Task.Run(
                () =>
            {
                this.UploaderClient.WaitUntilElementExists(By.Id("appointmentTypeSelection"));

                this.SetAppointmentType(info);

                var sendFeedback = info.IsFeedbackRequested ? "Yes" : "No";
                this.UploaderClient.ClickOnElementById($"SendFeedback_{sendFeedback}");

                var isOccupied = info.IsPropertyOccupied ? "Yes" : "No";
                this.UploaderClient.ClickOnElementById($"IsOccupied_{isOccupied}");
                if (info.IsPropertyOccupied)
                {
                    this.OnAlertClickOk();
                }

                var officeTemplates = info.FeedbackTemplate?.GetEnumDescription();
                var officeTemplatesOption = info.FeedbackTemplate is null ? null : this.UploaderClient.FindElement(
                    By.XPath($"//select[@id='ListingFeedbackTemplateId']/optgroup[@label='{officeTemplates}']/option[1]"),
                    isElementOptional: true);
                if (officeTemplatesOption != null)
                {
                    var officeTemplatesValue = officeTemplatesOption.GetAttribute("value");
                    this.UploaderClient.SetSelect(By.Id("ListingFeedbackTemplateId"), officeTemplatesValue);
                }

                var staffLanguage = info.RequiredStaffLanguage?.ToStringFromEnumMember();
                if (staffLanguage is not null)
                {
                    this.UploaderClient.SetSelect(By.Id("RequiredStaffLanguage"), staffLanguage);
                }
            },
                cancellationToken);

        public Task SetAppointmentRestrictions(AppointmentRestrictionsResponse info, CancellationToken cancellationToken = default) =>
            Task.Run(
                () =>
        {
            var leadTime = info.AdvancedNotice?.Equals(AdvancedNotice.LeadTime) ?? false;
            var advancedNoticeText = info.AdvancedNotice?.ToStringFromEnumMember().ToLower();
            var allowSameDayRequest = bool.Parse(advancedNoticeText ?? "false") ? "Yes" : "No";
            this.UploaderClient.ClickOnElement(By.Id($"AllowSameDayRequests_{allowSameDayRequest}"));

            var allowAppraisals = info.AllowAppraisals ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"AllowAppraisals_{allowAppraisals}");

            var allowInspections = info.AllowInspectionsAndWalkThroughs ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"AllowInspections_{allowInspections}");

            var allowRealTime = info.AllowRealtimeAvailabilityForBrokers ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"PublicListingAvailabilityEnabled_{allowRealTime}");

            var minShowingWindow = (info.MinShowingWindowShowings ?? default).ToStringFromEnumMember();
            this.UploaderClient.SetSelect(By.Id("MinShowingWindowShowings"), minShowingWindow);

            var maxShowingWindow = (info.MaxShowingWindowShowings ?? default).ToStringFromEnumMember();
            this.UploaderClient.SetSelect(By.Id("MaxShowingWindowShowings"), maxShowingWindow);

            var overlaping = (info.OverlappingAppointmentMode ?? default).ToStringFromEnumMember();
            this.UploaderClient.SetSelect(By.Id("OverlappingAppointmentMode"), overlaping);

            var bufferTime = (info.BufferTimeBetweenAppointments ?? default).ToStringFromEnumMember();
            this.UploaderClient.SetSelect(By.Id("AppointmentNoOverlapBufferMinutes"), bufferTime);

            if (leadTime)
            {
                var requiredTime = (info.RequiredTimeHours ?? default).ToStringFromEnumMember();
                this.UploaderClient.SetSelect(By.Id("RequiredLeadTime"), requiredTime);

                var suggestedTime = (info.SuggestedTimeHours ?? default).ToStringFromEnumMember();
                this.UploaderClient.SetSelect(By.Id("SuggestedLeadTime"), suggestedTime);
            }
        },
                cancellationToken);

        public Task SetAccessInformation(AccessInformationResponse info, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            var lockboxNotesId = "LockboxNotes";
            var lockboxCbsCodeId = "LockboxCbsCode";
            var lockboxSerialNumberId = "LockboxSerialNumber";
            this.UploaderClient.WriteTextbox(By.Id("GateCode"), info.GateCode);
            this.UploaderClient.WriteTextbox(By.Id("AccessNotes"), info.AccessNotes);
            this.UploaderClient.SetSelect(By.Id("AccessType"), (int)info.AccessMethod.Value);

            switch (info.AccessMethod.Value)
            {
                case AccessMethod.CodeBox:
                    this.UploaderClient.WriteTextbox(By.Id(lockboxSerialNumberId), info.Serial);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxNotesId), info.Location);
                    break;
                case AccessMethod.Combination:
                    this.UploaderClient.WriteTextbox(By.Id(lockboxNotesId), info.Combination);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxCbsCodeId), info.Location);
                    break;
                case AccessMethod.Keypad:
                    this.UploaderClient.WriteTextbox(By.Id(lockboxNotesId), info.Location);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxCbsCodeId), info.Code);
                    break;
                case AccessMethod.MasterLockBluetooth:
                    this.UploaderClient.WriteTextbox(By.Id(lockboxSerialNumberId), info.DeviceId);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxNotesId), info.Location);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxCbsCodeId), info.SharingCode);
                    break;
                case AccessMethod.SentriLock:
                case AccessMethod.SupraiBox:
                    this.UploaderClient.WriteTextbox(By.Id(lockboxSerialNumberId), info.Serial);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxNotesId), info.Location);
                    this.UploaderClient.WriteTextbox(By.Id(lockboxCbsCodeId), info.CbsCode);
                    break;
            }

            var provideAlarm = info.ProvideAlarmDetails ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"ProvideAlarm{provideAlarm}");
            if (info.ProvideAlarmDetails)
            {
                this.UploaderClient.WriteTextbox(By.Id("DisarmCode"), info.AlarmDisarmCode);
                this.UploaderClient.WriteTextbox(By.Id("ArmCode"), info.AlarmArmCode);
                this.UploaderClient.WriteTextbox(By.Id("PassCode"), info.AlarmPasscode);
                this.UploaderClient.WriteTextbox(By.Id("AlarmNotes"), info.AlarmNotes);
            }

            var manageKeySets = info.HasManageKeySets ? "Yes" : "No";
            this.UploaderClient.ClickOnElementById($"KeysAvailable{manageKeySets}");
        },
            cancellationToken);

        public Task SetAdditionalInstructions(AdditionalInstructionsResponse info, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.WriteTextbox(By.Id("ShowingInstructionsToApptStaff"), info.NotesForApptStaff);
            this.UploaderClient.WriteTextbox(By.Id("ShowingInstructionsToShowingAgent"), info.NotesForShowingAgent);
        },
            cancellationToken);

        public Task SetDrivingDirections(ResidentialListingRequest request, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.WriteTextbox(By.Id("Directions"), request.Directions);
        },
            cancellationToken);

        public Task<bool> AddExistentContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.ExecuteScript("document.querySelector('#addListingContact').click()");
            Task.Delay(1000, cancellationToken).Wait();
            var searchText = string.IsNullOrEmpty(contact.Email?.Trim()) ?
                $"{contact.FirstName} {contact.LastName}".Trim() : contact.Email;
            this.UploaderClient.WriteTextbox(By.Id("agentClientsFilter"), searchText);
            this.UploaderClient.ExecuteScript("document.querySelector('#agentClientSearch').click()");

            this.WaitUntilUnblockUI(By.XPath("//div[@class='blockUI blockMsg blockPage']"), cancellationToken);

            this.UploaderClient.WaitForElementToBeVisible(By.Id("agentClientsTable"), TimeSpan.FromSeconds(3));
            var element = this.UploaderClient.FindElement(
                By.XPath("//table[@id='agentClientsTable']/tbody/tr[1]"), isElementOptional: false);

            if (element.Text.Contains("No matching records found"))
            {
                this.UploaderClient.ExecuteScript(
                    "document.querySelectorAll('.ui-dialog-buttonset button')[1].click()");
                return false;
            }

            element.Click();
            this.UploaderClient.WaitForElementToBeVisible(By.Id("agentClientContactDetails"), TimeSpan.FromSeconds(3));
            this.UploaderClient.ClickOnElementById("ACIsOwner_Yes");
            this.UploaderClient.ClickOnElementById("ACIsOccupant_No");
            this.UploaderClient.ClickOnElementById("saveContact");
            return true;
        },
            cancellationToken);

        public Task<bool> AddNewContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            var xpath = "//div[@class='ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable']";
            this.UploaderClient.ExecuteScript("document.querySelector(`#addListingContact`).click()");
            this.UploaderClient.WaitForElementToBeVisible(By.XPath(xpath), TimeSpan.FromSeconds(4));
            this.UploaderClient.ExecuteScript("document.querySelectorAll(`.ui-dialog .ui-tabs-nav li > a`)[1].click()");

            this.WaitUntilUnblockUI(By.XPath("//div[@class='blockUI blockMsg blockPage']"), cancellationToken);

            this.UploaderClient.WriteTextbox(By.Id("FirstName"), contact.FirstName);
            this.UploaderClient.WriteTextbox(By.Id("LastName"), contact.LastName);
            this.UploaderClient.ExecuteScript("document.querySelector(`#IsOwner_Yes`).click()");
            this.UploaderClient.ExecuteScript("document.querySelector(`#IsOccupant_No`).click()");

            if (!string.IsNullOrEmpty(contact.MobilePhone?.Trim()))
            {
                var phone = contact.MobilePhone.PhoneFormat();
                this.UploaderClient.SetSelect(By.Id("PhoneDescriptions_0_"), "2");
                this.UploaderClient.WriteTextbox(By.Id("PhoneDescriptions_0_"), phone);
            }

            if (!string.IsNullOrEmpty(contact.OfficePhone?.Trim()))
            {
                var phone = contact.OfficePhone.PhoneFormat();
                this.UploaderClient.SetSelect(By.Id("PhoneDescriptions_1_"), "3");
                this.UploaderClient.WriteTextbox(By.Id("Phones_1_"), phone);
            }

            if (!string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                this.UploaderClient.WriteTextbox(By.Id("Email"), contact.Email.Trim());
            }

            this.UploaderClient.ExecuteScript("document.querySelector(`#saveContact`).click()");

            return true;
        },
            cancellationToken);

        public Task EditContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.ExecuteScript(@"document.querySelector(`button[title='Edit Contact']`).click()");
            this.UploaderClient.WaitForElementToBeVisible(
                By.XPath("//div[@class='ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable']"),
                TimeSpan.FromSeconds(3));

            if (string.IsNullOrEmpty(contact.MobilePhone?.Trim()))
            {
                this.UploaderClient.SetSelect(By.Name("PhoneDescriptions[0]"), "-1");
                this.UploaderClient.WriteTextbox(By.Name("Phones[0]"), string.Empty);
            }
            else
            {
                var phone = long.Parse(contact.MobilePhone.Trim()).ToString("(000) 000-0000");
                this.UploaderClient.SetSelect(By.Name("PhoneDescriptions[0]"), "2");
                this.UploaderClient.WriteTextbox(By.Name("Phones[0]"), phone);
            }

            if (string.IsNullOrEmpty(contact.OfficePhone?.Trim()))
            {
                this.UploaderClient.SetSelect(By.Name("PhoneDescriptions[1]"), "-1");
                this.UploaderClient.WriteTextbox(By.Name("Phones[1]"), string.Empty);
            }
            else
            {
                var phone = long.Parse(contact.OfficePhone.Trim()).ToString("(000) 000-0000");
                this.UploaderClient.SetSelect(By.Name("PhoneDescriptions[1]"), "3");
                this.UploaderClient.WriteTextbox(By.Name("Phones[1]"), phone);
            }

            if (string.IsNullOrEmpty(contact.Email?.Trim()))
            {
                this.UploaderClient.WriteTextbox(By.Id("SABOR--966512Email"), string.Empty);
            }
            else
            {
                this.UploaderClient.WriteTextbox(By.Id("SABOR--966512Email"), contact.Email.Trim());
            }

            var saveBtn = this.UploaderClient.FindElementById("contactEditSaveContact");

            if (string.IsNullOrEmpty(saveBtn.GetAttribute("disabled")))
            {
                this.UploaderClient.ClickOnElementById("contactEditSaveContact");
            }
            else
            {
                this.UploaderClient.ClickOnElementById("contactEditCancel");
            }
        },
            cancellationToken);

        public Task SetContactConfirmSection(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.OnAlertClickOk();
            var confirmSection = this.UploaderClient.FindElement(By.ClassName("confirm-section"), isElementOptional: true);
            if (IsConfirmSectionHidden(confirmSection))
            {
                return;
            }

            if (ShouldSendConfirmation(contact))
            {
                this.UploaderClient.ExecuteScript(
                        $"document.querySelector('#ConfirmMethod_SendConfirmation_{position}').click()");
            }

            this.HandleConfirmationMethods(contact, position);
            this.HandleFYIMethods(contact, position);
        },
            cancellationToken);

        public Task SetContactNotificationSection(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.ExecuteScript(
                    $"['#notifySms_{position}', '#notifyEmail_{position}', '#notifyPhone_{position}']"
                    + ".map(c => document.querySelector(c).checked = false);");

            if (contact.NotifyAppointmentsChangesByText.Value)
            {
                this.UploaderClient.ExecuteScript(
                    $"document.querySelector('#notifySms_{position}').click()");
            }

            if (contact.NotifyAppointmentChangesByEmail.Value)
            {
                this.UploaderClient.ExecuteScript(
                    $"document.querySelector('#notifyEmail_{position}').click()");
            }

            if (contact.NotifyAppointmentChangesByOfficePhone.Value || contact.NotifyAppointmentChangesByMobilePhone.Value)
            {
                this.UploaderClient.ExecuteScript(
                    $"document.querySelector('#notifyPhone_{position}').click()");
                var callType = contact.AppointmentChangesNotificationsOptionsOfficePhone ?? contact.AppointmentChangesNotificationsOptionsMobilePhone;
                this.UploaderClient.SetSelect(By.Id($"CallToCancelConfirm{position}"), callType.Value.ToStringFromEnumMember());
            }
        },
            cancellationToken);

        public async Task SetContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default)
        {
            var element = this.UploaderClient.FindElement(By.Id($"contact{position}_row"), isElementOptional: true);

            if (element is null)
            {
                var added = await this.AddExistentContact(contact, position, cancellationToken)
                    || await this.AddNewContact(contact, position, cancellationToken);

                element = added ? this.UploaderClient.FindElement(By.Id($"contact{position}_row"), isElementOptional: true) : null;
            }

            if (element is not null)
            {
                await this.EditContact(contact, position, cancellationToken);
                await this.SetContactConfirmSection(contact, position, cancellationToken);
                await this.SetContactNotificationSection(contact, position, cancellationToken);
            }
        }

        public async Task SetContacts(IEnumerable<ContactDetailResponse> contacts, CancellationToken cancellationToken)
        {
            for (int index = 0; index < (contacts?.Count() ?? 0); index++)
            {
                await this.SetContact(contacts.ElementAt(index), index + 1, cancellationToken);
            }
        }

        public async Task<UploaderResponse> Upload(ResidentialListingRequest request, CancellationToken cancellationToken = default)
        {
            UploaderResponse response = new UploaderResponse();
            response.UploadInformation = null;

            if (request?.ShowingTime is null
                || !await this.GetInShowingTimeSite(
                    request.CompanyId, request.MLSNum, cancellationToken))
            {
                this.UploaderClient.CloseDriver();
                response.UploadResult = UploadResult.Failure;
                return response;
            }

            await this.SetAppointmentCenter(
                request.ShowingTime.AppointmentSettings, cancellationToken);
            await this.SetAppointmentSettings(
                request.ShowingTime.AppointmentSettings, cancellationToken);
            await this.SetAppointmentRestrictions(
                request.ShowingTime.AppointmentRestrictions, cancellationToken);
            await this.SetAccessInformation(
                request.ShowingTime.AccessInformation, cancellationToken);
            await this.SetAdditionalInstructions(
                request.ShowingTime.AdditionalInstructions, cancellationToken);
            await this.SetDrivingDirections(request, cancellationToken);
            await this.SetContacts(request.ShowingTime.Contacts, cancellationToken);

            response.UploadResult = UploadResult.Success;
            return response;
        }

        public Task<bool> FindListing(string mlsNumber, CancellationToken cancellationToken = default) =>
            Task.Run(
            () =>
        {
            this.UploaderClient.ClickOnElement(By.XPath("//a/span[text()='Input']"));
            this.UploaderClient.WaitForElementToBeVisible(By.Id("m_dlInputList"), TimeSpan.FromSeconds(3));
            this.UploaderClient.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), mlsNumber);
            this.UploaderClient.ClickOnElementById("m_lvInputUISections_ctrl0_lbQuickEdit");
            Task.Delay(3000, cancellationToken).Wait();
            var listingFound = this.UploaderClient.FindElement(By.XPath("//span[text()='Modify Property']"), isElementOptional: true) != null;
            if (listingFound)
            {
                this.UploaderClient.ClickOnElementById("m_oThirdPartyLinks_m_lvThirdPartLinks_ctrl3_m_lbtnThirdPartyLink");
            }

            return listingFound;
        },
            cancellationToken);

        private static bool IsConfirmSectionHidden(IWebElement confirmSection)
        {
            return new string[] { null, "none" }.Contains(confirmSection?.GetCssValue("display"));
        }

        private static bool ShouldSendConfirmation(ContactDetailResponse contact)
        {
            return contact.ConfirmAppointmentsByEmail.Value
                || contact.ConfirmAppointmentsByMobilePhone.Value
                || contact.ConfirmAppointmentsByOfficePhone.Value
                || contact.ConfirmAppointmentsByText.Value;
        }

        private static bool ShouldSendFYI(ContactDetailResponse contact)
        {
            return contact.SendOnFYIByEmail.Value || contact.SendOnFYIByText.Value;
        }

        private void SetAppointmentType(AppointmentSettingsResponse info)
        {
            var appoimentType = info.AppointmentType?.ToStringFromEnumMember();
            if (appoimentType is not null)
            {
                this.UploaderClient.ExecuteScript(
                "document.querySelector('#appointmentTypeSelection > div > a').click()");
                this.UploaderClient.ExecuteScript(
                   $"document.querySelector(`a[data-dk-dropdown-value='{appoimentType}']`).click()");
            }

            if (info.AppointmentType?.Equals(AppointmentType.AppointmentRequiredConfirmWithAll) ?? false)
            {
                this.OnAlertClickOk();
            }

            if (info.AppointmentType?.Equals(AppointmentType.AppointmentRequiredConfirmWithAny) ?? false)
            {
                var isAgentAccompanied = info.IsAgentAccompaniedShowing ? "Yes" : "No";
                this.UploaderClient.WaitForElementToBeVisible(By.Id("isAgentAccompany_No"), TimeSpan.FromMilliseconds(600));
                this.UploaderClient.ClickOnElementById($"isAgentAccompany_{isAgentAccompanied}");
            }
        }

        private void HandleConfirmationMethods(ContactDetailResponse contact, int position)
        {
            if (contact.ConfirmAppointmentsByText.Value)
            {
                this.UploaderClient.ExecuteScript($"document.querySelector('#confirmSms_{position}').click()");
            }

            if (contact.ConfirmAppointmentsByEmail.Value)
            {
                this.UploaderClient.ExecuteScript($"document.querySelector('#confirmEmail_{position}').click()");
            }

            if (contact.ConfirmAppointmentsByMobilePhone.Value || contact.ConfirmAppointmentsByOfficePhone.Value)
            {
                this.HandlePhoneConfirmation(contact, position);
            }
        }

        private void HandlePhoneConfirmation(ContactDetailResponse contact, int position)
        {
            this.UploaderClient.ExecuteScript($"document.querySelector('#confirmPhone_{position}').click()");
            var caller = contact.ConfirmAppointmentCallerByOfficePhone ?? contact.ConfirmAppointmentCallerByMobilePhone;
            this.UploaderClient.SetSelect(By.Id($"phoneRequestChoice_{position}"), caller.Equals(ConfirmAppointmentCaller.AutoCall));
        }

        private void HandleFYIMethods(ContactDetailResponse contact, int position)
        {
            if (!ShouldSendFYI(contact))
            {
                return;
            }

            this.UploaderClient.ExecuteScript($"document.querySelector('#ConfirmMethod_FYI_{position}').click()");

            if (contact.SendOnFYIByText.Value)
            {
                this.UploaderClient.ExecuteScript($"document.querySelector('#fyiSms_{position}').click()");
            }

            if (contact.SendOnFYIByEmail.Value)
            {
                this.UploaderClient.ExecuteScript($"document.querySelector('#fyiEmail_{position}').click()");
            }
        }

        private void OnAlertClickOk()
        {
            var alertXPath = By.XPath($"//div[@class='{AlertClass}']");
            this.UploaderClient.WaitForElementToBeVisible(alertXPath, TimeSpan.FromMinutes(1));
            var alert = this.UploaderClient.FindElement(alertXPath, isElementOptional: true);
            if (alert is null)
            {
                return;
            }

            var alertButton = alert.FindElement(By.TagName("button"));
            alertButton?.Click();
        }

        private void WaitUntilUnblockUI(By by, CancellationToken cancellationToken)
        {
            try
            {
                this.UploaderClient.WaitUntilElementDisappears(by, cancellationToken);
                Task.Delay(1000, cancellationToken).Wait(cancellationToken);
            }
            catch
            {
                // Nothing to do
            }
        }
    }
}
