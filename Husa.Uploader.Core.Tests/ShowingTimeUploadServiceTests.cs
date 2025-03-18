namespace Husa.Uploader.Core.Tests
{
    using System.Linq;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.CompanyServicesManager.Api.Contracts.Response;
    using Husa.Quicklister.Extensions.Api.Contracts.Models.ShowingTime;
    using Husa.Quicklister.Extensions.Domain.Enums.ShowingTime;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OpenQA.Selenium;
    using Xunit;

    [Collection(nameof(ApplicationServicesFixture))]
    public class ShowingTimeUploadServiceTests
    {
        private readonly Mock<IUploaderClient> uploaderClient = new();
        private readonly Mock<ILogger<ShowingTimeUploadService>> logger = new();
        private readonly Mock<IServiceSubscriptionClient> serviceSubscriptionClient = new();

        public ShowingTimeUploadServiceTests()
        {
            this.uploaderClient.SetupAllProperties();
            this.Sut = new(
                this.uploaderClient.Object,
                this.serviceSubscriptionClient.Object,
                this.logger.Object);
        }

        public ShowingTimeUploadService Sut { get; private set; }

        [Fact]
        public async Task Login_Success()
        {
            var sut = this.Sut;
            var companyClient = new Mock<ICompany>();
            companyClient.Setup(x => x.GetShowingTimeInfo(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ShowingTimeResponse()));
            this.serviceSubscriptionClient.SetupGet(x => x.Company).Returns(companyClient.Object);
            this.uploaderClient.Setup(x => x.NavigateToUrl(It.IsAny<string>())).Verifiable();
            this.uploaderClient.Setup(x =>
                x.WaitForElementToBeVisible(It.IsAny<By>(), It.IsAny<TimeSpan>())).Verifiable();
            this.uploaderClient.Setup(x =>
                x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false))
                .Verifiable();
            this.uploaderClient.Setup(x => x.ClickOnElement(It.IsAny<By>())).Verifiable();

            var result = await sut.Login(Guid.Empty);

            this.uploaderClient.Verify(x => x.NavigateToUrl(It.IsAny<string>()), Times.Once);
            this.uploaderClient.Verify(
                x => x.WaitForElementToBeVisible(It.IsAny<By>(), It.IsAny<TimeSpan>()),
                Times.Once);
            this.uploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.Exactly(2));
            this.uploaderClient.Verify(x => x.ClickOnElement(It.IsAny<By>()), Times.Exactly(2));
            Assert.Equal(LoginResult.Logged, result);
        }

        [Fact]
        public async Task NavigateToListing_Success()
        {
            this.uploaderClient.Setup(x => x.NavigateToUrl(It.IsAny<string>())).Verifiable();

            await this.Sut.NavigateToListing("123456789", CancellationToken.None);

            this.uploaderClient.Verify(x => x.NavigateToUrl(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SetAppointmentCenter_Success()
        {
            this.uploaderClient.Setup(x => x.ClickOnElementById(It.IsAny<string>(), false, 400, false)).Verifiable();

            await this.Sut.SetAppointmentCenter(CancellationToken.None);

            this.uploaderClient.Verify(x => x.ClickOnElementById(It.IsAny<string>(), false, 400, false), Times.Exactly(2));
        }

        [Fact]
        public async Task SetAppointmentSettings_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            var clickOnElementByIdQty = showingTime.AppointmentType.Equals(AppointmentType.AppointmentRequired) ? 3 : 2;
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            this.uploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetAppointmentSettings(showingTime.AppointmentType.Value, CancellationToken.None);

            this.uploaderClient.Verify(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
            this.uploaderClient.Verify(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()),
                Times.Exactly(clickOnElementByIdQty));
        }

        [Fact]
        public async Task SetAppointmentRestrictions_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            this.uploaderClient.Setup(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Verifiable();
            this.uploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetAppointmentRestrictions(showingTime.AppointmentRestrictions, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()), Times.Exactly(2));
            this.uploaderClient.Verify(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task SetAccessInformation_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            var accessMethod = showingTime.AccessInformation.AccessMethod.Value;
            var alarmDetails = showingTime.AccessInformation.ProvideAlarmDetails;
            var accessMethodOptions = new AccessMethod[] { AccessMethod.CodeBox, AccessMethod.Combination, AccessMethod.Keypad };
            var writeTextBoxQty = (accessMethodOptions.Contains(accessMethod) ? 2 : 3) + (alarmDetails ? 4 : 0);
            this.uploaderClient.Setup(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()));
            this.uploaderClient.Setup(x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()));
            this.uploaderClient.Setup(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false));

            await this.Sut.SetAccessInformation(showingTime.AccessInformation, CancellationToken.None);

            this.uploaderClient.Verify(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()), Times.Once);
            this.uploaderClient.Verify(x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            this.uploaderClient.Verify(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false), Times.Exactly(writeTextBoxQty));
        }

        [Fact]
        public async Task SetAdditionalInstructions_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            this.uploaderClient.Setup(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false));

            await this.Sut.SetAdditionalInstructions(showingTime.AdditionalInstructions, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.Exactly(2));
        }

        [Theory]
        [InlineData("")]
        [InlineData("No matching records found")]
        public async Task AddExistentContact_Success(string htmlElementText)
        {
            var htmlElement = new Mock<IWebElement>();
            var showingTime = this.ShowingTimeFaker();
            var expectedResult = string.IsNullOrEmpty(htmlElementText);
            var executionScriptQty = expectedResult ? 2 : 3;
            var clickOnElementByIdQty = expectedResult ? 3 : 0;
            htmlElement.Setup(x => x.Text).Returns(htmlElementText);
            this.uploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            this.uploaderClient.Setup(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false))
                .Verifiable();
            this.uploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(htmlElement.Object)
                .Verifiable();
            this.uploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()));

            var position = Faker.RandomNumber.Next(1, 2);
            var contact = showingTime.Contacts.ElementAt(position - 1);
            var result = await this.Sut.AddExistentContact(contact, position, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(executionScriptQty));
            this.uploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.Once);
            this.uploaderClient.Verify(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()),
                Times.Exactly(clickOnElementByIdQty));
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task AddNewContact_Success()
        {
            var contact = this.ShowingTimeFaker().Contacts.ElementAt(1);

            var result = await this.Sut.AddNewContact(contact, 2, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task EditContact_Success()
        {
            var htmlElement = new Mock<IWebElement>();
            this.uploaderClient.Setup(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false))
                .Verifiable();
            this.uploaderClient.Setup(
                x => x.FindElementById(It.IsAny<string>())).Returns(htmlElement.Object).Verifiable();
            this.uploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.EditContact(this.ShowingTimeFaker().Contacts.ElementAt(0), 1, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.AtLeast(3));
            this.uploaderClient.Verify(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task SetContactConfirmSection_Success()
        {
            var contact = this.ShowingTimeFaker().Contacts.ElementAt(1);
            var executeScriptQty = 0;

            if (contact.ConfirmAppointmentsByMobilePhone.Value || contact.ConfirmAppointmentsByOfficePhone.Value)
            {
                executeScriptQty += 2;
            }

            if (contact.ConfirmAppointmentsByEmail.Value
                    || contact.ConfirmAppointmentsByMobilePhone.Value
                    || contact.ConfirmAppointmentsByOfficePhone.Value
                    || contact.ConfirmAppointmentsByText.Value)
            {
                executeScriptQty += 1;
            }

            this.uploaderClient.Setup(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetContactConfirmSection(contact, 2, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.AtLeast(executeScriptQty));
        }

        [Fact]
        public async Task SetContactNotificationSection_Success()
        {
            var executeScriptQty = 1;
            var contact = this.ShowingTimeFaker().Contacts.ElementAt(1);
            this.uploaderClient.Setup(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()))
                .Verifiable();
            if (contact.NotifyAppointmentsChangesByText.Value)
            {
                executeScriptQty += 1;
            }

            if (contact.NotifyAppointmentChangesByEmail.Value)
            {
                executeScriptQty += 1;
            }

            if (contact.NotifyAppointmentChangesByOfficePhone.Value || contact.NotifyAppointmentChangesByMobilePhone.Value)
            {
                executeScriptQty += 1;
            }

            await this.Sut.SetContactNotificationSection(contact, 2, CancellationToken.None);

            this.uploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(executeScriptQty));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FindListing_Success(bool listingFound)
        {
            var mlsNumber = Faker.RandomNumber.Next(1000000, 9999999).ToString();
            this.uploaderClient.Setup(
                x => x.FindElement(It.IsAny<By>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(listingFound ? new Mock<IWebElement>().Object : null)
                .Verifiable();

            Assert.Equal(listingFound, await this.Sut.FindListing(mlsNumber, CancellationToken.None));
        }

        private ShowingTimeFullInfo ShowingTimeFaker() => new()
        {
            AppointmentType = Faker.Enum.Random<AppointmentType>(),
            AppointmentRestrictions = new()
            {
                AllowAppraisals = Faker.Boolean.Random(),
                AllowInspectionsAndWalkThroughs = Faker.Boolean.Random(),
                LeadTime = Faker.Boolean.Random(),
                SuggestedTimeHours = Faker.RandomNumber.Next(1, 8),
                RequiredTimeHours = Faker.RandomNumber.Next(1, 4),
            },
            AccessInformation = new()
            {
                AccessMethod = Faker.Enum.Random<AccessMethod>(),
                ProvideAlarmDetails = Faker.Boolean.Random(),
                Location = Faker.RandomNumber.Next(1000, 9999).ToString(),
                Serial = Faker.RandomNumber.Next(1000, 9999).ToString(),
                Combination = Faker.RandomNumber.Next(1000, 9999).ToString(),
                SharingCode = Faker.RandomNumber.Next(1000, 9999).ToString(),
                CbsCode = Faker.RandomNumber.Next(1000, 9999).ToString(),
                Code = Faker.RandomNumber.Next(1000, 9999).ToString(),
                DeviceId = Faker.RandomNumber.Next(1000, 9999).ToString(),
                AlarmArmCode = Faker.RandomNumber.Next(1000, 9999).ToString(),
                AlarmDisarmCode = Faker.RandomNumber.Next(1000, 9999).ToString(),
                AlarmNotes = Faker.Lorem.Paragraph(10),
            },
            AdditionalInstructions = new()
            {
                NotesForApptStaff = Faker.Lorem.Paragraph(10),
                NotesForShowingAgent = Faker.Lorem.Paragraph(10),
            },
            Contacts =
            [
                new()
                {
                    Id = Guid.Parse("27DF3C14-85DB-4AE4-B938-02CD7620EA5D"),
                    FirstName = "BEN",
                    LastName = "CABALLERO",
                    OfficePhone = "4699165493",
                    MobilePhone = string.Empty,
                    Email = "caballero@homesusa.com",
                    ConfirmAppointmentsByOfficePhone = false,
                    ConfirmAppointmentCallerByOfficePhone = null,
                    NotifyAppointmentChangesByOfficePhone = false,
                    AppointmentChangesNotificationsOptionsOfficePhone = null,
                    ConfirmAppointmentsByMobilePhone = false,
                    ConfirmAppointmentCallerByMobilePhone = null,
                    NotifyAppointmentChangesByMobilePhone = false,
                    AppointmentChangesNotificationsOptionsMobilePhone = null,
                    ConfirmAppointmentsByText = false,
                    NotifyAppointmentsChangesByText = false,
                    SendOnFYIByText = false,
                    ConfirmAppointmentsByEmail = true,
                    NotifyAppointmentChangesByEmail = true,
                    SendOnFYIByEmail = true,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    OfficePhone = "1111111111",
                    MobilePhone = "1111111111",
                    Email = Faker.Internet.Email(),
                    ConfirmAppointmentsByOfficePhone = Faker.Boolean.Random(),
                    ConfirmAppointmentCallerByOfficePhone = Faker.Enum.Random<ConfirmAppointmentCaller>(),
                    NotifyAppointmentChangesByOfficePhone = Faker.Boolean.Random(),
                    AppointmentChangesNotificationsOptionsOfficePhone = Faker.Enum.Random<AppointmentChangesNotificationOptions>(),
                    ConfirmAppointmentsByMobilePhone = Faker.Boolean.Random(),
                    ConfirmAppointmentCallerByMobilePhone = Faker.Enum.Random<ConfirmAppointmentCaller>(),
                    NotifyAppointmentChangesByMobilePhone = Faker.Boolean.Random(),
                    AppointmentChangesNotificationsOptionsMobilePhone = Faker.Enum.Random<AppointmentChangesNotificationOptions>(),
                    ConfirmAppointmentsByText = Faker.Boolean.Random(),
                    NotifyAppointmentsChangesByText = Faker.Boolean.Random(),
                    SendOnFYIByText = Faker.Boolean.Random(),
                    ConfirmAppointmentsByEmail = Faker.Boolean.Random(),
                    NotifyAppointmentChangesByEmail = Faker.Boolean.Random(),
                    SendOnFYIByEmail = Faker.Boolean.Random(),
                },
            ],
        };
    }
}
