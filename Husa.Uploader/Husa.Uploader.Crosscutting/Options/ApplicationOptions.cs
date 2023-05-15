using System.ComponentModel.DataAnnotations;

namespace Husa.Uploader.Crosscutting.Options
{
    public class ApplicationOptions
    {
        public const string Section = "Application";

        [Required(AllowEmptyStrings = false)]
        public string AuthenticateServerUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ElasticSearchServerUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SignalRURLServer { get; set; }

        [MinLength(length: 1)]
        public int SingalRRefreshIntervalSeconds { get; set; }

        [MinLength(length: 45)]
        public int DataRefreshIntervalInSeconds { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ApplicationId { get; set; }

        public int ListDateSold { get; set; } = 4;

        [Required]
        public MarketConfiguration MarketInfo { get; set; }
        
        [Required]
        public UploaderSettings Uploader { get; set; }
    }
}
