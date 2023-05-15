using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class UploaderSettings
    {
        [Required]
        public BrowserSettings ChromeOptions { get; set; }
    }
}
