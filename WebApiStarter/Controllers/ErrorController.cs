using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiStarter.Controllers
{
    public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleErrorDevelopment()
        {
            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            //logger.LogError(exceptionHandlerFeature.Error, "Unhandled exception");

            return Problem(
                detail: null,
                title: exceptionHandlerFeature.Error.Message);
        }
    }
}
