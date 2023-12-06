namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateImages
    {
        Task<UploadResult> UpdateImages(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
