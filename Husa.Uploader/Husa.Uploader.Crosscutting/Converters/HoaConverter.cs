namespace Husa.Uploader.Crosscutting.Converters
{
    public static class HoaConverter
    {
        public static string ToStringFromHasMultipleHOA(this int numHoas) => numHoas > 1 ? "YES" : "NO";
    }
}
