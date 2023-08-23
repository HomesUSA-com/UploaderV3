namespace Husa.Uploader.Data.Entities
{
    public class HoaPhone
    {
        public HoaPhone(string phone)
        {
            this.AreaCode = phone != null ? phone.Substring(0, 3) : string.Empty;
            this.Prefix = phone != null ? phone.Substring(3, 3) : string.Empty;
            this.LineNumber = phone != null ? phone.Substring(6, 4) : string.Empty;
        }

        public string AreaCode { get; set; }
        public string Prefix { get; set; }
        public string LineNumber { get; set; }

        public bool IsEmpty() =>
            string.IsNullOrEmpty(this.AreaCode)
                || string.IsNullOrEmpty(this.Prefix)
                || string.IsNullOrEmpty(this.LineNumber);
    }
}
