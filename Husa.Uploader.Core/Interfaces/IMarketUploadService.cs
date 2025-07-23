namespace Husa.Uploader.Core.Interfaces
{
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Uploader.Core.Interfaces.ServiceActions;

    public interface IMarketUploadService :
        IUploadListing,
        IEditListing,
        IUpdateImages,
        IUpdatePrice,
        IUpdateStatus,
        IUpdateCompletionDate,
        IUpdateVirtualTour,
        IUpdateOpenHouse
    {
        IUploaderClient UploaderClient { get; }
        IServiceSubscriptionClient ServiceSubscriptionClient { get; }
    }
}
