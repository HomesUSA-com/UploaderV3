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
    }
}
