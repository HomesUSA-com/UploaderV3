namespace Husa.Uploader.Data.Entities.MarketRequests
{
    public class HoaRequest
    {
        public string Name { get; set; }
        public int? Fee { get; set; }
        public string FeePaid { get; set; }
        public int? TransferFee { get; set; }
        public HoaPhone Phone { get; set; }
        public string Website { get; set; }
    }
}
