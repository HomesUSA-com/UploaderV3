namespace Husa.Uploader.Data.Entities
{
    public class HoaPhone
    {
        public HoaPhone(string phone)
        {
            this.AreaCode = phone.Substring(0, 3);
            this.Prefix = phone.Substring(3, 3);
            this.LineNumber = phone.Substring(6, 4);
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
