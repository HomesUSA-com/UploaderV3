using System;
using System.Collections.Generic;

namespace Husa.Core.UploaderBase
{
    #region Commons 
    
    public interface IUploader
    {
        bool CanUpload(ResidentialListingRequest listing);

        UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media);

        LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing);

        UploadResult Logout(CoreWebDriver driver);

        bool IsFlashRequired { get; }
    }

    #endregion

    #region Listings

    public interface IEditor : IUploader
    {
        UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    public interface IImageUploader : IUploader
    {
        UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media);
    }

    public interface IPriceUploader : IUploader
    {
        UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    public interface IStatusUploader : IUploader
    {
        UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    public interface ICompletionDateUploader : IUploader
    {
        UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    public interface IUpdateOpenHouseUploader : IUploader
    {
        UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    public interface IUploadVirtualTourUploader : IUploader
    {
        UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media);
    }

    #endregion

    #region Leasing

    public interface ILeaseUploader : IUploader
    {
        UploadResult UpdateLease(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media);
        
    }

    public interface IStatusLeaseUploader : IUploader
    {
        UploadResult UpdateStatusLease(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    #endregion

    #region Lots

    public interface ILotUploader : IUploader
    {
        UploadResult UpdateLot(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media);
    }

    public interface IStatusLotUploader : IUploader
    {
        UploadResult UpdateStatusLot(CoreWebDriver driver, ResidentialListingRequest listing);
    }

    #endregion
}