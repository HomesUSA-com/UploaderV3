using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class MarketSettings
    {
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}