namespace Husa.Uploader.Data.QuicklisterEntities.Sabor.ValueObjects
{
    using Husa.Uploader.Crosscutting.Enums.Sabor;

    public class Rooms
    {
        public int? Length { get; set; }
        public int? Width { get; set; }
        public string Level { get; set; }
        public RoomType RoomType { get; set; }
        public string EntityOwnerType { get; }
    }
}
