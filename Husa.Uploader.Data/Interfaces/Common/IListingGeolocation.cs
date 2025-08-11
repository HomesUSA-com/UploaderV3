namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IListingGeolocation
    {
        bool UpdateGeocodes { get; set; }
        decimal? Latitude { get; set; }
        decimal? Longitude { get; set; }
    }
}
