namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class UploaderSettings
    {
        [Required]
        public BrowserSettings ChromeOptions { get; set; }
    }
}
