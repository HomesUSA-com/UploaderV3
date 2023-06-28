namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class BrowserSettings
    {
        [Required]
        public IReadOnlyList<UserProfilePreference> UserProfilePreferences { get; set; }

        [Required]
        public IReadOnlyList<string> Arguments { get; set; }
    }
}
