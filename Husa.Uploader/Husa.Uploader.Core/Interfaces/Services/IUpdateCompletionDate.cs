using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;

namespace Husa.Uploader.Core.Interfaces.Services
{
    public interface IUpdateCompletionDate
    {
        UploadResult UpdateCompletionDate(ResidentialListingRequest listing);
    }
}
