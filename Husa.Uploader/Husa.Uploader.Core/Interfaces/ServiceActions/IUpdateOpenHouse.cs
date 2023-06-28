namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateOpenHouse
    {
        Task<UploadResult> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
