namespace Husa.Uploader.Desktop.Factories
{
    using System.Collections.Generic;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Data.Entities.BulkUpload;

    public interface ITaxIdBulkUploadFactory
    {
        public ITaxIdBulkUploadListings Uploader { get; }

        T Create<T>(MarketCode marketCode, List<TaxIdBulkUploadListingItem> bulkListings);

        void CloseDriver();
    }
}
