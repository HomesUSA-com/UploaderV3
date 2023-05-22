namespace Husa.Uploader.Data.QuicklisterEntities.Sabor
{
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IConvertToUploaderRequest
    {
        bool IsDeleted { get; set; }

        ListingRequestState RequestState { get; set; }

        ResidentialListingRequest ConvertFromCosmos(string marketName, string marketUser, string marketPassword);
    }
}
