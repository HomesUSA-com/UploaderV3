namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class ServiceSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string Media { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string QuicklisterSabor { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string QuicklisterCtx { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string QuicklisterAbor { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CompanyServicesManager { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MigrationService { get; set; }
    }
}
