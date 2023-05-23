namespace Husa.Uploader.ViewModels.ListingRequestSaleSubClass
{
    using Husa.Uploader.ViewModels.Enum;
    using System.Collections.Generic;

    public class Rooms
    {
        public int? Length { get; set; }
        public int? Width { get; set; }
        public string Level { get; set; }
        public RoomType RoomType { get; set; }
        public string EntityOwnerType { get; }

        public ICollection<RoomFeatures> Features { get; set; }
    }
}
