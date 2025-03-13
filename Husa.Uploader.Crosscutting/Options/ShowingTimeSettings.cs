namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class ShowingTimeSettings
    {
        [Required]
        public Credentials Credentials { get; set; }
    }
}
