namespace Husa.Uploader.Core.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Client.Interfaces;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Core.Models;

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
        Task<UploaderResponse> FullUploadListing(MarketCode marketCode, Guid requestId, CancellationToken cancellationToken, bool logInFirstStepOnly = true, bool autoSave = true);
    }
}
