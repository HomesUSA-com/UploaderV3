namespace Husa.Uploader.Crosscutting.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveSlash(this string stringWithSlashes)
        {
            if (string.IsNullOrWhiteSpace(stringWithSlashes))
            {
                return stringWithSlashes;
            }

            if (stringWithSlashes.Contains('/'))
            {
                stringWithSlashes = stringWithSlashes.Replace('/', newChar: '-');
            }
            else if (stringWithSlashes.Contains('\\'))
            {
                stringWithSlashes = stringWithSlashes.Replace('\\', newChar: '-');
            }
            else if (stringWithSlashes.Contains('"'))
            {
                stringWithSlashes = stringWithSlashes.Replace("\"", newValue: string.Empty);
            }

            return stringWithSlashes;
        }

        public static string DecimalToString(this decimal? amount) => amount.HasValue ? ((int)amount.Value).ToString() : string.Empty;
        public static string StrictDecimalToString(this decimal? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;

        public static string IntegerToString(this int? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;
    }
}
