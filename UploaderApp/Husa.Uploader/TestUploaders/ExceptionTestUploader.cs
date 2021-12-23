#if DEBUG
using System;
using System.Collections.Generic;
using Husa.Core.UploaderBase;

namespace Husa.Cargador
{
    public class ExceptionTestUploader : IUploader
    {
        public bool IsFlashRequired { get { return false; } }

        public static ResidentialListingRequest GetRequest()
        {
            return new ResidentialListingRequest()
            {
                MarketName = "ExceptionTestMarket",
            };
        }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "ExceptionTestMarket";
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new Exception("This is a TEST UploadException");
        }

        public UploadResult Logout(CoreWebDriver driver)
        {
            throw new Exception("This is a TEST LogoutException");
        }

        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            throw new Exception("This is a TEST LoginException");
        }

        public UploadResult UploadLeasing(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new NotImplementedException();
        }
    }
}
#endif