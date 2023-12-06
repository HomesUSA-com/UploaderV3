namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdatePrice
    {
        Task<UploadResult> UpdatePrice(ResidentialListingRequest listing, CancellationToken cancellationToken = default);
    }
}
