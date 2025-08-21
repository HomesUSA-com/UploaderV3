namespace Husa.Uploader.Crosscutting.Regex
{
    using System.Text.RegularExpressions;

    public static partial class RegexGenerator
    {
        public static Regex InvalidInlineDots => InvalidInlineDotsRegex();
        public static Regex MultipleSpaces => MultipleSpacesRegex();

        [GeneratedRegex(@"\.{2}()|@\.{4,}")]
        private static partial Regex InvalidInlineDotsRegex();

        [GeneratedRegex(@"\s{2,}")]
        private static partial Regex MultipleSpacesRegex();
    }
}
