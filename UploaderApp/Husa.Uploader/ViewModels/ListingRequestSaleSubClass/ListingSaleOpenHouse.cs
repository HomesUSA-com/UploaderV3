namespace Husa.Uploader.ViewModels.ListingRequestSaleSubClass
{
    using System;

    public class ListingSaleOpenHouse
    {
        public string Type { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool Refreshments { get; set; }

        public bool Lunch { get; set; }

        public string OpenHouseType { get; set; }
    }
}
