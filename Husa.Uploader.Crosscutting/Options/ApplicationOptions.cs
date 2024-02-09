namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class ApplicationOptions
    {
        public const string Section = "Application";

        [Required(AllowEmptyStrings = false)]
        public string SignalRURLServer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PublishingPath { get; set; }

        [Range(2, int.MaxValue, ErrorMessage = "Value must be greater than 0.")]
        public int SignalRRefreshIntervalSeconds { get; set; }

        [Range(45, int.MaxValue, ErrorMessage = "Value must be greater than 0.")]
        public int DataRefreshIntervalInSeconds { get; set; }

        [Range(900, int.MaxValue, ErrorMessage = "Value must be greater than 0.")]
        public int VersionCheckIntervalInSeconds { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ApplicationId { get; set; }

        public int ListDateSold { get; set; } = 4;

        [Required]
        public MarketConfiguration MarketInfo { get; set; }

        [Required]
        public UploaderSettings Uploader { get; set; }

        [Required]
        public ServiceSettings Services { get; set; }

        [Required]
        public FeatureFlags FeatureFlags { get; set; }
    }
}
