namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class DatabaseSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string SaborDatabase { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CtxDatabase { get; set; }
    }
}
