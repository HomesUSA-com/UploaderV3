namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;
    using Husa.Extensions.Common.Enums;

    public class MarketSettings
    {
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public MarketCode MarketCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LoginUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LogoutUrl { get; set; }
        public string AgentId { get; set; }
        public string SupervisorId { get; set; }
    }
}
