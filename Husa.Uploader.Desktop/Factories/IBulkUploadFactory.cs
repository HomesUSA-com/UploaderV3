namespace Husa.Uploader.Desktop.Factories
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Data.Entities;

    public interface IBulkUploadFactory
    {
        public IBulkUploadListings Uploader { get; }

        T Create<T>(MarketCode marketCode, RequestFieldChange requestFieldChange, List<UploadListingItem> bulkListings);

        void CloseDriver();
    }
}
