namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateVirtualTour
    {
        Task<UploaderResponse> UploadVirtualTour(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
