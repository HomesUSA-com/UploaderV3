namespace Husa.Uploader.Data.Projections
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.Listing;
    using Husa.Uploader.Data.Entities.BulkUpload;

    public static class ListingProjection
    {
        public static TaxIdBulkUploadListingItem ToTaxIdBulkUploadListingItem(this InvalidTaxIdListingsResponse response, MarketCode marketCode) => new()
        {
            Id = response.Id,
            Address = response.Address,
            CompanyId = response.CompanyId,
            Market = marketCode,
            MlsNumber = response.MlsNumber,
            OwnerName = response.OwnerName,
            TaxId = response.TaxId,
            UnitNumber = response.UnitNumber,
        };
    }
}
