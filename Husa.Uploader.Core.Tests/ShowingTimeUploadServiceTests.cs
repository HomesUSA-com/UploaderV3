namespace Husa.Uploader.Core.Tests
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ShowingTime;
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
        private readonly Mock<IMarketUploadService> mockMarketUploadService;
        private readonly Mock<IUploaderClient> mockUploaderClient;
        private readonly Mock<ILogger> mockLogger;
        private readonly string agentSelectorValue = "test-agent";

        public ShowingTimeUploadServiceTests()
        {
            this.mockMarketUploadService = new Mock<IMarketUploadService>();
            this.mockUploaderClient = new Mock<IUploaderClient>();
            this.mockLogger = new Mock<ILogger>();

            this.mockMarketUploadService.Setup(x => x.UploaderClient).Returns(this.mockUploaderClient.Object);

            this.Sut = new ShowingTimeUploadService(
                this.mockMarketUploadService.Object,
                this.agentSelectorValue,
                this.mockLogger.Object);
        }

        public ShowingTimeUploadService Sut { get; private set; }

        [Fact]
        public async Task GetInShowingTimeSite_WithValidMlsNumber_ReturnsTrue()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "123456";
            var cancellationToken = CancellationToken.None;

            this.mockMarketUploadService
                .Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Logged);

            this.mockUploaderClient
                .Setup(x => x.ClickOnElement(
                    It.Is<By>(by => by.ToString().Contains("ctl02_ctl10_HyperLink2")),
                    true,
                    1000,
                    true,
                    false));

            this.mockUploaderClient
                .Setup(x => x.ClickOnElement(It.Is<By>(by => by.ToString().Contains("Input"))))
                .Verifiable();

            this.mockUploaderClient
                .Setup(x => x.WaitForElementToBeVisible(
                    It.Is<By>(by => by.ToString().Contains("m_dlInputList")),
                    It.IsAny<TimeSpan>()))
                .Verifiable();

            this.mockUploaderClient
                .Setup(x => x.WriteTextbox(
                    It.Is<By>(by => by.ToString().Contains("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox")),
                    mlsNumber,
                    false,
                    false,
                    false,
                    false))
                .Verifiable();

            this.mockUploaderClient
                .Setup(x => x.ClickOnElementById(
                    "m_lvInputUISections_ctrl0_lbQuickEdit", false, 400, false))
                .Verifiable();

            this.mockUploaderClient
                .Setup(x => x.FindElement(
                    It.Is<By>(by => by.ToString().Contains("Modify Property")),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .Returns(Mock.Of<IWebElement>());

            this.mockUploaderClient
                .Setup(x => x.ClickOnElementById(
                    "m_oThirdPartyLinks_m_lvThirdPartLinks_ctrl3_m_lbtnThirdPartyLink",
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .Verifiable();

            // Act
            var result = await this.Sut.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.True(result);
            this.mockMarketUploadService.Verify(x => x.Login(companyId), Times.Once);
            this.mockUploaderClient.Verify(
                x => x.ClickOnElement(
                    It.Is<By>(by => by.ToString().Contains("ctl02_ctl10_HyperLink2")),
                    true,
                    1000,
                    true,
                    It.IsAny<bool>()),
                Times.Once);
            this.mockUploaderClient.Verify();
        }

        [Fact]
        public async Task GetInShowingTimeSite_WithEmptyMlsNumber_ReturnsFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = string.Empty;
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await this.Sut.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.False(result);
            this.mockMarketUploadService.Verify(x => x.Login(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetInShowingTimeSite_WithNullMlsNumber_ReturnsFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            string mlsNumber = null;
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await this.Sut.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.False(result);
            this.mockMarketUploadService.Verify(x => x.Login(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetInShowingTimeSite_WithLoginFailure_ReturnsFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "123456";
            var cancellationToken = CancellationToken.None;

            this.mockMarketUploadService
                .Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Failure);

            // Act
            var result = await this.Sut.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.False(result);
            this.mockMarketUploadService.Verify(x => x.Login(companyId), Times.Once);
            this.mockUploaderClient.Verify(
                x => x.ClickOnElement(
                    It.IsAny<By>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task GetInShowingTimeSite_WithListingNotFound_ReturnsFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "123456";
            var cancellationToken = CancellationToken.None;

            this.mockMarketUploadService
                .Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Logged);

            this.mockUploaderClient
                .Setup(x => x.FindElement(
                    It.Is<By>(by => by.ToString().Contains("Modify Property")),
                    It.IsAny<bool>(),
                    true))
                .Returns((IWebElement)null);

            // Act
            var result = await this.Sut.GetInShowingTimeSite(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.False(result);
            this.mockMarketUploadService.Verify(x => x.Login(companyId), Times.Once);
            this.mockUploaderClient.Verify(
                x => x.ClickOnElementById(
                    "m_oThirdPartyLinks_m_lvThirdPartLinks_ctrl3_m_lbtnThirdPartyLink",
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteDuplicateClients_Success_ReturnsCorrectCount()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "12345";
            var cancellationToken = CancellationToken.None;

            this.SetupGetInShowingTimeSiteSuccess(companyId, mlsNumber);
            this.SetupDeleteDuplicateClientsFlow();

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(1, result); // Based on mock setup with 2 duplicates
            this.VerifyDeleteDuplicateClientsFlow();
        }

        [Fact]
        public async Task DeleteDuplicateClients_GetInShowingTimeSiteFails_ReturnsZero()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "12345";
            var cancellationToken = CancellationToken.None;

            this.SetupGetInShowingTimeSiteFailure(companyId);

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteDuplicateClients_NoDuplicates_ReturnsZero()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "12345";
            var cancellationToken = CancellationToken.None;

            this.SetupGetInShowingTimeSiteSuccess(companyId, mlsNumber);
            this.SetupNoDuplicatesFlow();

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteDuplicateClients_WithCancellation_ThrowsOperationCanceledException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "12345";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.SetupGetInShowingTimeSiteSuccess(companyId, mlsNumber);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(
                async () => await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [Fact]
        public async Task DeleteDuplicateClients_EmptyMlsNumber_ReturnsZero()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = string.Empty;
            var cancellationToken = CancellationToken.None;

            this.mockMarketUploadService.Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Logged);

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteDuplicateClients_NullMlsNumber_ReturnsZero()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            string mlsNumber = null;
            var cancellationToken = CancellationToken.None;

            this.mockMarketUploadService.Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Logged);

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteDuplicateClients_MultiplePages_ProcessesAllPages()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var mlsNumber = "12345";
            var cancellationToken = CancellationToken.None;

            this.SetupGetInShowingTimeSiteSuccess(companyId, mlsNumber);
            this.SetupMultiplePagesFlow();

            // Act
            var result = await this.Sut.DeleteDuplicateClients(companyId, mlsNumber, cancellationToken);

            // Assert
            Assert.Equal(2, result); // Based on mock setup with 4 entries and 2 duplicates across pages
        }

        [Fact]
        public async Task SetAppointmentCenter_Success()
        {
            this.mockUploaderClient.Setup(x => x.ClickOnElementById(It.IsAny<string>(), false, 400, false)).Verifiable();

            await this.Sut.SetAppointmentCenter(CancellationToken.None);

            this.mockUploaderClient.Verify(x => x.ClickOnElementById(It.IsAny<string>(), false, 400, false), Times.Exactly(2));
        }

        [Fact]
        public async Task SetAppointmentSettings_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            var clickOnElementByIdQty = showingTime.AppointmentSettings.AppointmentType.Equals(AppointmentType.AppointmentRequiredConfirmWithAny) ? 3 : 2;
            this.mockUploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            this.mockUploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetAppointmentSettings(showingTime.AppointmentSettings.AppointmentType.Value, CancellationToken.None);

            this.mockUploaderClient.Verify(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
            this.mockUploaderClient.Verify(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()),
                Times.Exactly(clickOnElementByIdQty));
        }

        [Fact]
        public async Task SetAppointmentRestrictions_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            this.mockUploaderClient.Setup(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Verifiable();
            this.mockUploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetAppointmentRestrictions(showingTime.AppointmentRestrictions, CancellationToken.None);

            this.mockUploaderClient.Verify(
                x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()), Times.Exactly(2));
            this.mockUploaderClient.Verify(
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
            this.mockUploaderClient.Setup(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false));

            await this.Sut.SetAccessInformation(showingTime.AccessInformation, CancellationToken.None);

            this.mockUploaderClient.Verify(x => x.SetSelect(It.IsAny<By>(), It.IsAny<object>(), It.IsAny<bool>()), Times.AtMostOnce);
            this.mockUploaderClient.Verify(x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()), Times.AtMostOnce);
            this.mockUploaderClient.Verify(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false), Times.Exactly(writeTextBoxQty));
        }

        [Fact]
        public async Task SetAdditionalInstructions_Success()
        {
            var showingTime = this.ShowingTimeFaker();
            this.mockUploaderClient.Setup(x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false));

            await this.Sut.SetAdditionalInstructions(showingTime.AdditionalInstructions, CancellationToken.None);

            this.mockUploaderClient.Verify(
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
            this.mockUploaderClient.Setup(x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            this.mockUploaderClient.Setup(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false))
                .Verifiable();
            this.mockUploaderClient.Setup(x => x.FindElement(It.IsAny<By>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(htmlElement.Object)
                .Verifiable();
            this.mockUploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()));

            var position = Faker.RandomNumber.Next(1, 2);
            var contact = showingTime.Contacts.ElementAt(position - 1);
            var result = await this.Sut.AddExistentContact(contact, position, CancellationToken.None);

            this.mockUploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(executionScriptQty));
            this.mockUploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.Once);
            this.mockUploaderClient.Verify(
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
            this.mockUploaderClient.Setup(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false))
                .Verifiable();
            this.mockUploaderClient.Setup(
                x => x.FindElementById(It.IsAny<string>())).Returns(htmlElement.Object).Verifiable();
            this.mockUploaderClient.Setup(
                x => x.ClickOnElementById(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.EditContact(this.ShowingTimeFaker().Contacts.ElementAt(0), 1, CancellationToken.None);

            this.mockUploaderClient.Verify(
                x => x.WriteTextbox(It.IsAny<By>(), It.IsAny<string>(), false, false, false, false),
                Times.AtLeast(3));
            this.mockUploaderClient.Verify(
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

            var confirmSectionMock = new Mock<IWebElement>();
            confirmSectionMock.SetupAllProperties();
            confirmSectionMock.Setup(x => x.GetCssValue(It.IsAny<string>()))
                .Returns("block");
            this.mockUploaderClient.Setup(
                x => x.FindElement(
                    It.Is<By>(y => y.Criteria == ".confirm\\-section"), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(confirmSectionMock.Object);
            this.mockUploaderClient.Setup(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()))
                .Verifiable();

            await this.Sut.SetContactConfirmSection(contact, 2, CancellationToken.None);

            this.mockUploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.AtLeast(executeScriptQty));
        }

        [Fact]
        public async Task SetContactNotificationSection_Success()
        {
            var executeScriptQty = 1;
            var contact = this.ShowingTimeFaker().Contacts.ElementAt(1);
            this.mockUploaderClient.Setup(
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

            this.mockUploaderClient.Verify(
                x => x.ExecuteScript(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(executeScriptQty));
        }

        private static IWebElement CreateMockTableRow(string firstName, string lastName, string email, string phone, string listingCount)
        {
            var row = new Mock<IWebElement>();

            // Create mock cells for the table row
            var firstNameCell = new Mock<IWebElement>();
            firstNameCell.Setup(x => x.Text).Returns(firstName);

            var lastNameCell = new Mock<IWebElement>();
            lastNameCell.Setup(x => x.Text).Returns(lastName);

            var emailCell = new Mock<IWebElement>();
            emailCell.Setup(x => x.Text).Returns(email);

            var phoneCell = new Mock<IWebElement>();
            phoneCell.Setup(x => x.Text).Returns(phone);

            var listingCountCell = new Mock<IWebElement>();
            listingCountCell.Setup(x => x.Text).Returns(listingCount);

            var dummyCell = new Mock<IWebElement>();
            dummyCell.Setup(x => x.Text).Returns(string.Empty);

            var actionCell = new Mock<IWebElement>();
            var actionLink = new Mock<IWebElement>();
            actionLink.Setup(x => x.Text).Returns("Edit");
            actionLink.Setup(x => x.GetAttribute("href")).Returns($"#edit_{firstName}_{lastName}");
            actionCell.Setup(x => x.FindElement(By.TagName("a"))).Returns(actionLink.Object);
            actionCell.Setup(x => x.Text).Returns("Edit");

            // Create mock delete button/checkbox for the row
            var deleteButton = new Mock<IWebElement>();
            deleteButton.Setup(x => x.GetAttribute("id")).Returns($"deleteButton_{firstName}_{lastName}");

            // Setup the row to return the cells when FindElements is called
            var cells = new List<IWebElement>
            {
                firstNameCell.Object,
                lastNameCell.Object,
                emailCell.Object,
                phoneCell.Object,
                listingCountCell.Object,
                dummyCell.Object,
                actionCell.Object,
            };

            row.Setup(x => x.FindElements(By.TagName("td")))
                .Returns(cells.AsReadOnly());

            // Setup finding the delete button within the row
            row.Setup(x => x.FindElement(By.XPath(".//input[@type='checkbox']")))
                .Returns(deleteButton.Object);

            return row.Object;
        }

        private ShowingTimeFullInfoResponse ShowingTimeFaker() => new()
        {
            AppointmentSettings = new()
            {
                AppointmentType = Faker.Enum.Random<AppointmentType>(),
            },
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

        private void SetupGetInShowingTimeSiteSuccess(Guid companyId, string mlsNumber)
        {
            this.mockMarketUploadService.Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Logged);

            this.mockUploaderClient.Setup(
                x => x.ClickOnElement(
                    By.Id("ctl02_ctl10_HyperLink2"), true, 1000, true, It.IsAny<bool>()));

            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a/span[text()='Input']")));
            this.mockUploaderClient.Setup(x => x.WaitForElementToBeVisible(By.Id("m_dlInputList"), TimeSpan.FromSeconds(3)));
            this.mockUploaderClient.Setup(x => x.WriteTextbox(
                By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"),
                mlsNumber,
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElementById(
                "m_lvInputUISections_ctrl0_lbQuickEdit",
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<bool>()));

            var modifyPropertyElement = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//span[text()='Modify Property']"), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(modifyPropertyElement.Object);

            this.mockUploaderClient.Setup(x => x.ClickOnElementById(
                "m_oThirdPartyLinks_m_lvThirdPartLinks_ctrl3_m_lbtnThirdPartyLink",
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<bool>()));
        }

        private void SetupGetInShowingTimeSiteFailure(Guid companyId)
        {
            this.mockMarketUploadService.Setup(x => x.Login(companyId))
                .ReturnsAsync(LoginResult.Failure);
        }

        private void SetupDeleteDuplicateClientsFlow()
        {
            var windowHandles = new ReadOnlyCollection<string>(new List<string>
                {
                    "window1",
                    "window2",
                });
            this.mockUploaderClient.Setup(x => x.WindowHandles).Returns(windowHandles);
            this.mockUploaderClient.Setup(x => x.SwitchTo()).Returns(Mock.Of<ITargetLocator>());

            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.XPath("//button/span[text()='Remind Me Later']"), true, 1000, true, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Contacts']")));
            this.mockUploaderClient.Setup(x => x.SetSelect(
                By.ClassName("selectList"), this.agentSelectorValue, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Sellers']")));

            // Setup pagination
            var paginationElement = new Mock<IWebElement>();
            paginationElement.Setup(x => x.Text).Returns("2");
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//div[@id='clientSellersTable_paginate']/span/a[last()]"), true, It.IsAny<bool>()))
                .Returns(paginationElement.Object);

            // Setup table with duplicate clients
            this.SetupTableWithDuplicates();
        }

        private void SetupTableWithDuplicates()
        {
            var tableElement = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElementById("clientSellersTable"))
                .Returns(tableElement.Object);

            // First page - 2 clients with same name/phone
            var row0 = CreateMockTableRow("First Name", "Last Name", "Email", "Phone", "Listings");
            var row1 = CreateMockTableRow("John", "Doe", string.Empty, "555-1234", "1 listing");
            var row2 = CreateMockTableRow("John", "Doe", string.Empty, "555-1234", string.Empty);
            var rows = new List<IWebElement> { row0, row1, row2 };

            tableElement.Setup(x => x.FindElements(By.TagName("tr")))
                .Returns(rows.AsReadOnly());

            // Setup search and delete flow
            this.mockUploaderClient.Setup(x => x.WriteTextbox(
                By.Id("tableFilter"),
                "John Doe",
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.Id("deleteButton"), true, 3000, false, It.IsAny<bool>()));

            var confirmButton = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//button/span[text()='Yes']"), true, false))
                .Returns(confirmButton.Object);
            confirmButton.Setup(x => x.Click());
        }

        private void SetupNoDuplicatesFlow()
        {
            var windowHandles = new ReadOnlyCollection<string>(new List<string> { "window1", "window2" });
            this.mockUploaderClient.Setup(x => x.WindowHandles).Returns(windowHandles);
            this.mockUploaderClient.Setup(x => x.SwitchTo()).Returns(Mock.Of<ITargetLocator>());

            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.XPath("//button/span[text()='Remind Me Later']"), true, 1000, true, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Contacts']")));
            this.mockUploaderClient.Setup(x => x.SetSelect(
                By.ClassName("selectList"), this.agentSelectorValue, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Sellers']")));

            var paginationElement = new Mock<IWebElement>();
            paginationElement.Setup(x => x.Text).Returns("1");
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//div[@id='clientSellersTable_paginate']/span/a[last()]"),
                true,
                It.IsAny<bool>()))
                .Returns(paginationElement.Object);

            var tableElement = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElementById("clientSellersTable"))
                .Returns(tableElement.Object);

            // No duplicate clients
            var row0 = CreateMockTableRow("First Name", "Last Name", "Email", "Phone", "Listings");
            var row1 = CreateMockTableRow("John", "Doe", string.Empty, "555-1234", string.Empty);
            var row2 = CreateMockTableRow("Jane", "Smith", string.Empty, "555-5678", string.Empty);
            var rows = new List<IWebElement> { row0, row1, row2 };

            tableElement.Setup(x => x.FindElements(By.TagName("tr")))
                .Returns(rows.AsReadOnly());
        }

        private void SetupMultiplePagesFlow()
        {
            var windowHandles = new ReadOnlyCollection<string>(new List<string> { "window1", "window2" });
            this.mockUploaderClient.Setup(x => x.WindowHandles).Returns(windowHandles);
            this.mockUploaderClient.Setup(x => x.SwitchTo()).Returns(Mock.Of<ITargetLocator>());

            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.XPath("//button/span[text()='Remind Me Later']"), true, 1000, true, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Contacts']")));
            this.mockUploaderClient.Setup(x => x.SetSelect(
                By.ClassName("selectList"), this.agentSelectorValue, It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(By.XPath("//a[text()='Sellers']")));

            var paginationElement = new Mock<IWebElement>();
            paginationElement.Setup(x => x.Text).Returns("3");
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//div[@id='clientSellersTable_paginate']/span/a[last()]"), true, It.IsAny<bool>()))
                .Returns(paginationElement.Object);

            var tableElement = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElementById("clientSellersTable"))
                .Returns(tableElement.Object);

            // Multiple pages with duplicates - 4 total duplicates across pages
            var row0 = CreateMockTableRow("First Name", "Last Name", "Email", "Phone", "Listings");
            var row1 = CreateMockTableRow("John", "Doe", string.Empty, "555-1234", string.Empty);
            var row2 = CreateMockTableRow("John", "Doe", string.Empty, "555-1234", "1 listing");
            var row3 = CreateMockTableRow("Jane", "Smith", string.Empty, "555-5678", string.Empty);
            var row4 = CreateMockTableRow("Jane", "Smith", string.Empty, "555-5678", "2 listings");
            var rows = new List<IWebElement> { row0, row1, row2, row3, row4 };

            tableElement.Setup(x => x.FindElements(By.TagName("tr")))
                .Returns(rows.AsReadOnly());

            // Setup search and delete flow for multiple duplicates
            this.mockUploaderClient.Setup(x => x.WriteTextbox(
                By.Id("tableFilter"),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()));
            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.Id("deleteButton"), true, 3000, false, It.IsAny<bool>()));

            var confirmButton = new Mock<IWebElement>();
            this.mockUploaderClient.Setup(x => x.FindElement(
                By.XPath("//button/span[text()='Yes']"), true, false))
                .Returns(confirmButton.Object);
            confirmButton.Setup(x => x.Click());

            // Setup pagination navigation for multiple pages
            this.mockUploaderClient.Setup(x => x.ClickOnElement(
                By.XPath("//div[@id='clientSellersTable_paginate']/span/a[contains(@class,'paginate_button') and not(contains(@class,'previous')) and not(contains(@class,'next'))]")));
        }

        private void VerifyDeleteDuplicateClientsFlow()
        {
            // Verify window switching
            this.mockUploaderClient.Verify(x => x.WindowHandles, Times.AtLeastOnce);
            this.mockUploaderClient.Verify(x => x.SwitchTo(), Times.AtLeastOnce);

            // Verify navigation clicks
            this.mockUploaderClient.Verify(
                x => x.ClickOnElement(
                By.XPath("//button/span[text()='Remind Me Later']"), true, 1000, true, It.IsAny<bool>()),
                Times.Once);
            this.mockUploaderClient.Verify(x => x.ClickOnElement(By.XPath("//a[text()='Contacts']")), Times.Once);
            this.mockUploaderClient.Verify(x => x.ClickOnElement(By.XPath("//a[text()='Sellers']")), Times.Once);

            // Verify agent selector
            this.mockUploaderClient.Verify(
                x => x.SetSelect(By.ClassName("selectList"), this.agentSelectorValue, It.IsAny<bool>()),
                Times.Once);

            // Verify pagination check
            this.mockUploaderClient.Verify(
                x => x.FindElement(
                By.XPath("//div[@id='clientSellersTable_paginate']/span/a[last()]"), true, It.IsAny<bool>()),
                Times.Once);

            // Verify table access
            this.mockUploaderClient.Verify(x => x.FindElementById("clientSellersTable"), Times.AtLeastOnce);

            // Verify search functionality for duplicates
            this.mockUploaderClient.Verify(
                x => x.WriteTextbox(
                    By.Id("tableFilter"),
                    "John Doe",
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()),
                Times.AtLeastOnce);

            // Verify delete button click
            this.mockUploaderClient.Verify(
                x => x.ClickOnElement(By.Id("deleteButton"), true, 3000, false, It.IsAny<bool>()),
                Times.AtLeastOnce);

            // Verify confirmation dialog
            this.mockUploaderClient.Verify(
                x => x.FindElement(By.XPath("//button/span[text()='Yes']"), true, false),
                Times.AtLeastOnce);
        }
    }
}
