using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBotAPI.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this._logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;

            Handle((dynamic)exception, context);
        }

        public void Handle(Exception ex, ExceptionContext context)
        {
            _logger.LogError($"[My LOG]\tMessage: {ex.Message}");

            context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(new { message = ex.Message });
        }
    }
}
