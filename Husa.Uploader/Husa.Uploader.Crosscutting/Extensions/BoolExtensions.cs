namespace Husa.Uploader.Crosscutting.Extensions
{
    public static class BoolExtensions
    {
        public static string BoolToYesNoBool(this bool value) => value ? "YES" : "NO";

        public static string BoolToNumericBool(this bool value) => value ? "1" : "0";
    }
}
