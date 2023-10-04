namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using Husa.Uploader.Crosscutting.Enums;

    public class OpenHouseRequest
    {
        public string Date { get; set; }
        public OpenHouseType Type { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool Active { get; set; }
        public string Comments { get; set; }
        public string Refreshments { get; set; }
        public string Lunch { get; set; }
        public string VirtualOpenHouseUrl { get; set; }
    }
}
