namespace Husa.Uploader.Api.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BrowserLogoutActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Implementation for cleanup after action execution
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Implementation for setup before action execution
        }
    }
}
