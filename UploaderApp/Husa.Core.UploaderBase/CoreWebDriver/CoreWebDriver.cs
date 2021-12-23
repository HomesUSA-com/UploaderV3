using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace Husa.Core.UploaderBase
{
    public sealed class CoreWebDriver : IWebDriver, IJavaScriptExecutor
    {
        public CoreWebDriver(IWebDriver driver, ILogger logger, int residentialListingRequestId)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }

            Logger = logger;
            InternalWebDriver = driver;
            InternalJSScript = (IJavaScriptExecutor)InternalWebDriver;
            UploadInformation = new WebDriverUploadInformation { RequestId = residentialListingRequestId };

            this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
        }

        public WebDriverWait wait;

        public ILogger Logger { get; private set; }

        public IWebDriver InternalWebDriver { get; private set; }

        public IJavaScriptExecutor InternalJSScript { get; private set; }

        public WebDriverUploadInformation UploadInformation { get; private set; }

        #region WebDriver Passthrough Implementation

        public IWebElement FindElement(By by)
        {
            return InternalWebDriver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return InternalWebDriver.FindElements(by);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return InternalJSScript.ExecuteAsyncScript(script, args);
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return InternalJSScript.ExecuteScript(script, args);
        }

        public void Close()
        {
            InternalWebDriver.Close();
        }

        public void Quit()
        {
            InternalWebDriver.Quit();
        }

        public IOptions Manage()
        {
            return InternalWebDriver.Manage();
        }

        public INavigation Navigate()
        {
            return InternalWebDriver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return InternalWebDriver.SwitchTo();
        }

        public string Url
        {
            get { return InternalWebDriver.Url; }
            set { InternalWebDriver.Url = value; }
        }

        public string Title
        {
            get { return InternalWebDriver.Title; }
        }

        public string PageSource
        {
            get { return InternalWebDriver.PageSource; }
        }

        public string CurrentWindowHandle
        {
            get { return InternalWebDriver.CurrentWindowHandle; }
        }

        public ReadOnlyCollection<string> WindowHandles
        {
            get { return InternalWebDriver.WindowHandles; }
        }
        #endregion

        #region IDisposible Interface

        // Flag: Has Dispose already been called? 
        bool _disposed;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                InternalWebDriver.Dispose();
                InternalWebDriver = null;
            }

            _disposed = true;
        }

        #endregion
    }
}