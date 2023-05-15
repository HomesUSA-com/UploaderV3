using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Data.Entities;
using Husa.Uploader.Data.Interfaces;

namespace Husa.Uploader.Core.Interfaces.Services
{
    public interface IUpdateImages
    {
        UploadResult UpdateImages(ResidentialListingRequest listing, IEnumerable<IListingMedia> media);
    }
}
