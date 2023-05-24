namespace Husa.Uploader.SignalR.WebApp.Pages
{
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class Index : PageModel
    {
        private readonly ILogger<Index> logger;

        public Index(ILogger<Index> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnGet()
        {
            this.logger.LogInformation("Getting Information");
        }
    }
}
