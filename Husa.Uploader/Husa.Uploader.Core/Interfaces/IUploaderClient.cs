namespace Husa.Uploader.Core.Interfaces
{
    using System.Collections.ObjectModel;
    using Husa.Uploader.Core.Models;
    using OpenQA.Selenium;

    public interface IUploaderClient
    {
        UploadCommandInfo UploadInformation { get; }
        string Url { get; }
        string Title { get; }
        string PageSource { get; }
        string CurrentWindowHandle { get; }
        ReadOnlyCollection<string> WindowHandles { get; }

        void InitializeUploadInfo(Guid listingRequestId, bool isNewListing);
        bool AcceptAlertWindow(bool isElementOptional = false, bool shouldWait = false);
        bool WaitUntilElementIsDisplayed(By findBy);
        bool WaitUntilElementIsDisplayed(By findBy, TimeSpan waitTime);
        bool WaitUntilElementIsDisplayed(Func<IWebDriver, bool> waitCondition);
        bool WaitUntilScriptIsComplete(string script, string expectedCompletedResult);
        void WaitUntilElementExists(By findBy);
        void WaitUntilElementExists(By findBy, TimeSpan waitTime);
        bool IsElementPresent(By findBy, bool isVisible = false);
        IWebElement FindElement(By findBy, bool shouldWait = false, bool isElementOptional = false);
        IWebElement FindElementById(string elementId);
        IWebElement FindFirstElement(By by);
        ReadOnlyCollection<IWebElement> FindElements(By findBy, bool shouldWait = false);
        ReadOnlyCollection<IWebElement> FindElementsByName(string elementName, bool shouldWait = true);
        ReadOnlyCollection<IWebElement> FindElementsByTagName(string elementTagName, bool shouldWait = true);
        void ClickOnElement(By findBy);
        void ClickOnElement(By findBy, bool shouldWait, int waitTime, bool isElementOptional, bool isSecondAttemp = false);
        void ClickOnElementById(string elementId, bool shouldWait = false, int waitTime = 400, bool isElementOptional = false);
        void ClickOnElementByName(string elementName, bool shouldWait = false, int waitTime = 400, bool isElementOptional = false);
        void WriteTextbox(By findBy, string entry, bool isElementOptional = false, bool handleAlerts = false, bool doNotClear = false, bool isSecondAttemp = false);
        void WriteTextbox(By findBy, object value, bool isElementOptional = false);
        void SetSelect(By findBy, object value, bool isElementOptional = false);
        void SetSelect(By findBy, string value, bool isElementOptional = false);
        void SetSelect(By findBy, string value, string fieldLabel, string fieldSection, bool isElementOptional = false);
        void SetSelectWithScript(string fieldId, string containerClassName, int childIndex, string fieldValue, string fieldName, string fieldSection);
        void SetSelectByText(By findBy, string value, string fieldLabel, string fieldSection, bool isElementOptional = false);
        void SetMultiSelect(By findBy, string csvValues, bool isElementOptional = false);
        void SetMultipleCheckboxById(string id, string values);
        void SetMultipleCheckboxById(string id, string values, string fieldLabel, string fieldSection);
        void SetRadioButton(By findBy, object value, bool isElementOptional = false);
        void SetAttribute(By findBy, object value, string attributeName, bool isOptional = false, bool handleAlerts = false);
        void SubmitForm(By findBy, bool isElementOptional = false);
        void ScrollToTop();
        void ScrollDown(int pixels = 500);
        void ScrollDownPosition(int pixels = 500);
        void ScrollDownToElement(string elementId, int pixels, bool navigateToParent = false);
        object ExecuteScript(string script, bool isScriptOptional = false, params object[] args);
        object ExecuteScriptAsync(string script, bool isScriptOptional = false, params object[] args);
        void NavigateToUrl(string url);
        INavigation Navigate();
        ITargetLocator SwitchTo();
        void SwitchTo(string frame, bool switchToParent);
        void SwitchTo(string windowName);
        void CloseDriver();
        void FinalizeSession();
        void DeleteAllCookies();
    }
}
