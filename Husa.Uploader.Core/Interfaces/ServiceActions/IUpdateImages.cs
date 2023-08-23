namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateImages
    {
        Task<UploadResult> UpdateImages(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
