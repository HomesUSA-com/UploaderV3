namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IEditListing
    {
        Task<UploadResult> Edit(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
