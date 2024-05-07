namespace Husa.Uploader.Core.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Microsoft.Extensions.Logging;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public class UploaderClient : IUploaderClient, IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        private readonly IJavaScriptExecutor internalJSScript;
        private readonly ILogger<UploaderClient> logger;
        private readonly TimeSpan defaultImplicitWait;
        private bool disposedValue;

        public UploaderClient(IWebDriver webDriver, ILogger<UploaderClient> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.driver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            this.internalJSScript = (IJavaScriptExecutor)this.driver;
            this.wait = new WebDriverWait(this.driver, timeout: TimeSpan.FromSeconds(10));
            this.UploadInformation = new();
            this.defaultImplicitWait = this.driver.Manage().Timeouts().ImplicitWait;
        }

        ~UploaderClient()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: false);
        }

        public UploadCommandInfo UploadInformation { get; private set; }

        public string Url => this.driver.Url;

        public string Title => this.driver.Title;

        public string PageSource => this.driver.PageSource;

        public string CurrentWindowHandle => this.driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => this.driver.WindowHandles;

        public void InitializeUploadInfo(Guid listingRequestId, bool isNewListing)
        {
            if (listingRequestId == Guid.Empty)
            {
                throw new ArgumentException("The listingRequest Id may not be empty and must have a valid value", nameof(listingRequestId));
            }

            this.UploadInformation = new()
            {
                IsNewListing = isNewListing,
                RequestId = listingRequestId,
            };
        }

        public bool AcceptAlertWindow(bool isElementOptional = false, bool shouldWait = false)
        {
            bool isAlertPresent;
            try
            {
                if (shouldWait)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                var alert = this.driver.SwitchTo().Alert();
                this.logger.LogInformation("Alert window is present for the request, accepting it.");
                alert.Accept();
                isAlertPresent = true;
            }
            catch (NoAlertPresentException noAlertPresentException)
            {
                this.logger.LogError(noAlertPresentException, "Unable to accept the alert window the request, the element was not found.");
                isAlertPresent = false;
                if (!isElementOptional)
                {
                    throw;
                }
            }

            return isAlertPresent;
        }

        public void WaitForElementToBeVisible(By findBy, TimeSpan timeout)
        {
            var customWait = new WebDriverWait(this.driver, timeout);
            customWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            customWait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(findBy);
                    return element != null && element.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }

        public bool WaitUntilElementIsDisplayed(By findBy, CancellationToken token = default)
        {
            this.logger.LogInformation("Waiting for the element '{By}' to be displayed", findBy.ToString());
            return this.wait.Until(driver => driver.FindElement(findBy).Displayed, token);
        }

        public bool WaitUntilElementIsDisplayed(By findBy, TimeSpan waitTime, CancellationToken token = default)
        {
            this.logger.LogDebug("Waiting for the element '{By}' to be displayed", findBy.ToString());
            var customWait = new WebDriverWait(this.driver, waitTime);
            return customWait.Until(driver => driver.FindElement(findBy).Displayed, token);
        }

        public bool WaitUntilElementIsDisplayed(Func<IWebDriver, bool> waitCondition, CancellationToken token = default)
        {
            this.logger.LogDebug("Waiting for the condition '{@WaitCondition}'", waitCondition);
            return this.wait.Until(waitCondition, token);
        }

        public bool WaitUntilElementDisappears(By findBy, CancellationToken token = default)
        {
            this.logger.LogInformation("Waiting for the element '{By}' to disappear", findBy.ToString());
            return this.wait.Until(driver => !driver.FindElement(findBy).Displayed, token);
        }

        public bool WaitUntilScriptIsComplete(string script, string expectedCompletedResult, CancellationToken token = default)
        {
            this.logger.LogInformation("Waiting for the script '{Script}' to execute with the result {Result}", script, expectedCompletedResult);
            return this.wait.Until(x => this.ExecuteScript(script).Equals(expectedCompletedResult), token);
        }

        public void WaitUntilElementExists(By findBy, TimeSpan waitTime, CancellationToken token = default)
        {
            this.logger.LogInformation("Waiting for the element '{By}' to exist", findBy.ToString());
            var customWait = new WebDriverWait(this.driver, waitTime);
            customWait.Until(driver => driver.FindElement(findBy), token);
        }

        public void WaitUntilElementExists(By findBy, CancellationToken token = default)
        {
            this.logger.LogInformation("Waiting for the element '{By}' to exist", findBy.ToString());
            this.wait.Until(driver => driver.FindElement(findBy), token);
        }

        public void SetImplicitWait(TimeSpan waitTime)
        {
            this.logger.LogInformation("Setting implicit wait by {Time} ms", waitTime.TotalMilliseconds);
            this.driver.Manage().Timeouts().ImplicitWait = waitTime;
        }

        public void ResetImplicitWait()
        {
            this.driver.Manage().Timeouts().ImplicitWait = this.defaultImplicitWait;
        }

        public bool IsElementPresent(By findBy, bool isVisible = false)
        {
            try
            {
                var element = this.FindElement(findBy, shouldWait: false, isElementOptional: false);
                return isVisible && element.Displayed;
            }
            catch
            {
                // Intentionally left empty because all fields are optional
            }

            return false;
        }

        public bool IsElementVisible(By findBy)
        {
            try
            {
                var element = this.FindElement(findBy, shouldWait: false, isElementOptional: false);
                return element.Displayed;
            }
            catch (Exception exception) when (exception is NoSuchElementException || exception is StaleElementReferenceException)
            {
                this.logger.LogDebug(exception, "Skipping exception because is expected for element {FindBy}", findBy);
                return false;
            }
        }

        public IWebElement FindElement(By findBy, bool shouldWait = false, bool isElementOptional = false)
        {
            if (shouldWait)
            {
                this.WaitUntilElementIsDisplayed(findBy);
            }

            try
            {
                this.logger.LogInformation("Finding element by '{By}'", findBy.ToString());
                return this.driver.FindElement(findBy);
            }
            catch (NoSuchElementException ex)
            {
                if (!isElementOptional)
                {
                    this.logger.LogWarning(ex, "The non-optional element with locator: {By} was not found when processing the request {RequestId}.", findBy, this.UploadInformation.RequestId);
                    throw;
                }
            }

            return null;
        }

        public IWebElement FindElementById(string elementId)
        {
            this.logger.LogInformation("Finding elements by id '{ElementId}'", elementId);
            return this.FindElement(By.Id(elementId));
        }

        public IWebElement FindFirstElement(By by)
        {
            this.logger.LogInformation("Finding elements by '{By}'", by.ToString());
            return this.driver.FindElements(by).FirstOrDefault();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By findBy, bool shouldWait = false)
        {
            this.logger.LogInformation("Finding elements by '{By}'", findBy.ToString());
            if (shouldWait)
            {
                this.WaitUntilElementIsDisplayed(findBy);
            }

            return this.driver.FindElements(findBy);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string elementName, bool shouldWait = true)
        {
            this.logger.LogInformation("Finding elements by name '{ElementName}'", elementName);
            var filterBy = By.Name(elementName);
            return this.FindElements(filterBy, shouldWait);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string elementTagName, bool shouldWait = true)
        {
            this.logger.LogInformation("Finding elements by tag name '{ElementTagName}'", elementTagName);
            var filterBy = By.TagName(elementTagName);
            return this.FindElements(filterBy, shouldWait);
        }

        public void ClickOnElement(By findBy, bool shouldWait, int waitTime, bool isElementOptional, bool isSecondAttemp = false)
        {
            if (findBy is null)
            {
                throw new ArgumentNullException(nameof(findBy));
            }

            try
            {
                this.logger.LogInformation("Clicking on element by '{FindBy}'", findBy);
                if (!this.FindElements(findBy).Any())
                {
                    this.logger.LogInformation("Element by '{By}' not found", findBy.ToString());
                    return;
                }

                var elementToClick = this.FindElement(findBy, shouldWait);
                elementToClick.Click();
                if (shouldWait)
                {
                    Thread.Sleep(waitTime);
                }
            }
            catch (Exception exception)
                when (exception is NoSuchElementException ||
                      exception.InnerException is NoSuchElementException ||
                      exception is ElementNotVisibleException)
            {
                this.logger.LogWarning(exception, "Element by '{By}' was not found or is not visible", findBy.ToString());
                if (!isElementOptional)
                {
                    throw;
                }
            }
            catch (StaleElementReferenceException staleElementException)
            {
                if (!isSecondAttemp)
                {
                    this.logger.LogWarning(staleElementException, "Stale reference while trying to click element {By} while processing the request.", findBy);
                    this.ClickOnElement(findBy, shouldWait: false, waitTime: 0, isElementOptional, isSecondAttemp: true);
                }

                this.logger.LogError(staleElementException, "Repeated Stale reference while trying to click element {By} while processing the request.", findBy);
                throw;
            }
        }

        public void ClickOnElement(By findBy)
            => this.ClickOnElement(findBy, shouldWait: false, waitTime: 0, isElementOptional: false, isSecondAttemp: false);

        public void ClickOnElementById(string elementId, bool shouldWait = false, int waitTime = 400, bool isElementOptional = false)
            => this.ClickOnElement(By.Id(elementId), shouldWait, waitTime, isElementOptional);

        public void ClickOnElementByName(string elementName, bool shouldWait = false, int waitTime = 400, bool isElementOptional = false)
            => this.ClickOnElement(By.Name(elementName), shouldWait, waitTime, isElementOptional);

        public void WriteTextbox(
            By findBy,
            string entry,
            bool isElementOptional = false,
            bool handleAlerts = false,
            bool doNotClear = false,
            bool isSecondAttemp = false)
        {
            var element = this.FindElement(findBy, isElementOptional: isElementOptional);
            if (isElementOptional && element is null)
            {
                this.logger.LogInformation("Textbox {By} not found, skipping process.", findBy);
                return;
            }

            if (string.IsNullOrEmpty(entry))
            {
                this.logger.LogInformation("Tried to write a null value to textbox with locator: {By} when processing the request.", findBy);
                element.Clear();
                return;
            }

            if (!this.UploadInformation.IsNewListing && !doNotClear)
            {
                try
                {
                    element.Clear();
                }
                catch (InvalidElementStateException ex)
                {
                    this.logger.LogError(ex, "The element with locator: {By} was in an invalid state when processing the request.", findBy);
                    return;
                }

                if (handleAlerts)
                {
                    this.AcceptAlertWindow(isElementOptional: true, shouldWait: true);
                }
            }

            try
            {
                if (!doNotClear)
                {
                    element.Clear();
                }

                element.SendKeys(entry);
                if (handleAlerts)
                {
                    this.AcceptAlertWindow(isElementOptional: true, shouldWait: true);
                }
            }
            catch (StaleElementReferenceException staleElementException)
            {
                if (!isSecondAttemp)
                {
                    this.logger.LogWarning(staleElementException, "Stale reference while trying to write textbox {By} while processing request.", findBy);
                    this.WriteTextbox(findBy, entry, isElementOptional, handleAlerts, doNotClear, isSecondAttemp: true);
                }

                this.logger.LogError(staleElementException, "Repeated Stale reference while trying to  write textbox {By} while processing request.", findBy);
                throw;
            }

            if (handleAlerts)
            {
                this.AcceptAlertWindow(isElementOptional: true, shouldWait: true);
            }
        }

        public void WriteTextbox(By findBy, object value, bool isElementOptional = false)
            => this.WriteTextbox(findBy, entry: value?.ToString() ?? string.Empty, isElementOptional);

        public void SetSelect(By findBy, object value, bool isElementOptional = false) => this.SetSelect(findBy, value: value?.ToString() ?? string.Empty, isElementOptional);

        public void SetSelect(By findBy, string value, bool isElementOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.logger.LogWarning("Tried to select a null value in Select with locator: {By} when processing the request.", findBy);
                    return;
                }

                var element = this.FindElement(findBy, isElementOptional: isElementOptional);
                var select = new SelectElement(element);
                if (!this.UploadInformation.IsNewListing && select.IsMultiple)
                {
                    select.DeselectAll();
                }

                select.SelectByValue(value);
            }
            catch (Exception exception) when (exception is NoSuchElementException || exception is UnexpectedTagNameException)
            {
                this.logger.LogWarning(exception, "Failed when trying to select a non-existing value in Select or to transform an element into a Select with locator: {By} when processing the request.", findBy);
                if (!isElementOptional)
                {
                    throw;
                }
            }
        }

        public void SetSelect(By findBy, string value, string fieldLabel, string fieldSection, bool isElementOptional = false)
        {
            try
            {
                var element = this.driver.FindElement(findBy);
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.ExecuteScript(script: " jQuery('select[id^=" + element.GetAttribute("id") + "]').each( function () { jQuery(this).val(''); }); ");
                    this.UploadInformation.AddError(
                        fieldFindBy: findBy.ToString(),
                        fieldLabel,
                        fieldSection,
                        friendlyErrorMessage: $"Tried to select a non-existing {value} in Select with locator: {findBy}");

                    return;
                }

                var select = new SelectElement(element);
                if (!this.UploadInformation.IsNewListing && select.IsMultiple)
                {
                    select.DeselectAll();
                }

                select.SelectByValue(value);
            }
            catch (Exception exception)
            {
                string friendlyErrorMessage;
                switch (exception)
                {
                    case NoSuchElementException noSuchElementException:
                        friendlyErrorMessage = $"Tried to select a non-existing {value} in Select with locator: {findBy}";
                        this.logger.LogWarning(noSuchElementException, "Tried to select a non-existing {Value} in Select with locator: {By} with optional parameter set to {IsOptional} when processing request {RequestId}.", value, findBy, isElementOptional, this.UploadInformation.RequestId);
                        break;
                    case UnexpectedTagNameException noSuchElementException:
                        friendlyErrorMessage = $"Tried to transform an element with locator: {findBy} into a Select when processing the value(s) {value}.";
                        this.logger.LogWarning(noSuchElementException, "Tried to transform an element with locator: {By} into a Select with optional parameter set to {IsOptional} when processing the request {RequestId}.", findBy, isElementOptional, this.UploadInformation.RequestId);
                        break;
                    default:
                        this.logger.LogError(exception, "Unknow error in Select with locator: {By} with optional parameter set to {IsOptional} when processing the request {RequestId}.", findBy, isElementOptional, this.UploadInformation.RequestId);
                        friendlyErrorMessage = $"Unknow error in Select with locator: {findBy}";
                        break;
                }

                this.UploadInformation.AddError(
                    fieldFindBy: findBy.ToString(),
                    fieldLabel,
                    fieldSection,
                    friendlyErrorMessage: friendlyErrorMessage,
                    errorMessage: exception.Message);
            }
        }

        public void SetSelectIfExist(By findBy, string value)
        {
            var element = this.FindElement(findBy);
            var select = new SelectElement(element);
            if (select.Options.Any(x => x.GetAttribute("value") == value))
            {
                this.SetSelect(findBy, value);
            }
        }

        public void SetSelectWithScript(string fieldId, string containerClassName, int childIndex, string fieldValue, string fieldName, string fieldSection)
        {
            this.ExecuteScript($"$('.{containerClassName} select:eq({childIndex})').attr('id', '{fieldId}');");
            this.WaitUntilElementExists(By.Id(fieldId));
            this.SetSelect(By.Id(fieldId), fieldValue, fieldName, fieldSection);
        }

        public void SetSelectByText(By findBy, string value, string fieldLabel, string fieldSection, bool isElementOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.logger.LogWarning("Tried to select a null value in Select with locator: {By} when processing the request {RequestId}.", findBy, this.UploadInformation.RequestId);
                    return;
                }

                var element = this.FindElement(findBy, isElementOptional: isElementOptional);
                var select = new SelectElement(element);
                if (!this.UploadInformation.IsNewListing && select.IsMultiple)
                {
                    select.DeselectAll();
                }

                select.SelectByText(value);
            }
            catch (Exception exception)
            {
                string friendlyErrorMessage;
                switch (exception)
                {
                    case NoSuchElementException noSuchElementException:
                        friendlyErrorMessage = $"Tried to select a non-existing {value} in Select with locator: {findBy}";
                        this.logger.LogWarning(noSuchElementException, "Tried to select a non-existing {Value} in Select with locator: {By} with optional parameter set to {IsOptional} when processing request {RequestId}.", value, findBy, isElementOptional, this.UploadInformation.RequestId);
                        break;
                    case UnexpectedTagNameException noSuchElementException:
                        friendlyErrorMessage = $"Tried to transform an element with locator: {findBy} into a Select when processing the value(s) {value}.";
                        this.logger.LogWarning(noSuchElementException, "Tried to transform an element with locator: {By} into a Select with optional parameter set to {IsOptional} when processing the request {RequestId}.", findBy, isElementOptional, this.UploadInformation.RequestId);
                        break;
                    default:
                        this.logger.LogError(exception, "Unknow error in Select with locator: {By} with optional parameter set to {IsOptional} when processing the request {RequestId}.", findBy, isElementOptional, this.UploadInformation.RequestId);
                        friendlyErrorMessage = $"Unknow error in Select with locator: {findBy}";
                        break;
                }

                this.UploadInformation.AddError(
                    fieldFindBy: findBy.ToString(),
                    fieldLabel,
                    fieldSection,
                    friendlyErrorMessage: friendlyErrorMessage,
                    errorMessage: exception.Message);
            }
        }

        public void FillFieldSingleOption(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var mainWindow = this.WindowHandles.FirstOrDefault(windowHandle => windowHandle == this.CurrentWindowHandle);
            this.ExecuteScript(script: $"jQuery('#{fieldName}_TB').focus();");
            this.ExecuteScript(script: $"jQuery('#{fieldName}_A')[0].click();");

            this.SwitchToLast();

            Thread.Sleep(400);

            char[] fieldValue = value.ToUpper().ToArray();

            foreach (var charact in fieldValue)
            {
                Thread.Sleep(200);
                var elem = this.FindElement(By.Id("m_txtSearch"));
                if (elem != null)
                {
                    elem.SendKeys(charact.ToString().ToUpper());
                }
            }

            Thread.Sleep(400);
            var apostropheAdaptedValue = value.Replace("'", "\\'");
            var selectElement = $"const selected = jQuery('li[title=\"{apostropheAdaptedValue}\"]'); jQuery(selected).focus(); jQuery(selected).click()";
            this.ExecuteScript(script: selectElement);
            Thread.Sleep(400);

            this.ExecuteScript("javascript:LBI_Popup.selectItem(true);");
            this.SwitchTo().Window(mainWindow);
        }

        public void SetMultiSelect(By findBy, string csvValues, bool isElementOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(csvValues))
                {
                    this.logger.LogWarning("Tried to select a null value in Select with locator: {By} when processing the request {RequestId}.", findBy, this.UploadInformation.RequestId);
                    return;
                }

                var element = this.FindElement(findBy, isElementOptional: isElementOptional);
                var select = new SelectElement(element);
                if (!this.UploadInformation.IsNewListing && select.IsMultiple)
                {
                    select.DeselectAll();
                }

                var splitValues = csvValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var val in splitValues)
                {
                    try
                    {
                        select.SelectByValue(val);
                    }
                    catch (NoSuchElementException exception)
                    {
                        this.logger.LogWarning(exception, "Tried to select a non-existing value in Select with locator: {By} when processing request {RequestId}.", findBy, this.UploadInformation.RequestId);
                    }
                }
            }
            catch (Exception exception) when (exception is NoSuchElementException || exception is UnexpectedTagNameException)
            {
                // Intentionally left empty because all fields are optional
            }
        }

        public void SetMultipleCheckboxById(string id, string csvValues)
        {
            if (string.IsNullOrWhiteSpace(csvValues))
            {
                this.logger.LogWarning("Tried to use a null value in MultiCheckbox with locator: {Id} when processing request {RequestId}.", id, this.UploadInformation.RequestId);
                return;
            }

            var positionParentElement = 0;
            string posY = this.ExecuteScript(script: " return jQuery('#" + id + "').position().top;").ToString();
            if (!string.IsNullOrEmpty(posY))
            {
                var positionYDecimal = decimal.Parse(posY);
                positionParentElement = int.Parse(decimal.Round(positionYDecimal, 0).ToString()) - 100;
                this.ExecuteScript(script: " jQuery(document).scrollTop(0);");
                Thread.Sleep(400);
                this.ScrollDownPosition(positionParentElement);
            }

            var splitValues = csvValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!this.UploadInformation.IsNewListing)
            {
                try
                {
                    var elements = this.FindElement(By.Id(id)).FindElements(By.CssSelector("input[type=checkbox]"));
                    foreach (var element in elements.Where(el => el.Selected))
                    {
                        element.Click();
                    }
                }
                catch
                {
                    foreach (var value in splitValues)
                    {
                        var elementId = $"{id}_{value}";
                        this.ExecuteScript(script: " jQuery('#" + elementId + "').prop('checked', false);");
                    }
                }
            }

            var scriptResult = this.ExecuteScript(script: "  (function($) { $.fn.hasScrollBar = function() { return this.get(0).scrollHeight > this.height(); }  })(jQuery); return $('#" + id + "').hasScrollBar();  ");
            _ = bool.TryParse(scriptResult.ToString(), out var hasScrollBar);
            foreach (var value in splitValues)
            {
                if (!this.FindElements(By.Id(id)).Any())
                {
                    continue;
                }

                this.FindElement(By.Id(id), shouldWait: true);
                var elementId = $"{id}_{value}";
                try
                {
                    int positionOfItem = 0;
                    var posItemY = this.ExecuteScript(script: " return jQuery('#" + elementId + "').position().top;").ToString();
                    if (!string.IsNullOrEmpty(posItemY) && !hasScrollBar)
                    {
                        var positionYDecimal = decimal.Parse(posItemY);
                        positionOfItem = int.Parse(decimal.Round(positionYDecimal, 0).ToString()) - 100;
                        this.ExecuteScript(script: " jQuery(document).scrollTop(0);");
                        Thread.Sleep(400);
                        this.ScrollDownPosition(positionOfItem);
                    }
                }
                catch
                {
                    // Intentionally left empty because all fields are optional
                }

                Thread.Sleep(400);
                this.ClickOnElementById(elementId, isElementOptional: true);
            }
        }

        /// <summary>
        /// This method needs to be reviewed and reimplemented, it's too convoluted and overly defensive.
        /// </summary>
        public void SetMultipleCheckboxById(string id, string csvValues, string fieldLabel, string fieldSection)
        {
            string friendlyErrorMessage = "Tried to transform an element with locator: {" + id + "} into a Select when processing the values {" + csvValues + "}.";
            try
            {
                if (string.IsNullOrWhiteSpace(csvValues))
                {
                    this.logger.LogWarning("Tried to use a null value in MultiCheckbox with locator: {Id} when processing Request with {ResidentialListingRequestId}.", id, this.UploadInformation.RequestId);
                    this.ExecuteScript(" jQuery('input[id^=" + id + "]').each( function () { jQuery(this).prop('checked', false) }); ");
                    return;
                }

                var positionParentElement = 0;
                string posY = this.ExecuteScript(script: " var returnValue = 0; try { returnValue = jQuery('#" + id + "').position().top; } catch { } return returnValue; ").ToString();
                if (!string.IsNullOrEmpty(posY))
                {
                    var positionYDecimal = decimal.Parse(posY);
                    positionParentElement = int.Parse(decimal.Round(positionYDecimal, 0).ToString()) - 100;
                    this.ExecuteScript(" jQuery(document).scrollTop(0);");
                    Thread.Sleep(400);
                    this.ScrollDownPosition(positionParentElement);
                }

                var splitValues = csvValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!this.UploadInformation.IsNewListing)
                {
                    try
                    {
                        var elements = this.FindElementById(id).FindElements(By.CssSelector("input[type=checkbox]"));
                        foreach (var element in elements.Where(el => el.Selected))
                        {
                            element.Click();
                        }
                    }
                    catch
                    {
                        try
                        {
                            foreach (var value in splitValues)
                            {
                                var elementId = $"{id}_{value}";
                                this.ExecuteScript(script: " jQuery('#" + elementId + "').prop('checked', false);");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.UploadInformation.AddError(id, fieldLabel, fieldSection, friendlyErrorMessage, ex.Message);
                            throw;
                        }
                    }
                }

                var scriptResult = this.ExecuteScript(script: "  (function($) { $.fn.hasScrollBar = function() { return this.get(0).scrollHeight > this.height(); }  })(jQuery); return $('#" + id + "').hasScrollBar();  ");
                _ = bool.TryParse(scriptResult.ToString(), out var hasScrollBar);
                foreach (var value in splitValues)
                {
                    friendlyErrorMessage = "Tried to select a non-existing {" + value + "} in Select with locator: " + id;
                    var elementId = $"{id}_{value}";
                    try
                    {
                        this.WaitUntilElementIsDisplayed(By.Id(id));
                        this.FindElement(By.Id(id));

                        try
                        {
                            int positionOfItem = 0;
                            string posItemY = this.ExecuteScript(script: " var returnValue = 0; try { returnValue = jQuery('#" + elementId + "').position().top; } catch { } return returnValue; ", isScriptOptional: true).ToString();
                            if (!string.IsNullOrEmpty(posItemY) && !hasScrollBar)
                            {
                                var positionYDecimal = decimal.Parse(posItemY);
                                positionOfItem = int.Parse(decimal.Round(positionYDecimal, 0).ToString()) - 100;
                                this.ExecuteScript(script: " jQuery(document).scrollTop(0);");
                                Thread.Sleep(400);
                                this.ScrollDownPosition(positionOfItem);
                            }
                        }
                        catch
                        {
                            // Intentionally left empty because all fields are optional
                        }

                        Thread.Sleep(400);
                        this.ClickOnElementById(elementId);
                    }
                    catch (Exception exception)
                    {
                        this.ExecuteScript(script: "jQuery('#" + id + " [value=\"" + value + "\"]').attr('selected', 'selected');", isScriptOptional: true);
                        this.UploadInformation.UploaderErrors.Add(new UploaderError(id, fieldLabel, fieldSection, friendlyErrorMessage, errorMessage: string.Empty));
                        this.logger.LogError(exception, "Error when processing the request {RequestId}.", this.UploadInformation.RequestId);
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error when processing the request {RequestId}", this.UploadInformation.RequestId);
                this.UploadInformation.UploaderErrors.Add(new UploaderError(id, fieldLabel, fieldSection, "Unknow error in Select with locator: " + id, exception.Message));
                throw;
            }
        }

        public void SetRadioButton(By findBy, object value, bool isElementOptional = false)
        {
            try
            {
                var entry = value ?? string.Empty;
                var links = this.driver.FindElements(findBy).ToList();

                for (int linkIndex = 0; linkIndex < links.Count; linkIndex++)
                {
                    if (links[linkIndex].GetAttribute("value") == entry.ToString())
                    {
                        var option = links[linkIndex];
                        option.Click();
                    }
                }
            }
            catch (NoSuchElementException exception)
            {
                this.logger.LogWarning(exception, "The non-optional element with locator: {By} was not found when processing the request {RequestId}.", findBy, this.UploadInformation.RequestId);
                if (!isElementOptional)
                {
                    throw;
                }
            }
        }

        public void SetAttribute(By findBy, object value, string attributeName, bool isOptional = false, bool handleAlerts = false)
        {
            var element = this.FindElement(findBy, isElementOptional: isOptional);
            if (element == null)
            {
                this.logger.LogWarning("The optional element was not found with locator: {By} when processing the request {RequestId}.", findBy, this.UploadInformation.RequestId);
                return;
            }

            this.ExecuteScript(script: "arguments[0].setAttribute(arguments[1], arguments[2])", isScriptOptional: false, element, attributeName, value);
        }

        public void SubmitForm(By findBy, bool isElementOptional = false)
        {
            try
            {
                this.FindElement(findBy).Submit();
            }
            catch (NoSuchElementException ex)
            {
                if (!isElementOptional)
                {
                    this.logger.LogWarning(ex, "The non-optional element with locator: {By} was not found when processing the request.", findBy);
                    throw;
                }
            }
        }

        public void ScrollBy(int position, int pixels)
        {
            var scrollTopScript = $"window.scrollBy({position}, {pixels});";
            this.logger.LogDebug("Scrolling with {Script}", scrollTopScript);
            this.ExecuteScript(scrollTopScript);
        }

        public void ScrollToTop() => this.ScrollBy(position: 0, pixels: 0);

        public void ScrollDown(int pixels = 500) => this.ScrollBy(position: 0, pixels);

        public void ScrollDownPosition(int pixels = 500)
        {
            const string scrollTopScript = "return document.documentElement.scrollTop;";
            this.logger.LogDebug("Scrolling top with script {Script}", scrollTopScript);
            var currentPosition = this.ExecuteScript(scrollTopScript).ToString();

            var scrollByScript = $"window.scrollBy({currentPosition}, {pixels});";
            this.logger.LogDebug("Scrolling to position {CurrentPosition} top with script {Script}", currentPosition, scrollByScript);
            this.ExecuteScript(scrollByScript);
        }

        public void ScrollDownToElement(string elementId, int pixels, bool navigateToParent = false)
        {
            var scrollDownToElementScript = $"jQuery('{elementId}')";
            if (navigateToParent)
            {
                this.logger.LogInformation("Adding navigation to parent window for element {ElementId}", elementId);
                scrollDownToElementScript += ".parent()";
            }

            scrollDownToElementScript += $".animate({{ scrollTop: {pixels} }});";
            this.logger.LogDebug("Scrolling down to element with script {Script}", scrollDownToElementScript);
            this.ExecuteScript(scrollDownToElementScript);
        }

        public void ScrollDownToElementHTML(string elementId)
        {
            var scrollDownToElementScript = this.FindElementById(elementId);
            this.ExecuteScript("arguments[0].scrollIntoView(true);", args: scrollDownToElementScript);
        }

        public object ExecuteScript(string script, bool isScriptOptional = false, params object[] args)
        {
            this.logger.LogInformation("Executing script '{Script}'", script);
            try
            {
                return this.internalJSScript.ExecuteScript(script, args);
            }
            catch (Exception exception)
            {
                if (!isScriptOptional)
                {
                    this.logger.LogWarning(exception, "The non-optional script '{Script}' executed failed while processing the request {RequestId}.", script, this.UploadInformation.RequestId);
                    throw;
                }
            }

            return null;
        }

        public object ExecuteScriptAsync(string script, bool isScriptOptional = false, params object[] args)
        {
            try
            {
                this.logger.LogInformation("Executing async script '{Script}'", script);
                return this.internalJSScript.ExecuteAsyncScript(script, args);
            }
            catch (Exception exception)
            {
                if (!isScriptOptional)
                {
                    this.logger.LogWarning(exception, "The non-optional script '{Script}' executed failed while processing the request {RequestId}.", script, this.UploadInformation.RequestId);
                    throw;
                }
            }

            return null;
        }

        public void NavigateToUrl(string url)
        {
            this.logger.LogDebug("Navigating to {Url}", url);
            this.Navigate().GoToUrl(url);
        }

        public INavigation Navigate() => this.driver.Navigate();

        public ITargetLocator SwitchTo() => this.driver.SwitchTo();

        public void SwitchTo(string frame, bool switchToParent)
        {
            try
            {
                if (switchToParent)
                {
                    this.driver.SwitchTo().ParentFrame();
                }

                this.SwitchTo().Frame(frame);
            }
            catch (NoSuchElementException noSuchElementException)
            {
                this.logger.LogWarning(noSuchElementException, "The frame with locator: {Frame} was not found when processing the request.", frame);
            }
        }

        public void SwitchTo(string windowName) => this.SwitchTo().Window(windowName);

        public void SwitchToLast()
        {
            var totalWindows = this.WindowHandles.Count;
            var lastWindow = this.WindowHandles[totalWindows - 1];
            this.SwitchTo().Window(lastWindow);
        }

        public void CloseDriver()
        {
            if (this.driver == null)
            {
                return;
            }

            this.logger.LogInformation("Finalizing driver session");
            this.driver.Quit();

            try
            {
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }
            }
            catch
            {
                // Intentionally left empty because the result is not important
            }
        }

        public void FinalizeSession()
        {
            this.DeleteAllCookies();
            this.CloseDriver();
        }

        public void DeleteAllCookies() => this.driver.Manage().Cookies.DeleteAllCookies();

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.CloseDriver();
                this.disposedValue = true;
            }
        }

        private void SetValueToSelectField(string fieldId, string containerClassName, int childIndex, string fieldValue, string fieldName, string fieldSection)
        {
            this.ExecuteScript("$('." + containerClassName + " select:eq(" + childIndex + ")').attr('id', '" + fieldId + "');");
            this.WaitUntilElementIsDisplayed(By.Id(fieldId));
            this.SetSelect(By.Id(fieldId), fieldValue, fieldName, fieldSection);
        }

        private void SetValueToTextField(string fieldId, string containerClassName, int childIndex, string fieldValue)
        {
            this.ExecuteScript("$('." + containerClassName + " input[type=\"text\"]:eq(" + childIndex + ")').attr('id', '" + fieldId + "');");
            this.WaitUntilElementIsDisplayed(By.Id(fieldId));
            this.WriteTextbox(By.Id(fieldId), fieldValue);
        }
    }
}
