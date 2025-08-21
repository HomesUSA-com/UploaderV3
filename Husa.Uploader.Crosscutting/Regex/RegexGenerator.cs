namespace Husa.Uploader.Crosscutting.Regex
{
    using System.Text.RegularExpressions;

    public static partial class RegexGenerator
    {
        public static Regex InvalidInlineDots => InvalidInlineDotsRegex();

        [GeneratedRegex(@"\.{2}()|@\.{4,}")]
        private static partial Regex InvalidInlineDotsRegex();
    }
}
