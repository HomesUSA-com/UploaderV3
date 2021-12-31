using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
//using Husa.Uploader.EventLog;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Husa.Core.UploaderBase
{
    public static class WebDriverExtensions
    {
        /// <summary>
        /// Clicks the element selected using the 'by'.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="isOptional">Whether the click is optional or required. If required, the code will log a warning</param>
        /// <param name="isSecondAttemp">If the click is the second attemp. Used to handle certain transient exceptions.</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver Click(this CoreWebDriver driver, By by, bool isOptional = false, bool isSecondAttemp = false)
        {
            try
            {
                //driver.wait.Until(ExpectedConditions.ElementExists(by));
                driver.FindElement(by).Click();
            }
            catch (NoSuchElementException ex)
            {
                if (!isOptional)
                {
                    driver.Logger.Warning(ex, "The non-optional element with locator: {by} was not found when processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    throw;
                }
            }
            catch (StaleElementReferenceException ex)
            {
                if (!isSecondAttemp)
                {

                    driver.Logger.Warning(ex, "Stale reference while trying to click element {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    return Click(driver, by, isOptional, true);
                }
                driver.Logger.Error(ex, "Repeated Stale reference while trying to click element {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                throw;
            }
            catch (ElementNotVisibleException ex)
            {
                if (!isOptional)
                {
                    //EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Element Not Visible while trying to click element " + by + " while processing Request " + driver.UploadInformation.RequestId + ".\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    driver.Logger.Error(ex, "Element Not Visible while trying to click element {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                //EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Undetermined exception when trying to click element " + by + " while processing Request . " + driver.UploadInformation.RequestId + ".\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                driver.Logger.Error(ex, "Undetermined exception when trying to click element {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                throw;
            }

            return driver;
        }

        /// <summary>
        /// Gets a WebDriverWait configured for the CoreWebDriver provided that lasts the duration specified.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="duration">The length of time that the wait should last. Defaults to 60 seconds.</param>
        /// <returns>A new instance of a WebDriverWait configured for the duration specified</returns>
        public static WebDriverWait GetWait(this CoreWebDriver driver, TimeSpan? duration = null)
        {
            if (!duration.HasValue)
                duration = TimeSpan.FromSeconds(60);

            if (duration.Value.TotalSeconds < 60)
                throw new ArgumentOutOfRangeException("duration", duration, "Cannot have a duration of less than 60 seconds");

            return new WebDriverWait(driver, duration.Value);
        }

        /// <summary>
        /// Points the browser managed by the CoreWebDriver to the URL provided
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="url">The url to navigate to</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver Navigate(this CoreWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
            return driver;
        }

        /// <summary>
        /// Submits the form identified with the 'by' provided and triggers all related actions
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="isOptional">Whether the form submission is optional or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SubmitForm(this CoreWebDriver driver, By by, bool isOptional = false)
        {
            try
            {
                driver.FindElement(by).Submit();
            }
            catch (NoSuchElementException ex)
            {
                if (!isOptional)
                    driver.Logger.Warning(ex, "The non-optional element with locator: {by} was not found when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }

            return driver;
        }

        /// <summary>
        /// Scroll to top document
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        public static CoreWebDriver ScrollToTop(this CoreWebDriver driver)
        {
            driver.ExecuteScript("window.scrollBy(0,0);");

            return driver;
        }

        /// <summary>
        /// Scroll the document by "pixels" vertically
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="pixels">Is an optional value to define how much pixels you want scroll down by default: 500px</param>
        public static CoreWebDriver ScrollDown(this CoreWebDriver driver, int pixels = 500)
        {
            driver.ExecuteScript("window.scrollBy(0, " + pixels + ");");

            return driver;
        }

        /// <summary>
        /// Scroll the document by "pixels" vertically
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="pixels">Is an optional value to define how much pixels you want scroll down by default: 500px</param>
        public static CoreWebDriver ScrollDownPosition(this CoreWebDriver driver, int pixels = 500)
        {
            var currentPosition = driver.ExecuteScript("return document.documentElement.scrollTop;").ToString();
            driver.ExecuteScript("window.scrollBy(" + currentPosition + ", " + pixels + ");");

            return driver;
        }

        public static CoreWebDriver ScrollDownToElement(this CoreWebDriver driver, String containerId, int pixels)
        {
            driver.ExecuteScript("jQuery('" + containerId + "').animate({ scrollTop: " + pixels + " });");
            return driver;
        }


        public static CoreWebDriver ScrollDownToElement(this CoreWebDriver driver, String elemtId, int pixels, bool action)
        {
            driver.ExecuteScript("jQuery('" + elemtId + "').parent().animate({ scrollTop: " + pixels + " });");
            return driver;
        }

        /// <summary>
        /// Writes the value provided to the textbox identified with the 'by' parameter.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value to write to the textbox</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <param name="handleAlerts">When set to true, if a javascript alerts appear, accept it and continue</param>
        /// <param name="doNotClear">When set to true, does not clear the textbox before writing the new information</param>
        /// <param name="isSecondAttemp">If the click is the second attemp. Used to handle certain transient exceptions.</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver WriteTextbox(this CoreWebDriver driver, By by, string value, bool isOptional = false, bool handleAlerts = false, bool doNotClear = false, bool isSecondAttemp = false)
        {
            IWebElement elem;
            var entry = value ?? "";

            try
            {
                elem = driver.FindElement(by);
            }
            catch (NoSuchElementException ex)
            {
                if (!isOptional)
                {
                    driver.Logger.Warning(ex, "The non-optional element with locator: {by} was not found when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    throw;
                }
                return driver;
            }

            if (value == null)
            {
                driver.Logger.Information("Tried to write a null value to textbox with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                return driver;
            }

            if (!driver.UploadInformation.IsNewListing && !doNotClear)
            {
                try
                {
                    elem.Clear();
                }
                catch (InvalidElementStateException ex)
                {
                    driver.Logger.Error(ex, "The element with locator: {by} was in an invalid state when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    return driver;
                }

                if (handleAlerts)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    try
                    {
                        var alert = driver.SwitchTo().Alert();
                        alert.Accept();
                    } // try 
                    catch (NoAlertPresentException)
                    { }
                }
            }
            try
            {
                if(!doNotClear)
                elem.Clear();

                elem.SendKeys(entry);
                if (handleAlerts)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    try
                    {
                        var alert = driver.SwitchTo().Alert();
                        alert.Accept();
                    } // try 
                    catch (NoAlertPresentException)
                    { }
                }

            }
            catch (StaleElementReferenceException ex)
            {
                if (!isSecondAttemp)
                {

                    driver.Logger.Warning(ex, "Stale reference while trying to write textbox {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    return WriteTextbox(driver, by, value, isOptional, handleAlerts, doNotClear, true);
                }
                driver.Logger.Error(ex, "Repeated Stale reference while trying to  write textbox {by} while processing Request {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                throw;
            }
            if (handleAlerts)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                try
                {
                    var alert = driver.SwitchTo().Alert();
                    alert.Accept();
                } // try 
                catch (NoAlertPresentException)
                { }
            }

            return driver;
        }

        /// <summary>
        /// Writes the value provided to the textbox identified with the 'by' parameter.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value to write to the textbox</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver WriteTextbox(this CoreWebDriver driver, By by, object value, bool isOptional = false)
        {
            var entry = value ?? "";

            return WriteTextbox(driver, by, entry.ToString(), isOptional);
        }

        /// <summary>
        /// Activates the checkboxes that represent the values provided in the 'values' parameter
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="id">The multiple checkboxes common id</param>
        /// <param name="values">A comma separated list of the values to activate/select</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetMultipleCheckboxById(this CoreWebDriver driver, string id, string values, bool isOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(values))
                {
                    //if (!isOptional)
                        driver.Logger.Warning("Tried to use a null value in MultiCheckbox with locator: {id} when processing Request with {ResidentialListingRequestId}.", id, driver.UploadInformation.RequestId);

                    return driver;
                }

                int positionParentElement = 0;
                string posY = driver.ExecuteScript(" return jQuery('#" + id + "').position().top;").ToString();
                if(!String.IsNullOrEmpty(posY))
                {
                    var positionYDecimal = Decimal.Parse(posY);
                    positionParentElement = int.Parse(Decimal.Round(positionYDecimal, 0).ToString()) - 100;
                    driver.ExecuteScript(" jQuery(document).scrollTop(0);");
                    Thread.Sleep(400);
                    driver.ScrollDownPosition(positionParentElement);
                }

                var splitValues = values.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!driver.UploadInformation.IsNewListing)
                {

                    try 
                    {
                        var elems = driver.FindElement(By.Id(id)).FindElements(By.CssSelector("input[type=checkbox]"));
                        //var elems = driver.FindElement(By.Id(id))
                        foreach (var el in elems.Where(el => el.Selected))
                        {
                            el.Click();
                        }
                    }
                    catch 
                    {
                        try
                        {
                            foreach(var value in splitValues)
                            {
                                driver.ExecuteScript(" jQuery('#"+ id + "_"+ value + "').prop('checked', false);");
                            }
                        }
                        catch { throw; }
                    }
                }

                bool hasScrollBar = Boolean.Parse(driver.ExecuteScript("  (function($) { $.fn.hasScrollBar = function() { return this.get(0).scrollHeight > this.height(); }  })(jQuery); return $('#" + id + "').hasScrollBar();  ").ToString());

                foreach (var value in splitValues)
                {
                    try
                    {
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
                        driver.FindElement(By.Id(id));

                        try
                        {
                            int positionOfItem = 0;
                            string posItemY = driver.ExecuteScript(" return jQuery('#" + id + "_" + value + "').position().top;").ToString();
                            if (!String.IsNullOrEmpty(posItemY) && !hasScrollBar)
                            {
                                var positionYDecimal = Decimal.Parse(posItemY);
                                positionOfItem = int.Parse(Decimal.Round(positionYDecimal, 0).ToString()) - 100;
                                driver.ExecuteScript(" jQuery(document).scrollTop(0);");
                                Thread.Sleep(400);
                                driver.ScrollDownPosition(positionOfItem);
                            }
                        } catch { }

                        Thread.Sleep(400);
                        driver.Click(By.Id(id + "_" + value));
                    }
                    catch (Exception e)
                    {
                        driver.Logger.Error("Error when processing Request with {ResidentialListingRequestId}.\nError Details: " + e.Message, null, driver.UploadInformation.RequestId);
                    }
                }

                return driver;
            }
            catch (Exception e)
            {
                driver.Logger.Error("Error when processing Request with {ResidentialListingRequestId}.\nError Details: " + e.Message, null, driver.UploadInformation.RequestId);
                throw e;
            }
        }

        /// <summary>
        /// Selects an specific value into a Select element identified with the 'by' provided
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value to choose in the Select element</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetSelect(this CoreWebDriver driver, By by, string value, bool isOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (!isOptional)
                        driver.Logger.Warning("Tried to select a null value in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);

                    return driver;
                }

                if (isOptional && string.IsNullOrWhiteSpace(value))
                    return driver;

                var elem = driver.FindElement(by);

                var select = new SelectElement(elem);

                if (!driver.UploadInformation.IsNewListing && select.IsMultiple)
                    select.DeselectAll();

                select.SelectByValue(value);
            }
            catch (NoSuchElementException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to select a non-existing {value} in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }
            catch (UnexpectedTagNameException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to transform an element with locator: {by} into a Select when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }

            return driver;
        }

        /// <summary>
        /// Selects an specific value into a Select element identified with the 'by' provided
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="text">The text to choose in the Select element</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetSelectByText(this CoreWebDriver driver, By by, string text, bool isOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (!isOptional)
                        driver.Logger.Warning("Tried to select a null value in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);

                    return driver;
                }

                if (isOptional && string.IsNullOrWhiteSpace(text))
                    return driver;

                var elem = driver.FindElement(by);

                var select = new SelectElement(elem);

                if (!driver.UploadInformation.IsNewListing && select.IsMultiple)
                    select.DeselectAll();

                select.SelectByText(text);
            }
            catch (NoSuchElementException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to select a non-existing {value} in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }
            catch (UnexpectedTagNameException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to transform an element with locator: {by} into a Select when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }

            return driver;
        }

        /// <summary>
        /// Selects an specific value into a Select element identified with the 'by' provided
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value to choose in the Select element</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetSelect(this CoreWebDriver driver, By by, object value, bool isOptional = false)
        {
            var entry = value ?? "";
            return SetSelect(driver, by, entry.ToString(), isOptional);
        }

        /// <summary>
        /// Selects the options that represent the values provided in the 'values' parameter
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="values">A comma separated list of the values to activate/select</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetMultiSelect(this CoreWebDriver driver, By by, string values, bool isOptional = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(values))
                {
                    if (!isOptional)
                        driver.Logger.Warning("Tried to select a null value in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);

                    return driver;
                }

                if (isOptional && string.IsNullOrWhiteSpace(values))
                    return driver;

                var elem = driver.FindElement(by);

                var select = new SelectElement(elem);

                if (!driver.UploadInformation.IsNewListing && select.IsMultiple)
                    select.DeselectAll();

                var splitValues = values.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var val in splitValues)
                {
                    try
                    {
                        select.SelectByValue(val);
                    }
                    catch (NoSuchElementException)
                    {
                        if (!isOptional)
                            driver.Logger.Warning("Tried to select a non-existing {value} in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
                    }
                }
            }
            catch (NoSuchElementException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to select a non-existing {value} in Select with locator: {by} when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }
            catch (UnexpectedTagNameException)
            {
                if (!isOptional)
                    driver.Logger.Warning("Tried to transform an element with locator: {by} into a Select when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);
            }

            return driver;
        }

        /// <summary>
        /// Switch the driver to the specified frame
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="frame">The id of the frame to switch to</param>
        /// <param name="switchToParent">whether or not it needs to switch to the parent frame prior to switching to the specified frame</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SwitchTo(this CoreWebDriver driver, string frame, bool switchToParent = false)
        {
            try
            {
                if (switchToParent)
                    driver.SwitchTo().ParentFrame();
                driver.SwitchTo().Frame(frame);
            }
            catch (NoSuchElementException ex)
            {
                driver.Logger.Warning(ex, "The frame with locator: {frame} was not found when processing Request with {ResidentialListingRequestId}.", frame, driver.UploadInformation.RequestId);
            }

            return driver;
        }

        /// <summary>
        /// Sets the attribute provided in the element identified with the 'by' parameter to the 'value' provided
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value to set in the attribute</param>
        /// <param name="attributeName">The attribute to modify</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetAttribute(this CoreWebDriver driver, By by, object value, string attributeName, bool isOptional = false, bool handleAlerts = false)
        {
            IWebElement element;
            try
            {
                element = driver.FindElement(by);
            }
            catch (NoSuchElementException ex)
            {
                if (!isOptional)
                    driver.Logger.Warning(ex, "The non-optional element with locator: {by} was not found when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);

                return driver;
            }

            var javascript = (IJavaScriptExecutor)driver.InternalWebDriver;

            javascript.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
            return driver;
        }

        /// <summary>
        /// Selects the readie button identified with the 'by' provided that has the correct 'value' attribute
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="by">The Selenium selector used to determine the element to act on</param>
        /// <param name="value">The value of the radio button to activate</param>
        /// <param name="isOptional">Whether the value is optional (and so may be null or empty) or required. If required, the code will log a warning</param>
        /// <returns>The same instance of CoreWebDriver provided to the method. Used for extension method chaining</returns>
        public static CoreWebDriver SetRadioButton(this CoreWebDriver driver, By by, object value, bool isOptional = false)
        {
            try
            {
                var entry = value ?? "";
                IWebElement option = driver.FindElement(by);
                List<IWebElement> Links = new List<IWebElement>(driver.FindElements(by));

                for (int k = 0; k < Links.Count; k++)
                {
                    if (Links[k].GetAttribute("value") == entry.ToString())
                    {
                        option = Links[k];
                        option.Click();
                    }
                }

            }
            catch (NoSuchElementException ex)
            {
                if (!isOptional)
                    driver.Logger.Warning(ex, "The non-optional element with locator: {by} was not found when processing Request with {ResidentialListingRequestId}.", by, driver.UploadInformation.RequestId);

                return driver;
            }

            return driver;
        }
    }
}
