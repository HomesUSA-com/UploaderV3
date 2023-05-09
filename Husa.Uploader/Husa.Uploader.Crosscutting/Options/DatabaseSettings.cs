using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class DatabaseSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string SaborDatabase { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CtxDatabase { get; set; }
    }
}