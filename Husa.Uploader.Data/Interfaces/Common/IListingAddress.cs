namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IListingAddress : IListingGeolocation
    {
        string StreetNum { get; set; }
        string StreetName { get; set; }
        string StreetType { get; set; }
        string County { get; set; }
        string City { get; set; }
        string CityCode { get; set; }
        string State { get; set; }
        string Zip { get; set; }
    }
}
