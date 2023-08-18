namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateStatus
    {
        Task<UploadResult> UpdateStatus(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
