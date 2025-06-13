namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using System.Threading;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Data.Entities.LotListing;

    public interface IUploadListing
    {
        MarketCode CurrentMarket { get; }

        bool IsFlashRequired { get; }

        bool CanUpload(ResidentialListingRequest listing);

        Task<UploaderResponse> Upload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        Task<UploaderResponse> PartialUpload(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);

        Task<UploaderResponse> TaxIdUpdate(TaxIdBulkUploadListingItem listing, bool logIn = true, CancellationToken cancellationToken = default);

        Task<LoginResult> Login(Guid companyId);

        UploaderResponse Logout();

        void CancelOperation();

        Task<UploaderResponse> UploadLot(LotListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true);
    }
}
