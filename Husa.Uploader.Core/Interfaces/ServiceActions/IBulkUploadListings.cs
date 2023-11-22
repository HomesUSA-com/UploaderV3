namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;

    public interface IBulkUploadListings
    {
        MarketCode CurrentMarket { get; }

        RequestFieldChange RequestFieldChange { get; set; }

        Task<UploadResult> Upload(CancellationToken cancellationToken = default);

        void CancelOperation();

        UploadResult Logout();

        void SetRequestFieldChange(RequestFieldChange requestFieldChange);
    }
}
