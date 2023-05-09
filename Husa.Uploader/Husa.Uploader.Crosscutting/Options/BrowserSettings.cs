using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class BrowserSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string DriverPath { get; set; }

        [Required]
        public IReadOnlyList<UserProfilePreference> UserProfilePreferences { get; set; }

        [Required]
        public IReadOnlyList<string> Arguments { get; set; }
    }
}
