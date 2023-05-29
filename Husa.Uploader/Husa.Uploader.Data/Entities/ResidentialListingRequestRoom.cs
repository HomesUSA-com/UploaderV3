namespace Husa.Uploader.Data.Entities
{
    public class ResidentialListingRequestRoom
    {
        public string Dimensions => $"{this.Length} X {this.Width}";

        public int Length { get; set; }

        public int Width { get; set; }

        public string Level { get; set; }

        public string RoomType { get; set; }
    }
}
