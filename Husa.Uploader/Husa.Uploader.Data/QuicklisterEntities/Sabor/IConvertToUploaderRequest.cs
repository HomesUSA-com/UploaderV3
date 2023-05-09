using Husa.Uploader.Data.Entities;

namespace Husa.Uploader.Data.QuicklisterEntities.Sabor
{
    public interface IConvertToUploaderRequest
    {
        bool IsDeleted { get; set; }

        ResidentialListingRequest ConvertFromCosmos(string marketName, string marketUser, string marketPassword);
    }
}