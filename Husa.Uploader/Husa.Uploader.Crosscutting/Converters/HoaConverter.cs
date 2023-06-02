namespace Husa.Uploader.Crosscutting.Converters
{
    public static class HoaConverter
    {
        public static string ToStringFromHasMultipleHOA(this int numHoas) => numHoas > 1 ? "YES" : "NO";

        public static string TruncateHOAWebsite(this string website) => website.Length > 70 ? website[..70] : website;
    }
}
