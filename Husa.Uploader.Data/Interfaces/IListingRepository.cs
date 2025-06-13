namespace Husa.Uploader.Data.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Data.Entities.BulkUpload;

    public interface IListingRepository
    {
        Task<IEnumerable<TaxIdBulkUploadListingItem>> GetListingsWithInvalidTaxId(MarketCode marketCode, CancellationToken token = default);
    }
}
