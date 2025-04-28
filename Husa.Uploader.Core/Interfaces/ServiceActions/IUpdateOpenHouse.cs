namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateOpenHouse
    {
        Task<UploaderResponse> UpdateOpenHouse(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
