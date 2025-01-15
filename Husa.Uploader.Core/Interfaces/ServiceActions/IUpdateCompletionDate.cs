namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IUpdateCompletionDate
    {
        Task<UploadResult> UpdateCompletionDate(ResidentialListingRequest listing, CancellationToken cancellationToken = default, bool logIn = true, bool autoSave = false);
    }
}
