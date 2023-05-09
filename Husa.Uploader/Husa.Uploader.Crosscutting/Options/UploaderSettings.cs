using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class UploaderSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string LoginUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LogoutUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required]
        public BrowserSettings ChromeOptions { get; set; }
    }
}
