using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Husa.Uploader.Support;
using Husa.Core.UploaderBase;
using Husa.Core.Uploaders.SanAntonio;
using OpenQA.Selenium.Chrome;
using Husa.Uploader.EventLog;
using System.Diagnostics;

namespace Husa.Uploader
{
    public static class UploaderEngine
    {
        private static readonly List<IUploader> Uploaders;
        private static readonly List<CoreWebDriver> Drivers = new List<CoreWebDriver>();
        private static readonly ChromeDriverService NoPluginDriverService;
        private static readonly ChromeOptions NoPluginOptions;
        private static readonly ChromeDriverService WithPluginDriverService;
        private static readonly ChromeOptions WithPluginOptions;

        static UploaderEngine()
        {
            //PrimeUploaders
            // This code should be removed as soon as a correct mechanism for handling Uploaders is developed
            IUploader prime = new SanAntonioUploader();

            Uploaders = GetUploaders();

            NoPluginOptions = new ChromeOptions();
            NoPluginOptions.AddArguments("test-type", "disable-extensions", "disable-plugin", "disable-bundled-ppapi-flash", "start-maximized", "--incognito", "--disable-geolocation");
            NoPluginOptions.AddUserProfilePreference("credentials_enable_service", false);
            NoPluginOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            NoPluginOptions.AddUserProfilePreference("profile.default_content_settings.geolocation", 2);
            NoPluginOptions.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            NoPluginOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);


            NoPluginDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            NoPluginDriverService.HideCommandPromptWindow = true;

            WithPluginOptions = new ChromeOptions();
            WithPluginOptions.AddArguments("test-type", "disable-extensions", "start-maximized", "--incognito", "--disable-geolocation");
            WithPluginOptions.AddUserProfilePreference("credentials_enable_service", false);
            WithPluginOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            WithPluginOptions.AddUserProfilePreference("profile.default_content_settings.geolocation", 2);
            WithPluginOptions.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            WithPluginOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);

            WithPluginDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            WithPluginDriverService.HideCommandPromptWindow = true;
        }

        private static UploadResponse ExecUpload<T>(Func<T, CoreWebDriver, ResidentialListingRequest, UploadResult> func, ResidentialListingRequest listing) where T : IUploader
        {
            CoreWebDriver webDriver = null;
            try
            {
                var upl = (T)GetUploader(listing);
                webDriver = GetWebDriver(listing.ResidentialListingRequestID, upl.IsFlashRequired);
                
                var result = func(upl, webDriver, listing);

                return new UploadResponse
                {
                    Driver = webDriver,
                    Result = result,
                    Uploader = upl,
                    Exception = null,
                    Listing = listing
                };
            }
            catch (Exception ex)
            {
                ShellViewModel vm = new ShellViewModel();
                vm.ShowCancelButton = false;
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error while executing Update market data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                return new UploadResponse
                {
                    Driver = webDriver,
                    Result = UploadResult.Failure,
                    Uploader = null,
                    Exception = ex,
                    Listing = listing
                };
            }
        }

        #region Listing

        public static UploadResponse Upload(ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            return ExecUpload<IUploader>((uploader, driver, residentialListing) => uploader.Upload(driver, residentialListing, media), listing);
        }

        public static UploadResponse Edit(ResidentialListingRequest listing)
        {
            return ExecUpload<IEditor>((uploader, driver, residentialListing) => uploader.Edit(driver, residentialListing), listing);
        }

        public static UploadResponse UpdateStatus(ResidentialListingRequest listing)
        {
            return ExecUpload<IStatusUploader>((uploader, driver, residentialListing) => uploader.UpdateStatus(driver, residentialListing), listing);
        }

        public static UploadResponse UpdatePrice(ResidentialListingRequest listing)
        {
            return ExecUpload<IPriceUploader>((uploader, driver, residentialListing) => uploader.UpdatePrice(driver, residentialListing), listing);
        }

        public static UploadResponse UpdateCompletionDate(ResidentialListingRequest listing)
        {
            return ExecUpload<ICompletionDateUploader>((uploader, driver, residentialListing) => uploader.UpdateCompletionDate(driver, residentialListing), listing);
        }

        public static UploadResponse UpdateImages(ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            return ExecUpload<IImageUploader>((uploader, driver, residentialListing) => uploader.UpdateImages(driver, residentialListing, media), listing);
        }

        public static UploadResponse UpdateOpenHouse(ResidentialListingRequest listing)
        {
            return ExecUpload<IUpdateOpenHouseUploader>((uploader, driver, residentialListing) => uploader.UpdateOpenHouse(driver, residentialListing), listing);
        }

        public static UploadResponse UpdateVirtualTour(ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            return ExecUpload<IUploadVirtualTourUploader>((uploader, driver, residentialListing) => uploader.UploadVirtualTour(driver, residentialListing, media), listing);
        }

        #endregion

        #region Leasing

        public static UploadResponse UpdateLease(ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            return ExecUpload<ILeaseUploader>((uploader, driver, residentialListing) => uploader.UpdateLease(driver, residentialListing, media), listing);
        }

        public static UploadResponse UpdateLeaseStatus(ResidentialListingRequest listing) 
        {
            return ExecUpload<IStatusLeaseUploader>((uploader, driver, residentialListing) => uploader.UpdateStatusLease(driver, residentialListing), listing);
        }

        #endregion

        #region Lots
        
        public static UploadResponse UpdateLot(ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            return ExecUpload<ILotUploader>((uploader, driver, residentialListing) => uploader.UpdateLot(driver, residentialListing, media), listing);
        }

        public static UploadResponse UpdateLotStatus(ResidentialListingRequest listing)
        {
            return ExecUpload<IStatusLotUploader>((uploader, driver, residentialListing) => uploader.UpdateStatusLot(driver, residentialListing), listing);
        }

        #endregion

        #region Drivers manage

        private static CoreWebDriver GetWebDriver(Guid requestId, bool allowFlash)
        {
            var logger = LoggingSupport.GetLogger(requestId.ToString(), Guid.NewGuid());

            var driver = allowFlash
                ? new CoreWebDriver(new ChromeDriver(WithPluginDriverService, WithPluginOptions), logger, requestId)
                : new CoreWebDriver(new ChromeDriver(NoPluginDriverService, NoPluginOptions), logger, requestId);

            Drivers.Add(driver);
            return driver;
        }

        private static List<IUploader> GetUploaders()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var uploaders = new List<IUploader>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    uploaders.AddRange(assembly.GetExportedTypes()
                              .Where(t => t.IsClass && typeof(IUploader).IsAssignableFrom(t))
                              .Select(t => Activator.CreateInstance(t) as IUploader));
                }
                catch (NotSupportedException) //We swallow this exception because it is caused by Dynamic Assemblies
                { }
            }

            return uploaders;
        }

        private static IUploader GetUploader(ResidentialListingRequest listing)
        {
            var uploader = Uploaders.FirstOrDefault(c => c.CanUpload(listing));

            return uploader != null
                ? Activator.CreateInstance(uploader.GetType()) as IUploader
                : null;
        } 

        public static bool UploaderSupports<T>(ResidentialListingRequest listing) where T : IUploader
        {
            if (listing == null)
                return false;

            var uploader = Uploaders.FirstOrDefault(c => c.CanUpload(listing));

            if (uploader == null)
                return false;

            var ff = uploader.GetType().GetInterfaces().Any(c => c == typeof(T));

            return ff;
        }

        public static void CloseDrivers()
        {
            foreach (var driver in Drivers.ToList())
            {
                CloseDriver(driver);
            }

            try
            {
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }

            }
            catch { }
        }

        public static void CloseDriver(CoreWebDriver driver)
        {
            try
            {
                driver.Quit();
            }
            catch { }
            finally
            {
                if (Drivers.Contains(driver))
                    Drivers.Remove(driver);
            }
        }

        #endregion
    }
}
