using Husa.Uploader.Core.BrowserTools;
using Husa.Uploader.Core.Interfaces;
using Husa.Uploader.Core.Models;
using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Reflection;

namespace Husa.Uploader.Core
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
            ////IUploader prime = new SanAntonioUploader();

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

        private static UploadResponse ExecUpload<T>(
            Func<T, CoreWebDriver, ResidentialListingRequest, UploadResult> func,
            ResidentialListingRequest listing,
            ILogger logger)
            where T : IUploader
        {
            CoreWebDriver webDriver = null;
            try
            {
                var upl = (T)GetUploader(listing);
                webDriver = GetWebDriver(listing.ResidentialListingRequestID, upl.IsFlashRequired, logger);

                var result = func(upl, webDriver, listing);

                return new()
                {
                    Driver = webDriver,
                    Result = result,
                    Uploader = upl,
                    Exception = null,
                    Listing = listing
                };
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while executing Update market data");
                return new()
                {
                    Driver = webDriver,
                    Result = UploadResult.Failure,
                    Uploader = null,
                    Exception = exception,
                    Listing = listing
                };
            }
        }

        public static UploadResponse Upload(ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media, ILogger logger) =>
            ExecUpload<IUploader>((uploader, driver, residentialListing) => uploader.Upload(driver, residentialListing, media), listing, logger);

        public static UploadResponse Edit(ResidentialListingRequest listing, ILogger logger) =>
            ExecUpload<IEditor>((uploader, driver, residentialListing) => uploader.Edit(driver, residentialListing), listing, logger);

        public static UploadResponse UpdateStatus(ResidentialListingRequest listing, ILogger logger) =>
            ExecUpload<IStatusUploader>((uploader, driver, residentialListing) => uploader.UpdateStatus(driver, residentialListing), listing, logger);

        public static UploadResponse UpdatePrice(ResidentialListingRequest listing, ILogger logger) =>
            ExecUpload<IPriceUploader>((uploader, driver, residentialListing) => uploader.UpdatePrice(driver, residentialListing), listing, logger);

        public static UploadResponse UpdateCompletionDate(ResidentialListingRequest listing, ILogger logger) =>
            ExecUpload<ICompletionDateUploader>((uploader, driver, residentialListing) => uploader.UpdateCompletionDate(driver, residentialListing), listing, logger);

        public static UploadResponse UpdateImages(ResidentialListingRequest listing, IEnumerable<IListingMedia> media, ILogger logger) =>
            ExecUpload<IImageUploader>((uploader, driver, residentialListing) => uploader.UpdateImages(driver, residentialListing, media), listing, logger);

        public static UploadResponse UpdateOpenHouse(ResidentialListingRequest listing, ILogger logger) =>
            ExecUpload<IUpdateOpenHouseUploader>((uploader, driver, residentialListing) => uploader.UpdateOpenHouse(driver, residentialListing), listing, logger);

        public static UploadResponse UpdateVirtualTour(ResidentialListingRequest listing, IEnumerable<IListingMedia> media, ILogger logger) =>
            ExecUpload<IUploadVirtualTourUploader>((uploader, driver, residentialListing) => uploader.UploadVirtualTour(driver, residentialListing, media), listing, logger);

        private static CoreWebDriver GetWebDriver(
            Guid requestId,
            bool allowFlash,
            ILogger logger)
        {
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

        public static bool UploaderSupports<T>(ResidentialListingRequest listing)
            where T : IUploader
        {
            if (listing == null)
            {
                return false;
            }

            var uploader = Uploaders.FirstOrDefault(c => c.CanUpload(listing));
            if (uploader == null)
            {
                return false;
            }

            var supportsAction = uploader.GetType().GetInterfaces().Any(c => c == typeof(T));
            return supportsAction;
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
                {
                    Drivers.Remove(driver);
                }
            }
        }
    }
}
