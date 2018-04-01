using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContentManagement.Controllers
{
    public partial class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public virtual IActionResult Index(int? id)
        {
            var logBuilder = new StringBuilder();

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            logBuilder.AppendLine($"Error {id} for {Request.Method} {statusCodeReExecuteFeature?.OriginalPath ?? Request.Path.Value}{Request.QueryString.Value}\n");

            var exceptionHandlerFeature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature?.Error != null)
            {
                var exception = exceptionHandlerFeature.Error;
                logBuilder.AppendLine($"<h1>Exception: {exception.Message}</h1>{exception.StackTrace}");
            }

            foreach (var header in Request.Headers)
            {
                var headerValues = string.Join(",", value: header.Value);
                logBuilder.AppendLine($"{header.Key}: {headerValues}");
            }
            _logger.LogError(logBuilder.ToString());

            if (id == null)
            {
                return View("Error");
            }

            switch (id.Value)
            {
                case 401:
                case 403:
                    return View("AccessDenied");
                case 404:
                    return View("NotFound");

                default:
                    return View("Error");
            }
        }
    }
}