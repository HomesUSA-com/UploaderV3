namespace Husa.Uploader.Api.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandlingFilterAttribute> logger;

        public ExceptionHandlingFilterAttribute(ILogger<ExceptionHandlingFilterAttribute> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionMessage = context.Exception.GetBaseException().Message;
            var errorMessage = string.Empty;
            switch (context.Exception)
            {
                case HttpRequestException httpException when httpException.StatusCode == HttpStatusCode.NotFound:
                    errorMessage = httpException.Message;
                    context.Result = new BadRequestObjectResult(httpException.Message);
                    break;
                default:
                    errorMessage = $"An unhandled exception of type {context.Exception.GetType().FullName} was thrown.";
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Result = new JsonResult(exceptionMessage);
                    break;
            }

            this.logger.LogError(
                context.Exception,
                "errorMessage: {ErrorMessage} \n\n exceptionMessage: {ExceptionMessage}",
                errorMessage,
                exceptionMessage);

            context.ExceptionHandled = true;
            return base.OnExceptionAsync(context);
        }
    }
}
