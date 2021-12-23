#if DEBUG
using System;
using System.Collections.Generic;
using Husa.Core.UploaderBase;

namespace Husa.Cargador
{
    public class InterfaceTestUploader : IUploader, IImageUploader, IPriceUploader, IStatusUploader
    {
        public bool IsFlashRequired { get { return false; } }

        public static ResidentialListingRequest GetRequest(UploadResult expectedResult)
        {
            return new ResidentialListingRequest()
            {
                MarketName = "UiTestMarket",
                MlsStatus = expectedResult.ToString()
            };
        }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "UiTestMarket";
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            return (UploadResult)Enum.Parse(typeof(UploadResult), listing.MlsStatus);
        }

        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            return (UploadResult)Enum.Parse(typeof(UploadResult), listing.MlsStatus);
        }

        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            return (UploadResult)Enum.Parse(typeof(UploadResult), listing.MlsStatus);
        }

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            return (UploadResult)Enum.Parse(typeof(UploadResult), listing.MlsStatus);
        }

        public UploadResult Logout(CoreWebDriver driver)
        {
            return UploadResult.Success;
        }

        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            return LoginResult.Logged;
        }

        public UploadResult UploadLeasing(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new NotImplementedException();
        }
    }
}
#endif