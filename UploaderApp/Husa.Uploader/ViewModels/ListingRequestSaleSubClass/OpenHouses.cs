namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    using Husa.Cargador.ViewModels.Enum;
    public class OpenHouses
    {
        public OpenHouseTypeEnum Type { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Refreshments { get; set; }

        public string Comments { get; set; }
    }
}
