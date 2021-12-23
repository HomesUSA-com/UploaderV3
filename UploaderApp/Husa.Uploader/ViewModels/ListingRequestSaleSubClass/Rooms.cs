namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    using Husa.Cargador.ViewModels.Enum;
    using System;

    public class Rooms
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Level { get; set; }

        public RoomTypeEnum Type { get; set; }

        public string Dimensions { get; set; }
    }
}
