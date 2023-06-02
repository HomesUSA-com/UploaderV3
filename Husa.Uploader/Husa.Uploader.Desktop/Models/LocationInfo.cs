namespace Husa.Uploader.Desktop.Models
{
    public record LocationInfo
    {
        public LocationInfo(decimal latitude, decimal longitude)
            : this()
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public LocationInfo()
        {
            this.Longitude = 0;
            this.Latitude = 0;
        }

        public decimal Latitude { get; init; }

        public decimal Longitude { get; init; }

        public bool IsValidLocation => this.Latitude != 0 && this.Longitude != 0;
    }
}
